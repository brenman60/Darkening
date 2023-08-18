using UnityEditor;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public Nights nights;

    public GameObject continueButton;

    public CreditsUI creditsUI;

    public bool open = false;

    private CanvasGroup cg;

    private void Start()
    {
        if (PlayerPrefs.HasKey("night"))
            continueButton.SetActive(true);
        else
            continueButton.SetActive(false);

        cg = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        VisibilityUpdate();
        Cursor.lockState = CursorLockMode.None;
    }

    void VisibilityUpdate()
    {
        float newAlpha = open ? 1.0f : -1.0f;
        cg.alpha += newAlpha * (Time.deltaTime * 5f);
        cg.interactable = open;
        cg.blocksRaycasts = open;
    }

    public void Play()
    {
        GameManager.Night = nights.GetNight(1);
        PlayerPrefs.SetInt("night", 1);

        TransitionUI.Instance.OpenScene("Game");
    }
    
    public void Continue()
    {
        GameManager.Night = nights.GetNight(PlayerPrefs.GetInt("night"));

        TransitionUI.Instance.OpenScene("Game");
    }

    public void Credits()
    {
        open = false;
        creditsUI.open = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}