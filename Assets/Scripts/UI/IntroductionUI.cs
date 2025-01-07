using System.Collections;
using UnityEngine;

public class IntroductionUI : MonoBehaviour
{
    private static bool introduced;

    public CanvasGroup disclaimerCG;
    public CanvasGroup titleCG;
    private CanvasGroup cg;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 1f;

        if (introduced)
            return;

        introduced = true;

        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        cg.alpha = 1f;

        // Wait for initial loading
        yield return new WaitForSeconds(2.5f);

        // Fade disclaimer in
        while (disclaimerCG.alpha < 1)
        {
            disclaimerCG.alpha += Time.deltaTime * 1.5f;

            if (Input.GetKeyDown(KeyCode.Return))
                disclaimerCG.alpha = 1;

            yield return new WaitForEndOfFrame();
        }

        // Wait for disclaimer reading
        yield return new WaitForSeconds(2.5f);

        // Fade disclaimer out
        while (disclaimerCG.alpha > 0)
        {
            disclaimerCG.alpha -= Time.deltaTime * 1.5f;

            if (Input.GetKeyDown(KeyCode.Return))
                disclaimerCG.alpha = 0;

            yield return new WaitForEndOfFrame();
        }

        // Wait for a bit before flashing title
        yield return new WaitForSeconds(.5f);

        // Fade title in
        while (titleCG.alpha < 1)
        {
            titleCG.alpha += Time.deltaTime * 1.5f;

            if (Input.GetKeyDown(KeyCode.Return))
                titleCG.alpha = 1;

            yield return new WaitForEndOfFrame();
        }

        // Wait for title reading
        yield return new WaitForSeconds(2.5f);

        // Fade title out (by fading everything out)
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime * 1.5f;

            if (Input.GetKeyDown(KeyCode.Return))
                cg.alpha = 0;

            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }
}