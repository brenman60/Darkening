using UnityEngine;

public class ViewBobbing : MonoBehaviour
{
    // written by ChatGPT i have no idea how to do this
    // i did edit it a tiny bit though i guess

    public bool locked = false;

    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;

    private float timer = 0.0f;
    private float midpoint = 0.0f;

    private void Start()
    {
        midpoint = transform.localPosition.y;
    }

    private void Update()
    {
        if (locked) return;

        Vector2 movement = Keybinds.Instance.movement.ReadValue<Vector2>();

        // Check if the player is moving
        if (Mathf.Abs(movement.x) > 0.1f || Mathf.Abs(movement.y) > 0.1f)
        {
            // Calculate the sinusoidal motion for bobbing
            float waveSlice = Mathf.Sin(timer);
            timer += bobbingSpeed * Time.deltaTime;

            // Apply bobbing to the vertical position of the camera
            float bobbingAmountThisFrame = waveSlice * bobbingAmount;
            float newY = midpoint + bobbingAmountThisFrame;

            // Apply the bobbing motion to the camera's local position
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            // Reset the timer and camera position if the player stops moving
            timer = 0.0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, midpoint, transform.localPosition.z), Time.deltaTime * 5f);
        }
    }
}