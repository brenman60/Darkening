using UnityEngine;

public class DogCage : MonoBehaviour
{
    private AudioSource dogBarking;
    private float initialVolume;

    private void Start()
    {
        dogBarking = GetComponent<AudioSource>();
        initialVolume = dogBarking.volume;
        dogBarking.volume = 0f;
    }

    void Update()
    {
        bool bark = BeastController.Instance.CurrentRoom == "LivingRoom";
        dogBarking.volume = Mathf.Lerp(dogBarking.volume, bark ? initialVolume : 0f, Time.deltaTime * 2.5f);
    }
}