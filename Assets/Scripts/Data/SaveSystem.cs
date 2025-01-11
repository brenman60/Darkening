using System.IO;
using UnityEngine;
using AESEncryption;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class SaveSystem
{
    public static bool initialized { get; private set; } = false;

    private static readonly string mainPath = Application.persistentDataPath;
    private static string globalDataPath
    {
        get { return Path.Combine(mainPath, "global.dark"); }
    }

    private static bool globalDataExists
    {
#if !UNITY_WEBGL
        get { return File.Exists(globalDataPath); }
#elif UNITY_WEBGL
        get { return PlayerPrefs.HasKey("Global"); }
#endif
    }

    private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = Formatting.Indented,
    };

#if !UNITY_WEBGL
    public static async void SaveGlobal()
    {
        if (!initialized) return;

        string[] globalData = new string[2]
        {
            Keybinds.Instance.GetSaveData(),
            GameSettings.Instance.GetSaveData(),
        };

        await WriteToFile(globalDataPath, JsonConvert.SerializeObject(globalData, serializerSettings));
        if (Application.isEditor) await WriteToFile(globalDataPath + "debug_.txt", JsonConvert.SerializeObject(globalData, serializerSettings), false);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static async Task LoadGlobal()
    {
        if (initialized) return;
        if (!globalDataExists)
        {
            initialized = true;
            return;
        }

        string[] dataPoints = JsonConvert.DeserializeObject<string[]>(await ReadFromFile(globalDataPath), serializerSettings);
        if (dataPoints.Length >= 1) Keybinds.Instance.PutSaveData(dataPoints[0]);
        if (dataPoints.Length >= 2) GameSettings.Instance.PutSaveData(dataPoints[1]);

        initialized = true;
    }

#elif UNITY_WEBGL
    public static void SaveGlobal()
    {
        if (!initialized) return;

        try
        {
            string[] globalData = new string[2] 
            {
                Keybinds.Instance.GetSaveData(),
                GameSettings.Instance.GetSaveData(),
            };

            PlayerPrefs.SetString("Global", JsonConvert.SerializeObject(globalData, serializerSettings));
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogError("WebGL save data encountered an error while putting global data: " + e.GetBaseException());
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadGlobal()
    {
        if (initialized) return;
        if (!globalDataExists)
        {
            initialized = true;
            return;
        }

        string[] globalData = JsonConvert.DeserializeObject<string[]>(PlayerPrefs.GetString("Global"), serializerSettings);
        Keybinds.Instance.PutSaveData(globalData[0]);
        GameSettings.Instance.PutSaveData(globalData[1]);

        initialized = true;
    }
#endif

    public static async Task<bool> WriteToFile(string filePath, string data, bool encrypt = true)
    {
        try
        {
            using (FileStream stream = File.Exists(filePath) ? new FileStream(filePath, FileMode.Truncate) : File.Create(filePath))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string encrypedData = encrypt ? AesOperation.EncryptString(data) : data;
                    await writer.WriteAsync(encrypedData);

                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error writing to file with path '" + filePath + "': " + e.GetBaseException());
            return false;
        }
    }

    public static async Task<string> ReadFromFile(string filePath)
    {
        try
        {
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string rawData = await reader.ReadToEndAsync();
                    string unencrypted = await AesOperation.DecryptString(rawData);

                    return unencrypted;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading from file '" + filePath + "': " + e.GetBaseException());
            return null;
        }
    }
}
