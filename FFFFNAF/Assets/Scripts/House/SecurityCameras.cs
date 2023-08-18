using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SecurityCameras : MonoBehaviour
{
    public static SecurityCameras Instance { get; private set; }

    public Dictionary<Camera, float> camerasDisabled = new Dictionary<Camera, float>();
    public Dictionary<Camera, float> camerasBroken = new Dictionary<Camera, float>();

    private float randomDisablingDebounce;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        // Loop through disabled cameras and make them not disabled and stuff
        if (camerasDisabled.Count > 0)
        {
            foreach (KeyValuePair<Camera, float> disabledCam in camerasDisabled.ToArray())
            {
                if (disabledCam.Value <= 0)
                {
                    camerasDisabled.Remove(disabledCam.Key);

                    if (CameraSystemUI.Instance.CurrentCam == disabledCam.Key)
                        CameraSystemUI.Instance.PulseCamera();

                    continue;
                }

                camerasDisabled[disabledCam.Key] -= Time.deltaTime;
            }
        }

        RandomCameraDisabling();
    }

    void RandomCameraDisabling()
    {
        randomDisablingDebounce -= Time.deltaTime;
        if (randomDisablingDebounce <= 0)
        {
            randomDisablingDebounce = Random.Range(25f, 45f);
            DisableCamera(CameraSystemUI.Instance.cameras[Random.Range(0, CameraSystemUI.Instance.cameras.Count)], Random.Range(5f, 15f));
        }
    }

    // Breaking and disabling cameras
    public void DisableCamera(Camera selectedCam, float timeDisabled)
    {
        if (!camerasDisabled.ContainsKey(selectedCam))
        {
            camerasDisabled.Add(selectedCam, timeDisabled);

            if (CameraSystemUI.Instance.CurrentCam == selectedCam)
                CameraSystemUI.Instance.PulseCamera();
        }
    }

    public void BreakCamera(Camera selectedCam)
    {
        if (!camerasBroken.ContainsKey(selectedCam))
        {
            camerasBroken.Add(selectedCam, 10f);

            if (CameraSystemUI.Instance.CurrentCam == selectedCam)
                CameraSystemUI.Instance.PulseCamera();
        }
    }

    // Repairing cameras
    public bool RepairCamera(Camera selectedCam)
    {
        if (camerasBroken.ContainsKey(selectedCam))
        {
            camerasBroken[selectedCam] -= Time.deltaTime;

            return camerasBroken[selectedCam] <= 0;
        }
        else
            return true;
    }

    public float GetCameraRepairProgress(Camera selectedCam)
    {
        if (camerasBroken.ContainsKey(selectedCam))
            return 10f - camerasBroken[selectedCam];
        else
            return 0;
    }
}