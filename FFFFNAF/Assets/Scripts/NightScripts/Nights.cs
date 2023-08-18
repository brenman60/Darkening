using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nights/Night List Template")]
public class Nights : ScriptableObject
{
    public List<Night> nights = new List<Night>();

    public Night GetNight(int nightNumber)
    {
        foreach (Night night in nights)
            if (night.nightNumber == nightNumber)
                return night;

        Debug.LogError("Night: " + nightNumber + " not found.");
        return null;
    }
}