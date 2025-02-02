using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [HideInInspector] public bool lockMovement;

    public float movementSpeed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public int groundMask;

    [SerializeField] private Transform groundCheck;

    private CharacterController characterController;
    private Camera playerCamera;
    private ViewBobbing viewBobbing;
    private AudioSource heartbeat;
    private float verticalRotation = 0.0f;

    private float stepSoundDebounce;

    private Vector3 velocity;
    bool isGrounded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        viewBobbing = GetComponentInChildren<ViewBobbing>();
        heartbeat = GetComponent<AudioSource>();
    }
    private void Update()
    {
        CameraUpdate();
        Movement();

        if (GameManager.isNight)
        {
            HeartbeatVolume();
        }
    }

    private void Movement()
    {
        if (lockMovement) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = 0f;

        stepSoundDebounce -= Time.deltaTime;

        Vector2 move = Keybinds.Instance.movement.ReadValue<Vector2>();
        bool sprinting = Keybinds.Instance.sprint.ReadValue<float>() != 0f;

        Vector3 movement = transform.right * move.x + transform.forward * move.y;
        if (sprinting)
        {
            movement *= 2f;
            stepSoundDebounce -= Time.deltaTime;
        }

        characterController.Move(movement * movementSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        if (Mathf.Abs(move.x) > .25f || Mathf.Abs(move.y) > .25f && stepSoundDebounce < 0)
        {
            if (stepSoundDebounce > 0)
                return;

            stepSoundDebounce = .6f;
            SoundManager.Instance.PlayAudio("CarpetStep", true, 0.15f, transform);
        }
    }

    private void HeartbeatVolume()
    {
        float normalizedProgress = GameManager.Instance.gameTime / ((GameManager.Night.nightLength / 60) * 30.0f);

        float totalHours = 12.0f - 6.0f;
        float currentHourFloat = totalHours * normalizedProgress / 2;
        int currentHour = Mathf.FloorToInt(currentHourFloat);

        heartbeat.volume = Mathf.Lerp(heartbeat.volume, currentHour / 6f, Time.deltaTime * 5f);
    }

    private void CameraUpdate()
    {
        if (lockMovement || Time.timeScale == 0) return;

        if (CameraSystemUI.Instance)
            if (!CameraSystemUI.Instance.On)
                Cursor.lockState = CursorLockMode.Locked;

        Vector2 mousePos = Keybinds.Instance.mousePosition.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * mousePos.x * mouseSensitivity);
        verticalRotation -= mousePos.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90.0f, 90.0f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    Coroutine setPosition = null;
    Coroutine setRotation = null;
    Coroutine settingCameraTransform = null;
    public void SetCameraTransform(Vector3 newPosition, Vector3 newRotationEuler, float speed = 5, bool unlockControls = false, bool useLocalPos = false)
    {
        lockMovement = true;
        viewBobbing.locked = true;

        if (settingCameraTransform != null)
            StopCoroutine(settingCameraTransform);

        settingCameraTransform = StartCoroutine(_SetCameraTransform());
        IEnumerator _SetCameraTransform()
        {
            bool _setPosition = false;
            bool _setRotation = false;

            setPosition = StartCoroutine(SetPosition());
            IEnumerator SetPosition()
            {
                while ((useLocalPos ? playerCamera.transform.localPosition : playerCamera.transform.position) != newPosition)
                {
                    Vector3 newPos = Vector3.MoveTowards(useLocalPos ? playerCamera.transform.localPosition : playerCamera.transform.position, newPosition, (Time.deltaTime * speed) * Vector3.Distance(playerCamera.transform.position, newPosition));

                    if (useLocalPos)
                        playerCamera.transform.localPosition = newPos;
                    else
                        playerCamera.transform.position = newPos;

                    yield return new WaitForEndOfFrame();

                    if (setPosition == null)
                        break;
                }

                _setPosition = true;
            }

            setRotation = StartCoroutine(SetRotation());
            IEnumerator SetRotation()
            {
                while (playerCamera.transform.rotation.eulerAngles != newRotationEuler)
                {
                    playerCamera.transform.rotation = Quaternion.RotateTowards(playerCamera.transform.rotation, Quaternion.Euler(newRotationEuler), (Time.deltaTime * speed) * Vector3.Distance(playerCamera.transform.rotation.eulerAngles, newRotationEuler));
                    yield return new WaitForEndOfFrame();

                    if (setRotation == null)
                        break;
                }

                _setRotation = true;
            }

            yield return new WaitUntil(() => _setPosition && _setRotation);

            if (unlockControls)
            {
                viewBobbing.locked = false;
            }
        }
    }

    private Vector3 lastLockedPos;
    private Vector3 lastLockedRot;

    public void LockPlayer()
    {
        lastLockedPos = playerCamera.transform.localPosition;
        lastLockedRot = playerCamera.transform.rotation.eulerAngles;
        viewBobbing.locked = true;
        lockMovement = true;
    }

    public void ResetTransform(bool lerpReset = true)
    {
        if (setPosition != null)
            StopCoroutine(setPosition);

        if (setRotation != null)
            StopCoroutine(setRotation);

        if (!lerpReset)
        {
            playerCamera.transform.position = lastLockedPos;
            playerCamera.transform.rotation = Quaternion.Euler(lastLockedRot);
            viewBobbing.locked = false;
            lockMovement = false;
        }
        else
        {
            SetCameraTransform(lastLockedPos, lastLockedRot, 15f, true, true);
            lockMovement = false;
        }
    }
}