using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/New Enemy", fileName = "New Enemy")]
public class EnemyProperties : ScriptableObject
{
    public string displayName = "New Enemy";
    public NightSection nightSection;
    public float movementChanceThreshold = 1;
    public GameObject enemyPrefab;
}
