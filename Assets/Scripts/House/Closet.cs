using System.Collections;
using UnityEngine;

public class Closet : MonoBehaviour
{
    public static Closet Instance { get; private set; }

    public Transform playerPosition;
    public Transform doorHinge1;
    public Transform doorHinge2;
    public AudioSource doorKnocking;
    public AudioSource doorSlam;

    public float openSpeed = 2.5f;
    private float startOpenSpeed;

    public bool Open
    {
        get
        {
            return open;
        }
    }

    public bool Open_
    {
        get
        {
            return _open;
        }
    }

    private bool open = false;
    private bool _open = false;

    private float holdingDebounce;
    private float openTime;
    private bool closing;

    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        startOpenSpeed = openSpeed;
        mainCam = Camera.main;

        Keybinds.Instance.hold.performed += HoldPressed;
        Keybinds.Instance.interact.performed += Interacted;
    }

    private void Interacted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!open) return;

        closing = true;
    }

    private void HoldPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (holdingDebounce > 0) return;

        open = !open;

        if (!open)
        {
            doorSlam.Play();
            holdingDebounce = 1f;
            openSpeed = startOpenSpeed * 2.5f;
            Flashlight.Instance.lockedOff = true;
        }
        else
        {
            if (holdingDebounce <= 0)
                SoundManager.Instance.PlayAudio("DoorOpening", true, 1f, transform);

            holdingDebounce = 1f;
            openSpeed = startOpenSpeed;
            Flashlight.Instance.lockedOff = false;
        }
    }

    private void Update()
    {
        CheckInputs();
        ToggleDoors();
    }

    private void CheckInputs()
    {
        if (Keybinds.Instance.hold.ReadValue<float>() == 0)
            holdingDebounce -= Time.deltaTime;

        if (!_open)
            return;
        else if (open)
            openTime += Time.deltaTime;
    }

    private void ToggleDoors()
    {
        float newYRot1 = open ? -90f : 0f;
        Vector3 newEulerRot1 = new Vector3(doorHinge1.rotation.eulerAngles.x, newYRot1, doorHinge1.rotation.eulerAngles.z);
        Quaternion newRot1 = Quaternion.Euler(newEulerRot1);
        doorHinge1.rotation = Quaternion.RotateTowards(doorHinge1.rotation, newRot1, Time.deltaTime * openSpeed);

        float newYRot2 = open ? 90f : 0f;
        Vector3 newEulerRot2 = new Vector3(doorHinge2.rotation.eulerAngles.x, newYRot2, doorHinge2.rotation.eulerAngles.z);
        Quaternion newRot2 = Quaternion.Euler(newEulerRot2);
        doorHinge2.rotation = Quaternion.RotateTowards(doorHinge2.rotation, newRot2, Time.deltaTime * openSpeed);
    }

    public void StartLooking()
    {
        StartCoroutine(StartLooking_());
        IEnumerator StartLooking_()
        {
            AmbientLight.Instance.On = false;
            Player.Instance.LockPlayer();
            Player.Instance.SetCameraTransform(playerPosition.position, playerPosition.rotation.eulerAngles);

            SoundManager.Instance.PlayAudio("DoorOpening", true, 1f, transform);

            openSpeed = startOpenSpeed;

            open = true;
            _open = true;

            Flashlight.Instance.lockedOff = false;

            yield return new WaitForSeconds(.1f);
            yield return new WaitUntil(() => closing);
            closing = false;

            Player.Instance.ResetTransform(true);

            bool wasOpen = open;
            openSpeed = startOpenSpeed * 5f;
            open = false;
            _open = false;

            Flashlight.Instance.lockedOff = false;
            AmbientLight.Instance.On = true;

            if (wasOpen)
                SoundManager.Instance.PlayAudio("DoorOpening", true, 1f, transform);

            yield return new WaitForSeconds(.3f);

            if (wasOpen)
                SoundManager.Instance.PlayAudio("DoorClosing", true, 1f, transform);
        }
    }

    private void OnDestroy()
    {
        Keybinds.Instance.hold.performed -= HoldPressed;
        Keybinds.Instance.interact.performed -= Interacted;
    }
}