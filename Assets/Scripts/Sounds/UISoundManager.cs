using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySound(string sound)
    {
        SoundEvent.Sound _sound = (SoundEvent.Sound)Enum.Parse(typeof(SoundEvent.Sound), sound);
        SoundEvent.PlaySound(_sound, Vector2.zero, true, null);
    }
}