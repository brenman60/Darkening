using UnityEngine;

public class UICursor : MonoBehaviour
{
    public static UICursor Instance { get; private set; }
    public static bool hovering;

    [SerializeField] private Vector2 defaultSize;
    [SerializeField] private Vector2 hoverSize;

    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;

        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, hovering ? hoverSize : defaultSize, Time.deltaTime * 5f);
    }
}
