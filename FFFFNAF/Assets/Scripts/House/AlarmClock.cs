using TMPro;
using UnityEngine;

public class AlarmClock : MonoBehaviour
{
    public TextMeshProUGUI timeText;


    private float textVisibleTimer = 0;
    private bool textVisible = true;

    private void Update()
    {
        TextVisibleTimer();
        SetTextVisibility();
    }

    void GetCurrentTime()
    {
        float normalizedProgress = GameManager.Instance.gameTime / ((GameManager.Night.nightLength / 60) * 30.0f);

        float totalHours = 12.0f - 6.0f;
        float currentHourFloat = totalHours * normalizedProgress / 2;
        int currentHour = Mathf.FloorToInt(currentHourFloat);

        int displayHour = (currentHour % 12 == 0) ? 12 : currentHour % 12;

        string timeText_ = Mathf.Abs(displayHour).ToString("D2") + ":" + "00" + " AM";

        timeText.text = timeText_;
    }

    void SetTextVisibility()
    {
        timeText.gameObject.SetActive(textVisible);

        if (textVisible)
            GetCurrentTime();
    }

    void TextVisibleTimer()
    {
        textVisibleTimer += Time.deltaTime;
        if (textVisibleTimer > .5f)
        {
            textVisibleTimer = 0f;
            textVisible = !textVisible;
        }
    }
}