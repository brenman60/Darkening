using UnityEngine;

public class BeastBreathing : MonoBehaviour
{
    private AudioSource breathingAudio;
    private float initialVolume;

    private void Start()
    {
        breathingAudio = GetComponent<AudioSource>();
        initialVolume = breathingAudio.volume;
    }

    void Update()
    {
        float newVolume = Door.Instance.Open_ ? initialVolume : 0f;
        breathingAudio.volume = Mathf.Lerp(breathingAudio.volume, newVolume, Time.deltaTime * 10f);
    }
}