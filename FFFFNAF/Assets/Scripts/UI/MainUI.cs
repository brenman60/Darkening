using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    public TextMeshProUGUI heatText;

    private void Update()
    {
        heatText.text = "Heat: " + Mathf.FloorToInt(GameManager.heat).ToString();
    }
}