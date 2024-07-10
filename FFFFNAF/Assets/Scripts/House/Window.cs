using UnityEngine;

public class Window : MonoBehaviour
{
    public static Window Instance { get; private set; }

    public float windowSpeed = 15f;

    private AudioSource nighttimeAmbient;

    public bool Open
    {
        get
        {
            return open;
        }
    }

    private bool open = false;

    private float heatTicker = 0;

    private float beastWindowTimer = 0;

    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        nighttimeAmbient = GetComponent<AudioSource>();

        mainCam = Camera.main;
    }

    void Update()
    {
        MoveWindow();
        ChangeAmbientVolume();
        HeatTick();
        BeastWindowCheck();
    }

    void HeatTick()
    {
        heatTicker += Time.deltaTime;
        if (heatTicker > 4f / (BeastController.Instance.aggression / 10f))
        {
            heatTicker = 0f;
            GameManager.heat += open ? -1 : 1;
        }
    }

    void MoveWindow()
    {
        float yPos = !open ? 0.2474f : 0.3481f;
        Vector3 newPos = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, Time.deltaTime * windowSpeed);
    }

    void ChangeAmbientVolume()
    {
        float newVolume = open ? 1f : 0f;
        nighttimeAmbient.volume = Mathf.Lerp(nighttimeAmbient.volume, newVolume, Time.deltaTime * windowSpeed);
    }

    void BeastWindowCheck()
    {
        bool beastInWindow = BeastController.Instance.CurrentRoom == "Window";
        if (beastInWindow)
            beastWindowTimer += Time.deltaTime;

        float angle = Vector3.Angle(mainCam.transform.forward, transform.position - mainCam.transform.position);

        if (beastInWindow)
        {
            if (angle < 45f && !CameraSystemUI.Instance.On)
                DeathScreen.Instance.KillPlayer("Window");

            if (beastWindowTimer > 10f)
                DeathScreen.Instance.KillPlayer("Window");
        }
    }

    public void ToggleWindow()
    {
        open = !open;
    }
}
