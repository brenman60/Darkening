using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float interactionRange;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float interactionUISpeed = 5f;

    [Header("References")]
    [SerializeField] private CanvasGroup interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;

    private Camera cam;
    private Interactable interactable = null;

    private void Start()
    {
        cam = GetComponent<Camera>();

        Keybinds.interact.performed += Interact;
    }

    private void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (interactable == null) return;
        else if (!interactable.active) return;

        interactable.Interact();
    }

    private void Update()
    {
        InteractionCheck();
        InteractionUI();
    }

    private void InteractionCheck()
    {
        Ray raycast = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(raycast, out RaycastHit hit, interactionRange, interactionLayer))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                this.interactable = interactable;
                interactionText.text = interactable.interactText;
            }
            else
                interactable = null;
        }
        else
            interactable = null;
    }

    private void InteractionUI()
    {
        bool visible = interactable != null;
        interactionUI.alpha = Mathf.Lerp(interactionUI.alpha, visible ? 1f : 0f, Time.deltaTime * interactionUISpeed);
        UICursor.hovering = visible;
    }

    private void OnDestroy()
    {
        Keybinds.interact.performed -= Interact;
    }
}
