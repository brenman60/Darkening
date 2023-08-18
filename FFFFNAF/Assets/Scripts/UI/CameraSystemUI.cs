using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraSystemUI : MonoBehaviour
{
    public static CameraSystemUI Instance { get; private set; }

    public float pulseSpeed = 50f;

    [Space(25)]

    public List<Camera> cameras = new List<Camera>();

    [Space(25)]

    public AudioClip[] cameraClickClips;
    public AudioSource cameraClick;
    public TextMeshProUGUI exitText;
    public Button repairButton;
    public Slider repairProgressBar;
    public GameObject disabledCameraScreen;
    public GameObject brokenCameraScreen;
    public AudioSource cameraRepairingAudio;

    private RectMask2D rectMask;
    private AudioSource cameraRecordingAudio;

    public bool On
    {
        get
        {
            return on;
        }
    }

    private bool on = false;

    private float exitTextVibrateAmount;
    private float exitTextVibrationsDebounce = 0;
    private float exitTextRandomMultiplier = 1;
    private Vector3 startingExitTextPos;

    private bool repairingCam;

    public Camera CurrentCam
    {
        get
        {
            return currentCam;
        }
    }

    private Camera currentCam = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rectMask = GetComponentInChildren<RectMask2D>();
        cameraRecordingAudio = GetComponent<AudioSource>();
        startingExitTextPos = exitText.transform.position;
    }

    private void Update()
    {
        ChangeCameraAudioVolume();
        ChangeCameraRepairingVolume();
        ExitTextVibrations();

        if (repairingCam)
        {
            bool camRepaired = SecurityCameras.Instance.RepairCamera(currentCam);
            if (camRepaired)
            {
                SecurityCameras.Instance.camerasBroken.Remove(currentCam);
                repairingCam = false;
                PulseCamera();
            }

            repairProgressBar.value = SecurityCameras.Instance.GetCameraRepairProgress(currentCam);
        }
    }

    void ChangeCameraAudioVolume()
    {
        float newVolume = on ? .25f : 0f;
        cameraRecordingAudio.volume = Mathf.Lerp(cameraRecordingAudio.volume, newVolume, Time.deltaTime * pulseSpeed);
        cameraClick.volume = Mathf.Lerp(cameraRecordingAudio.volume, newVolume, Time.deltaTime * pulseSpeed);
    }

    void ChangeCameraRepairingVolume()
    {
        float newVolume = repairingCam ? .5f : 0f;
        cameraRepairingAudio.volume = Mathf.Lerp(cameraRecordingAudio.volume, newVolume, Time.deltaTime * pulseSpeed);
    }

    void ExitTextVibrations()
    {
        exitTextVibrateAmount -= Time.deltaTime;
        exitTextVibrationsDebounce -= Time.deltaTime;
        if (exitTextVibrationsDebounce > 0)
            return;

        exitTextVibrateAmount = Random.Range(3f, 6f);
        exitTextVibrationsDebounce = Random.Range(2.5f, 6.5f);
        exitTextRandomMultiplier = Random.Range(1f, 3f);

        StartCoroutine(VibrateExitText());
        IEnumerator VibrateExitText()
        {
            while (exitTextVibrateAmount > 1f)
            {
                exitText.transform.position = new Vector3(startingExitTextPos.x + (Random.Range(-exitTextVibrateAmount, exitTextVibrateAmount) * exitTextRandomMultiplier), startingExitTextPos.y, startingExitTextPos.z);
                yield return new WaitForEndOfFrame();
            }

            exitText.transform.position = startingExitTextPos;
        }
    }

    public void ChangeCamera(Camera selectedCamera)
    {
        if (selectedCamera == currentCam)
            return;

        foreach (Camera cam in cameras)
        {
            bool isCam = cam == selectedCamera;
            cam.gameObject.SetActive(isCam);

            if (isCam)
                currentCam = cam;
        }

        PulseCamera();
    }

    Coroutine pulseCameraCoroutine = null;
    public void PulseCamera()
    {
        if (pulseCameraCoroutine != null)
            StopCoroutine(pulseCameraCoroutine);

        // Stop repairing the last cam we were on if we were
        repairingCam = false;
        repairButton.gameObject.SetActive(false);
        repairProgressBar.gameObject.SetActive(false);

        disabledCameraScreen.SetActive(false);
        brokenCameraScreen.SetActive(false);

        // 550 is the minimum padding to make the top and bottom connect in the middle and make everything disappear
        rectMask.padding = new Vector4(rectMask.padding.x, 550, rectMask.padding.z, 550);

        // Play clicking sound
        cameraClick.Play();

        pulseCameraCoroutine = StartCoroutine(PulseCamera_());
        IEnumerator PulseCamera_()
        {
            Vector4 requiredSize = new Vector4(0, 0, 0, 0);
            while (rectMask.padding != requiredSize)
            {
                Vector4 newSize = Time.deltaTime * new Vector4(0, 1, 0, 1) * pulseSpeed;
                newSize.y = Mathf.Clamp(newSize.y, 0f, float.MaxValue);
                newSize.w = Mathf.Clamp(newSize.w, 0f, float.MaxValue);
                rectMask.padding -= newSize;
                yield return new WaitForEndOfFrame();
            }
        }

        // Check if this camera is disabled or broken
        if (SecurityCameras.Instance.camerasDisabled.ContainsKey(currentCam))
        {
            // Show disabled screen
            disabledCameraScreen.SetActive(true);
        }

        if (SecurityCameras.Instance.camerasBroken.ContainsKey(currentCam))
        {
            // Show broken screen and repair button + repair bar
            if (SecurityCameras.Instance.camerasBroken[currentCam] == 10)
            {
                repairButton.gameObject.SetActive(true);
                brokenCameraScreen.SetActive(true);
            }
            else
            {
                repairingCam = true;
                repairProgressBar.gameObject.SetActive(true);
                brokenCameraScreen.SetActive(true);
            }
        }
    }

    public void BeginCameraRepair()
    {
        repairingCam = true;
        repairButton.gameObject.SetActive(false);
        repairProgressBar.gameObject.SetActive(true);
    }

    public void ToggleCameras()
    {
        if (currentCam == null)
            ChangeCamera(cameras[0]);

        on = !on;

        if (on)
            PulseCamera();
        else
            repairingCam = false;
    }

    public void InstantShutdown()
    {
        on = false;
        gameObject.SetActive(false);
    }
}