using UnityEngine;

public class AmbientLight : MonoBehaviour
{
    public static AmbientLight Instance { get; private set; }

    public bool On
    {
        get
        {
            return on;
        }
        set
        {
            on = value;
        }
    }

    private bool on;

    public float offSpeed;
    public float onSpeed;

    private Light light_;

    private float initialIntensity;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        light_ = GetComponent<Light>();
        initialIntensity = light_.intensity;
    }

    private void Update()
    {
        SetLightIntensity();
    }

    void SetLightIntensity()
    {
        float newIntensity = on ? initialIntensity : 0f;
        light_.intensity = Mathf.Lerp(light_.intensity, newIntensity, Time.deltaTime * (on ? onSpeed : offSpeed));
    }
}