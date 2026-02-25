using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    public float speedMultiple { get; private set; }

    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float crouchingHeight;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private int groundMask;
    [SerializeField] private Transform groundCheck;

    private Camera playerCamera;
    private AudioSource heartbeat;
    private float verticalRotation = 0.0f;
    private float stepSoundDebounce;
    private Vector3 velocity;
    private bool isGrounded;
    private bool crouching;
    private float height;
    private Vector3 lastPosition;
    private CharacterController characterController;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        heartbeat = GetComponent<AudioSource>();

        height = characterController.height;

        Keybinds.crouch.performed += CrouchPressed;
    }

    private void CrouchPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        crouching = !crouching;
        height += crouching ? -crouchingHeight : crouchingHeight;
    }

    private void Update()
    {
        CameraUpdate();
        Movement();
        HeartbeatVolume();
    }

    private void Movement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = 0f;

        stepSoundDebounce -= Time.deltaTime;

        Vector2 move = Keybinds.movement.ReadValue<Vector2>();
        bool sprinting = Keybinds.sprint.ReadValue<float>() != 0f;

        Vector3 movement = transform.right * move.x + transform.forward * move.y;
        if (sprinting && !crouching)
        {
            movement *= 2f;
            stepSoundDebounce -= Time.deltaTime;
        }
        else if (crouching)
        {
            movement *= 0.35f;
            stepSoundDebounce += Time.deltaTime * 0.35f;
        }

        characterController.Move(movementSpeed * Time.deltaTime * movement);

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        characterController.height = Mathf.MoveTowards(characterController.height, height, Time.deltaTime * 5f);

        if (Mathf.Abs(move.x) > .25f || Mathf.Abs(move.y) > .25f && stepSoundDebounce < 0)
        {
            if (stepSoundDebounce > 0)
                return;

            stepSoundDebounce = .6f;
            SoundManager.Instance.PlayAudio("CarpetStep", true, 0.15f, transform);
        }

        speedMultiple = (sprinting && !crouching && move != Vector2.zero) ? 2f : (!crouching && move != Vector2.zero ? 1f : (crouching && move != Vector2.zero) ? 0.5f : 0f);
    }

    private void HeartbeatVolume()
    {
        float normalizedProgress = GameManager.Instance.nightTime / ((GameManager.Night.nightLength / 60) * 30.0f);

        float totalHours = 12.0f - 6.0f;
        float currentHourFloat = totalHours * normalizedProgress / 2;
        int currentHour = Mathf.FloorToInt(currentHourFloat);

        heartbeat.volume = Mathf.Lerp(heartbeat.volume, currentHour / 6f, Time.deltaTime * 5f);
    }

    private void CameraUpdate()
    {
        if (Time.timeScale == 0) return;

        if (CameraSystemUI.Instance)
            if (!CameraSystemUI.Instance.On)
                Cursor.lockState = CursorLockMode.Locked;

        Vector2 mousePos = Keybinds.mousePosition.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * mousePos.x * mouseSensitivity);
        verticalRotation -= mousePos.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90.0f, 90.0f);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void OnDestroy()
    {
        Keybinds.crouch.performed -= CrouchPressed;
    }
}