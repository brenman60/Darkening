using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    // this script should be disabled in actual builds


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            string screenshotFilename = "screenshot_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            ScreenCapture.CaptureScreenshot(screenshotFilename);
        }
    }
}