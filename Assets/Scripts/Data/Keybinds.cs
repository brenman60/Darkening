using UnityEngine;
using UnityEngine.InputSystem;

public class Keybinds : MonoBehaviour
{
    public static Keybinds Instance { get; private set; }

    public PlayerInput playerControls { get; private set; }
    public InputAction movement { get; private set; }
    public InputAction sprint { get; private set; }
    public InputAction interact { get; private set; }
    public InputAction hold { get; private set; }
    public InputAction flashlight { get; private set; }
    public InputAction pause { get; private set; }
    public InputAction mousePosition { get; private set; }
    public InputAction jumpscare { get; private set; }

    private static bool saveLoaded = false;

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
    }
}
