using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public static Flashlight Instance { get; private set; }

    [HideInInspector] public bool lockedOff;

    public Light light_;
    public float lightSpeed = 5f;

    private bool on;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Keybinds.Instance.flashlight.performed += FlashlightClicked;
    }

    private void FlashlightClicked(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (lockedOff) return;

        on = !on;

        if (BeastController.Instance.CurrentRoom == "Door" && on && Door.Instance.Open)
            DeathScreen.Instance.KillPlayer("Door");

        SoundManager.Instance.PlayAudio("FlashlightClick", true, 1f, transform);
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (lockedOff)
            on = false;

        ChangeBrightness();
    }

    private void ChangeBrightness()
    {
        float newIntensity = on ? 8.5f : 0.0f;
        light_.intensity = Mathf.Lerp(light_.intensity, newIntensity, Time.deltaTime * lightSpeed);
    }

    private void OnDestroy()
    {
        Keybinds.Instance.flashlight.performed -= FlashlightClicked;
    }
}