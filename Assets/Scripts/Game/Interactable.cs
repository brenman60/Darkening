using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Customization")]
    public string interactText;
    public float interactCooldown;

    [Header("References")]
    [SerializeField] private UnityEvent interactionAction;

    public bool active
    {
        get
        {
            return active_;
        }
        private set
        {
            active_ = value;
        }
    }

    private bool active_;
    private float cooldownTimer;

    public void Interact()
    {
        if (cooldownTimer > 0) return;

        cooldownTimer = interactCooldown;
        interactionAction.Invoke();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        active = cooldownTimer <= 0f;
    }
}
