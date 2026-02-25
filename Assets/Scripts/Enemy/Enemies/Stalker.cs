using UnityEngine;

public class Stalker : Enemy
{
    protected override void Tick()
    {
        base.Tick();

        float movementChance = Random.Range(0f, 1f);
        if (movementChance >= properties.movementChanceThreshold)
        {
            // Movement successful
        }
    }
}
