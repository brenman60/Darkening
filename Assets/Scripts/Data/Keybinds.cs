using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinds : MonoBehaviour
{
    public static Keybinds Instance { get; private set; }

    public static PlayerInput playerControls { get; private set; }
    public static InputAction movement { get; private set; }
    public static InputAction sprint { get; private set; }
    public static InputAction interact { get; private set; }
    public static InputAction hold { get; private set; }
    public static InputAction flashlight { get; private set; }
    public static InputAction pause { get; private set; }
    public static InputAction mousePosition { get; private set; }
    public static InputAction jumpscare { get; private set; }
    public static InputAction crouch { get; private set; }
    public static InputAction scroll { get; private set; }

    //private static bool saveLoaded = false;

    private float saveTimer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject keybindObject = Instantiate(Resources.Load<GameObject>("Utils/Keybinds"));
            keybindObject.name = "Keybinds";

            Keybinds keybinds = keybindObject.GetComponent<Keybinds>();
            Instance = keybinds;

            DontDestroyOnLoad(keybindObject);
        }
    }

    private void Update()
    {
        if (saveTimer > 0)
        {
            saveTimer -= Time.deltaTime;
            if (saveTimer <= 0)
                SaveSystem.SaveGlobal();
        }
    }

    private void OnEnable()
    {
        InputSystem.EnableDevice(Mouse.current);

        playerControls = new PlayerInput();

        movement = playerControls.Gameplay.Movement;
        movement.Enable();

        sprint = playerControls.Gameplay.Sprint;
        sprint.Enable();

        interact = playerControls.Gameplay.Interact;
        interact.Enable();

        hold = playerControls.Gameplay.Hold;
        hold.Enable();

        flashlight = playerControls.Gameplay.Flashlight;
        flashlight.Enable();

        pause = playerControls.Misc.Pause;
        pause.Enable();

        mousePosition = playerControls.Misc.MousePosition;
        mousePosition.Enable();

        jumpscare = playerControls.Gameplay.Jumpscare;
        jumpscare.Enable();

        crouch = playerControls.Gameplay.Crouch;
        crouch.Enable();

        scroll = playerControls.UI.ScrollWheel;
        scroll.Enable();
    }

    private void OnDisable()
    {
        movement?.Disable();
        sprint?.Disable();
        interact?.Disable();
        hold?.Disable();
        flashlight?.Disable();
        pause?.Disable();
        mousePosition?.Disable();
        jumpscare?.Disable();
        crouch?.Disable();
        scroll?.Disable();
    }
}
