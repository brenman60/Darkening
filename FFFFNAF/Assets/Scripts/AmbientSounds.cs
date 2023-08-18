using UnityEngine;

public class AmbientSounds : MonoBehaviour
{
    private float ambientTimer = 20f;

    private void Update()
    {
        ambientTimer -= Time.deltaTime;
        if (ambientTimer < 0)
        {
            AudioSource randomAmbientSound = transform.GetChild(Random.Range(0, transform.childCount)).GetComponent<AudioSource>();
            randomAmbientSound.Play();
            ambientTimer = randomAmbientSound.clip.length + Random.Range(10f, 50f);
        }
    }
}