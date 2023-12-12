using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CameraSystem : MonoBehaviour
{
    public Volume globalVolume;
    public VolumeProfile cameraProfile;

    public RectMask2D cameraSystem;
    public CanvasGroup cameraSystemCG;
    public float cameraSystemOpenSpeed = 20f;

    public Transform playerPosition;

    private Camera mainCam;
    private VolumeProfile mainCamProfile;

    private void Start()
    {
        mainCam = Camera.main;
        mainCamProfile = globalVolume.profile;
    }

    private float playerStartFOV = 0;
    public void EnterCameras()
    {
        StartCoroutine(EnterCameras_());
        IEnumerator EnterCameras_()
        {
            Player.Instance.LockPlayer();
            Player.Instance.SetCameraTransform(playerPosition.position, playerPosition.rotation.eulerAngles);
            Cursor.lockState = CursorLockMode.Confined;
            playerStartFOV = mainCam.fieldOfView;

            Flashlight.Instance.lockedOff = true;

            yield return new WaitForSeconds(.25f);

            // Zoom in camera
            while (mainCam.fieldOfView > 20)
            {
                mainCam.fieldOfView -= Time.deltaTime * 50f;
                yield return new WaitForEndOfFrame();
            }

            // Change the current volume to the camera volume
            globalVolume.profile = cameraProfile;

            cameraSystem.padding = new Vector4(cameraSystem.padding.x, 550, cameraSystem.padding.z, 550);

            // Open camera system UI
            mainCam.enabled = false;

            cameraSystemCG.interactable = true;
            cameraSystemCG.blocksRaycasts = true;
            while (cameraSystemCG.alpha < 1)
            {
                cameraSystemCG.alpha += Time.deltaTime * cameraSystemOpenSpeed;
                yield return new WaitForEndOfFrame();
            }

            CameraSystemUI.Instance.ToggleCameras();

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            CameraSystemUI.Instance.ToggleCameras();

            // Put back things that can be changed instantly
            mainCam.enabled = true;
            Player.Instance.ResetTransform(true);
            Cursor.lockState = CursorLockMode.Locked;
            Flashlight.Instance.lockedOff = false;
            mainCam.fieldOfView = playerStartFOV;
            globalVolume.profile = mainCamProfile;

            cameraSystemCG.alpha = 0;
            cameraSystemCG.interactable = false;
            cameraSystemCG.blocksRaycasts = false;
        }
    }
}