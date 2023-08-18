using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionUI : MonoBehaviour
{
    public static TransitionUI Instance { get; private set; }

    private Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenScene(string sceneName)
    {
        StartCoroutine(OpenScene_());
        IEnumerator OpenScene_()
        {
            animator.SetBool("Closed", true);
            yield return new WaitForSeconds(1.5f);

            AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
            
            while (!scene.isDone)
            {
                if (scene.progress >= 0.9f)
                {
                    scene.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}