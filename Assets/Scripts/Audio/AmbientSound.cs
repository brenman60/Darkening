using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    private float initialAudio;
    private float volumeChangeTimer;
    private bool soundEnabled;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initialAudio = audioSource.volume;
    }

    private void Update()
    {
        volumeChangeTimer += Time.deltaTime;

        float newVolume = Mathf.Lerp(soundEnabled ? 0f : initialAudio, soundEnabled ? initialAudio : 0f, volumeChangeTimer / 5f);
        audioSource.volume = newVolume;
    }

    public void ToggleVolume(bool toggle)
    {
        soundEnabled = toggle;
        volumeChangeTimer = 0f;
    }
}
