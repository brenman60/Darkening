using System.Collections;
using UnityEngine;

public class BlackScreen : MonoBehaviour
{
    public static BlackScreen Instance { get; private set; }

    private CanvasGroup cg;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    Coroutine flashBlackCoroutine = null;
    public void FlashBlack(float duration, float fadeSpeed = 5f)
    {
        if (flashBlackCoroutine != null)
            StopCoroutine(flashBlackCoroutine);

        flashBlackCoroutine = StartCoroutine(FlashBlack_());
        IEnumerator FlashBlack_()
        {
            cg.alpha = 1f;

            yield return new WaitForSeconds(duration);

            while (cg.alpha > 0)
            {
                cg.alpha -= Time.deltaTime * fadeSpeed;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}