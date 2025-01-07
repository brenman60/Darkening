using System.Collections;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen Instance { get; private set; }

    public Animator jumpscareObject;
    public AudioSource jumpscareAudio;
    public CanvasGroup blackScreenCG;

    private CanvasGroup cg;

    private bool open = false;
    private bool blackScreenOpen = false;

    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cg = GetComponent<CanvasGroup>();

        mainCam = Camera.main;
    }

    void Update()
    {
        SetVisibility();
        SetBlackScreenVisibility();
    }

    void SetVisibility()
    {
        float newAlpha = open ? 1.0f : -1.0f;
        cg.alpha += newAlpha * Time.deltaTime * 2f;
        cg.interactable = open;
        cg.blocksRaycasts = open;
    }

    void SetBlackScreenVisibility()
    {
        float newAlpha = blackScreenOpen ? 1.0f : -1.0f;
        blackScreenCG.alpha += newAlpha * Time.deltaTime * 2f;
        blackScreenCG.interactable = blackScreenOpen;
        blackScreenCG.blocksRaycasts = blackScreenOpen;
    }

    public void KillPlayer(string killedBy)
    {
        if (EndingScreen.Instance.NightWon)
            return;

        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(KillPlayer_());
        IEnumerator KillPlayer_()
        {
            if (!open)
            {
                open = true;

                // Make sure player rotation is level (so beast is not scaled weird)
                StartCoroutine(ConstantPlayerFix());

                // Turn off cams as to not block jumpscare visuals
                CameraSystemUI.Instance.InstantShutdown();

                // Perform jumpscare visuals and audio
                jumpscareObject.gameObject.SetActive(true);
                jumpscareAudio.Play();

                Player.Instance.LockPlayer();
                Flashlight.Instance.lockedOff = true;

                yield return new WaitForSeconds(1.25f);

                blackScreenOpen = true;
            }
        }

        IEnumerator ConstantPlayerFix()
        {
            float fixTimer = 0f;
            while (fixTimer < 10f)
            {
                mainCam.transform.rotation = Quaternion.Euler(0, mainCam.transform.rotation.y, 0);
                fixTimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void OverheatPlayer()
    {
        if (EndingScreen.Instance.NightWon)
            return;

        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(OverheatPlayer_());
        IEnumerator OverheatPlayer_()
        {
            if (!open)
            {
                open = true;

                Player.Instance.LockPlayer();
                Flashlight.Instance.lockedOff = true;

                if (CameraSystemUI.Instance.On)
                    CameraSystemUI.Instance.ToggleCameras();

                yield return new WaitForSeconds(1.25f);

                blackScreenOpen = true;
            }
        }
    }
}