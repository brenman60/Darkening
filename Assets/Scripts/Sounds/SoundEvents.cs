using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SoundEvents : MonoBehaviour
{
    private static SoundEvents _instance;

    public static SoundEvents instance
    {
        get
        {
            if (_instance == null) _instance = (Instantiate(Resources.Load("SoundEvents")) as GameObject).GetComponent<SoundEvents>();
            return _instance;
        }
    }

    public List<SoundAudioClip> soundAudioClipArray = new List<SoundAudioClip>();

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundEvent.Sound sound;
        public AudioClip audioClip;
    }
}