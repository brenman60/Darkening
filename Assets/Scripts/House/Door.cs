using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public static Door Instance { get; private set; }

    public Transform playerPosition;
    public Transform hinge;

    public float openSpeed = 2.5f;
    private float startOpenSpeed;

    public bool Open
    {
        get
        {
            return _open;
        }
    }

    public bool Open_
    {
        get
        {
            return open;
        }
    }

    private bool open = false;
    private bool _open = false;

    private float holdingDebounce;
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
        if (holdingDebounce > 0 || !_open) return;

        doorClosedTimer = 0f;

        SoundManager.Instance.PlayAudio("DoorOpening", true, 1f, transform);

        holdingDebounce = 1.5f;
        open = !open;

        if (!open)
        {
            openSpeed = startOpenSpeed;
            Flashlight.Instance.lockedOff = true;
        }
        else
        {
            if ((BeastController.Instance.CurrentRoom == "Door" || BeastController.Instance.CurrentRoom == "Hallway") && doorClosedTimer < 3.5f)
                DeathScreen.Instance.KillPlayer("Door");
            else if (doorClosedTimer >= 3.5f)
                BeastController.Instance.ForceIntoRoom("Kitchen");

            if (holdingDebounce <= 0)
                SoundManager.Instance.PlayAudio("DoorOpening", true, 1f, transform);

            openSpeed = startOpenSpeed * 2.5f;
            Flashlight.Instance.lockedOff = false;
        }
    }

    private void Update()
    {
        ToggleDoor();
        HoldDoor();
    }

    private void ToggleDoor()
    {
        float newYRot = open ? -51.21f : 0f;
        Vector3 newEulerRot = new Vector3(hinge.rotation.eulerAngles.x, newYRot, hinge.rotation.eulerAngles.z);
        Quaternion newRot = Quaternion.Euler(newEulerRot);
        hinge.rotation = Quaternion.RotateTowards(hinge.rotation, newRot, Time.deltaTime * openSpeed);
    }

    private float doorClosedTimer = 0;
    private void HoldDoor()
    {
        float hold = Keybinds.Instance.hold.ReadValue<float>();
        if (hold == 0)
            holdingDebounce -= Time.deltaTime;
        else
            doorClosedTimer += Time.deltaTime;
    }

    public void StartListening()
    {
        StartCoroutine(StartListening_());
        IEnumerator StartListening_()
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