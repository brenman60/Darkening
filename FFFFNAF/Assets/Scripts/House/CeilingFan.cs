using System.Collections;
using UnityEngine;

public class CeilingFan : MonoBehaviour
{
    public Transform[] blades;
    [Space(20)]
    public Light ceilingLight;
    [Space(20)]
    public float flickerRate;
    public float flickerSpeed = .25f;
    public float fanSpeed = 5f;
    private bool spinFan = true;

    private AudioSource fanBuzzing;

    public float startFlickerRate;
    private float ceilingLightIntensity;
    private float flickerDebounce;

    private void Start()
    {
        fanBuzzing = GetComponent<AudioSource>();

        ceilingLightIntensity = ceilingLight.intensity;
        startFlickerRate = flickerRate;
    }

    private void Update()
    {
        SpinFan();
        Flicker();
        ChangeBuzzingVolume();
    }

    void SpinFan()
    {
        if (!spinFan)
        {
            // Return fan to original orientation
            for (int i = 0; i < blades.Length; i++)
            {
                Quaternion originalRot = Quaternion.Euler(blades[0].rotation.eulerAngles.x, 90f * i, blades[0].rotation.eulerAngles.z);
                blades[i].rotation = Quaternion.Lerp(blades[i].rotation, originalRot, Time.deltaTime * fanSpeed);
            }

            return;
        }

        for (int i = 0; i < blades.Length; i++)
        {
            Quaternion newRot = Quaternion.Euler(blades[i].rotation.eulerAngles.x, blades[i].rotation.eulerAngles.y + 15f, blades[i].rotation.eulerAngles.z);
            blades[i].rotation = Quaternion.Lerp(blades[i].rotation, newRot, Time.deltaTime * fanSpeed);
        }
    }

    void Flicker()
    {
        flickerDebounce -= Time.deltaTime;

        if (flickerRate <= 0)
            return;

        if (Random.Range(0f, 200f) < flickerRate)
        {
            flickerDebounce = flickerSpeed * 1.5f;
            StartCoroutine(Flicker_());
            IEnumerator Flicker_()
            {
                ceilingLight.intensity = ceilingLightIntensity / 8f;
                yield return new WaitForSeconds(flickerSpeed / 2);
                ceilingLight.intensity = ceilingLightIntensity;
            }
        }
    }

    void ChangeBuzzingVolume()
    {
        float newVolume = spinFan ? 1f : 0f;
        fanBuzzing.volume = Mathf.Lerp(fanBuzzing.volume, newVolume, Time.deltaTime * fanSpeed);
    }
}