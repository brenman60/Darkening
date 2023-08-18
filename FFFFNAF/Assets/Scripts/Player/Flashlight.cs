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

    void Update()
    {
        if (Time.timeScale == 0) return;

        ToggleLight();
        ChangeBrightness();
    }

    void ToggleLight()
    {
        if (Input.GetMouseButtonDown(0) && !lockedOff)
        {
            on = !on;

            if (BeastController.Instance.CurrentRoom == "Door" && on && Door.Instance.Open)
                DeathScreen.Instance.KillPlayer("Door");

            SoundEvent.PlaySound(SoundEvent.Sound.FlashlightClick, transform.position, false, transform, 2.5f);
        }
        else if (lockedOff)
            on = false;
    }

    void ChangeBrightness()
    {
        float newIntensity = on ? 8.5f : 0.0f;
        light_.intensity = Mathf.Lerp(light_.intensity, newIntensity, Time.deltaTime * lightSpeed);
    }
}