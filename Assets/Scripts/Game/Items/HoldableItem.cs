using UnityEngine;

public class HoldableItem : MonoBehaviour
{
    public bool holding;
    public bool inUse { get; private set; }

    [SerializeField] private Vector3 _defaultPosition;
    public Vector3 defaultPosition { get { return _defaultPosition; } }

    [SerializeField] private Vector3 _usePosition;
    public Vector3 usePosition { get { return _usePosition; } }

    [SerializeField] private Vector3 _defaultRotation;
    public Vector3 defaultRotation { get { return _defaultRotation; } }

    [SerializeField] private Vector3 _useRotation;
    public Vector3 useRotation { get { return _useRotation; } }

    private void Update()
    {
        Vector3 position = holding ? (inUse ? usePosition : defaultPosition) : defaultPosition - new Vector3(0, 1, 0);
        Vector3 rotation = inUse ? useRotation : defaultRotation;
        transform.localPosition = Vector3.Lerp(transform.localPosition, position, Time.deltaTime * 8f);
        transform.localEulerAngles = rotation;
    }
}
