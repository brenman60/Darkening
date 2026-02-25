using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public EnemyProperties properties { get; private set; }

    public virtual void Init(EnemyProperties properties)
    {
        this.properties = properties;

        GameManager.EnemyTick += Tick;
    }

    protected virtual void Tick()
    {
        
    }

    private void OnDestroy()
    {
        GameManager.EnemyTick -= Tick;
    }
}
