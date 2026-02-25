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

        float movement = Player.Instance.speedMultiple;
        if (movement != 0f)
        {
            float waveSlice = Mathf.Sin(timer);
            timer += bobbingSpeed * Time.deltaTime * (movement);

            float bobbingAmountThisFrame = waveSlice * bobbingAmount;
            float newY = midpoint + bobbingAmountThisFrame;

            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            timer = 0.0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, midpoint, transform.localPosition.z), Time.deltaTime * 5f);
        }
    }
}