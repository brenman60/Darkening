using System.Collections;
using UnityEngine;

public class EndingScreen : MonoBehaviour
{
    public static EndingScreen Instance { get; private set; }

    public Nights nights;

    public float endingScreenOpenSpeed = 2.5f;
    public bool NightWon { get; private set; }

    public CanvasGroup cg;
    public CanvasGroup timeTextCG;
    public AudioSource beepAudio;

    private void Awake()
    {
        Instance = this;
    }

    public void WinNight()
    {
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(WinNight_());
        IEnumerator WinNight_()
        {
            NightWon = true;

            cg.interactable = true;
            cg.blocksRaycasts = true;

            Player.Instance.LockPlayer();
            Flashlight.Instance.lockedOff = true;

            if (CameraSystemUI.Instance.On)
                CameraSystemUI.Instance.ToggleCameras();

            // Make screen visible
            StartCoroutine(MakeScreenVisible());
            IEnumerator MakeScreenVisible()
            {
                while (cg.alpha < 1)
                {
                    cg.alpha += Time.deltaTime * endingScreenOpenSpeed;
                    yield return new WaitForEndOfFrame();
                }
            }

            bool beepOn = false;
            for (int beep = 0; beep < 16f; beep++)
            {
                float newTextAlpha = beepOn ? 1f : 0f;
                timeTextCG.alpha = newTextAlpha;

                if (beepOn)
                    beepAudio.Play();

                yield return new WaitForSeconds(.4f);
                beepOn = !beepOn;
            }

            int nextNight = Mathf.Clamp(GameManager.Night.nightNumber + 1, 1, 5);

            GameManager.Night = nights.GetNight(nextNight);
            PlayerPrefs.SetInt("night", nextNight);

            yield return new WaitForSeconds(1f);

            if (GameManager.Night.nightNumber + 1 != 6)
                TransitionUI.Instance.OpenScene("Game");
            else
                TransitionUI.Instance.OpenScene("MainMenu");
        }
    }
}