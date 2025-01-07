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

    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        startOpenSpeed = openSpeed;
        mainCam = Camera.main;
    }

    private void Update()
    {
        ToggleDoor();
        HoldDoor();
    }

    void ToggleDoor()
    {
        float newYRot = open ? -51.21f : 0f;
        Vector3 newEulerRot = new Vector3(hinge.rotation.eulerAngles.x, newYRot, hinge.rotation.eulerAngles.z);
        Quaternion newRot = Quaternion.Euler(newEulerRot);
        hinge.rotation = Quaternion.RotateTowards(hinge.rotation, newRot, Time.deltaTime * openSpeed);
    }

    private float doorClosedTimer = 0;
    void HoldDoor()
    {
        if (!Input.GetKey(KeyCode.Space))
            holdingDebounce -= Time.deltaTime;
        else if (Input.GetKey(KeyCode.Space))
            doorClosedTimer += Time.deltaTime;

        if (!_open)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && holdingDebounce <= 0)
        {
            doorClosedTimer = 0f;

            SoundEvent.PlaySound(SoundEvent.Sound.DoorOpening, transform.position, false, null, .5f);

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
                    SoundEvent.PlaySound(SoundEvent.Sound.DoorOpening, transform.position, false, null, .5f);

                openSpeed = startOpenSpeed * 2.5f;
                Flashlight.Instance.lockedOff = false;
            }
        }
    }

    public void StartListening()
    {
        StartCoroutine(StartListening_());
        IEnumerator StartListening_()
        {
            AmbientLight.Instance.On = false;
            Player.Instance.LockPlayer();
            Player.Instance.SetCameraTransform(playerPosition.position, playerPosition.rotation.eulerAngles);

            SoundEvent.PlaySound(SoundEvent.Sound.DoorOpening, transform.position, false, null, .5f);

            openSpeed = startOpenSpeed;
            open = true;
            _open = true;

            Flashlight.Instance.lockedOff = false;

            yield return new WaitForSeconds(.1f);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            Player.Instance.ResetTransform(true);

            bool wasOpen = open;
            openSpeed = startOpenSpeed * 5f;
            open = false;
            _open = false;

            Flashlight.Instance.lockedOff = false;
            AmbientLight.Instance.On = true;

            if (wasOpen)
                SoundEvent.PlaySound(SoundEvent.Sound.DoorOpening, transform.position, false, null, .5f);

            yield return new WaitForSeconds(.3f);

            if (wasOpen)
                SoundEvent.PlaySound(SoundEvent.Sound.DoorClosing, transform.position, false, null, 2f);
        }
    }
}