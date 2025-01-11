using UnityEngine;

public class PauseUI : MonoBehaviour
{
    private CanvasGroup cg;

    private bool open = false;

    private float openDebounce;

    private CursorLockMode previousLockMode;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();

        Keybinds.Instance.pause.performed += Paused;
    }

    private void Paused(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (openDebounce > 0) return;

        open = !open;
        openDebounce = 2.5f;

        if (open)
        {
            Time.timeScale = 0f;
            previousLockMode = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = previousLockMode;
        }
    }

    private void Update()
    {
        openDebounce -= Time.unscaledDeltaTime;
        SetVisibility();
    }

    void SetVisibility()
    {
        float newAlpha = open ? 1f : 0f;
        cg.alpha = Mathf.Lerp(cg.alpha, newAlpha, Time.unscaledDeltaTime * 5f);
        cg.interactable = open;
        cg.blocksRaycasts = open;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        TransitionUI.Instance.OpenScene("Game");
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        TransitionUI.Instance.OpenScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        Keybinds.Instance.pause.performed -= Paused;
    }
}