using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinds : MonoBehaviour, ISaveData
{
    public static Keybinds Instance { get; private set; }

    public PlayerInput playerControls { get; private set; }
    public InputAction movement { get; private set; }
    public InputAction interact { get; private set; }
    public InputAction hold { get; private set; }
    public InputAction flashlight { get; private set; }
    public InputAction pause { get; private set; }
    public InputAction mousePosition { get; private set; }

    private static bool saveLoaded = false;

    private float saveTimer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject keybindObject = Instantiate(Resources.Load<GameObject>("Utils/Keybinds"));
            keybindObject.name = "Keybinds";

            Keybinds keybinds = keybindObject.GetComponent<Keybinds>();
            Instance = keybinds;

            DontDestroyOnLoad(keybindObject);

            //keybinds.ResetAllBinds();
        }
    }

    private void Update()
    {
        if (saveTimer > 0)
        {
            saveTimer -= Time.deltaTime;
            if (saveTimer <= 0)
                SaveSystem.SaveGlobal();
        }
    }

    public string GetSaveData()
    {
        //string compiledBinds = JsonConvert.SerializeObject(Binds);
        //return compiledBinds;

        return "";
    }

    // Something notable here is that the data will not add any keybinds that were present in the save but missing in the current list of keybinds (it will pretty much discard them)
    public void PutSaveData(string data)
    {
        //ResetAllBinds();

        //Dictionary<KeyType, KeyCode> savedBinds = JsonConvert.DeserializeObject<Dictionary<KeyType, KeyCode>>(data);
        //foreach (KeyValuePair<KeyType, KeyCode> bind in savedBinds)
        //    if (Binds.ContainsKey(bind.Key))
        //        Binds[bind.Key] = bind.Value;

        saveLoaded = true;
    }

    private void OnEnable()
    {
        InputSystem.EnableDevice(Mouse.current);

        playerControls = new PlayerInput();

        movement = playerControls.Gameplay.Movement;
        movement.Enable();

        interact = playerControls.Gameplay.Interact;
        interact.Enable();

        hold = playerControls.Gameplay.Hold;
        hold.Enable();

        flashlight = playerControls.Gameplay.Flashlight;
        flashlight.Enable();

        pause = playerControls.Misc.Pause;
        pause.Enable();

        mousePosition = playerControls.Misc.MousePosition;
        mousePosition.Enable();
    }

    private void OnDisable()
    {
        movement?.Disable();
        interact?.Disable();
        hold?.Disable();
        flashlight?.Disable();
        pause?.Disable();
        mousePosition?.Disable();
    }
}
