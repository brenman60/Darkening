using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SoundEvent : MonoBehaviour
{
    public static SoundEvent instance;

    [SerializeField]
    public enum Sound
    {
        CarpetStep,
        Jumpscare1,
        DoorKnocking1,
        DoorKnocking2,
        DoorKnocking3,
        WindowMovement,
        None,
        DoorOpening,
        DoorClosing,
        FlashlightClick,
        CameraClick,
        CameraClick2,
    }

    private void Awake()
    {
        instance = this;
    }

    public static void PlaySound(Sound sound, Vector3 soundPos, bool isUniversal, Transform parent, float volumeDecrease = 1)
    {
        GameObject soundGameObject = new GameObject("Sound");
        soundGameObject.transform.SetParent(parent);
        soundGameObject.transform.position = new Vector3(soundPos.x, soundPos.y, soundPos.z);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.volume /= volumeDecrease;

        if (!isUniversal)
            audioSource.spatialBlend = 1f;

        audioSource.PlayOneShot(GetAudioClip(sound));
        Destroy(soundGameObject, GetAudioClip(sound).length);
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundEvents.SoundAudioClip soundAudioClip in SoundEvents.instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
                return soundAudioClip.audioClip;
        }

        Debug.Log("Sound " + sound + " not found!");
        return null;
    }
}
