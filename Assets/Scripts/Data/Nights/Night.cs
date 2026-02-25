using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nights/New Night")]
public class Night : ScriptableObject
{
    [Header("Night Customization")]
    public int nightNumber = 1;
    public string nightStartText = "Night 1";
    [Tooltip("Formatted in seconds. Minimum length until the night can be completed.")] public int nightLength = 60;
    [Tooltip("Formatted in seconds. Minimum length until each section can be completed.")] public int sectionLength = 60;
    public List<Task> availableTasks = new List<Task>();

    [Header("Enemy Customization")]
    public List<EnemyProperties> availableEnemies = new List<EnemyProperties>();
    [Tooltip("The number the default movement tick will be divided by. Increase = faster movements, Decrease = slower movements")] public float movementTickReduction = 1;
}
