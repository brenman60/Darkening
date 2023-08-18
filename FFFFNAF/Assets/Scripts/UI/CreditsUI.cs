using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    public MainMenuUI mainMenuUI;

    public bool open = false;

    private CanvasGroup cg;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        VisibilityUpdate();
    }

    void VisibilityUpdate()
    {
        float newAlpha = open ? 1.0f : -1.0f;
        cg.alpha += newAlpha * (Time.deltaTime * 5f);
        cg.interactable = open;
        cg.blocksRaycasts = open;
    }

    public void GoBack()
    {
        open = false;
        mainMenuUI.open = true;
    }
}