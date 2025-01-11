using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static bool isNight = true;

    public static Night Night;

    public static float heat = 60;
    public static float beastSpeedMultiplier = 1;

    public Nights nights;

    [Space(25)]

    public GameObject mainUI;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI heatText;

    [Space(25)]
    [Header("Night Start Animation")]
    public TextMeshProUGUI nightStartText;
    public Animator nightStartAnimator;

    // Private Components
    private CanvasGroup dialogueCG;
    private AudioSource dialogueAudio;

    public float gameTime { get; private set; }
    public bool gameWon { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dialogueCG = dialogueText.GetComponent<CanvasGroup>();
        dialogueAudio = dialogueText.GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene().name != "Night")
            isNight = false;

        if (isNight)
            StartNight();
    }

    void StartNight()
    {
        if (Night == null)
            Night = nights.GetNight(1);

        // Start Night animations and stuff
        nightStartText.text = Night.nightStartText;
        nightStartAnimator.SetTrigger("NightStart");

        // Reset static variables
        heat = 60f;

        // Set Beast properties
        BeastController.Instance.movementSpeed = Night.beastSpeed;
        BeastController.Instance.aggression = Night.beastAggression;
        BeastController.Instance.closetBeast = Night.closetBeast;

        StartCoroutine(PlayDialogue());
    }

    private void Update()
    {
        if (isNight)
        {
            gameTime += Time.deltaTime;
            if (gameTime >= Night.nightLength && !gameWon)
            {
                gameWon = true;
                EndingScreen.Instance.WinNight();
            }

            HeatCheck();
        }
    }

    private float heatTextFlashDebounce;
    void HeatCheck()
    {
        heatTextFlashDebounce -= Time.deltaTime;

        heat = Mathf.Clamp(heat, 50, 110);

        if (heat > 100)
            beastSpeedMultiplier = 2f;
        else if (heat > 90)
            beastSpeedMultiplier = 1.5f;
        else if (heat > 70)
            beastSpeedMultiplier = 1f;
        else if (heat > 50)
            beastSpeedMultiplier = .75f;

        if (heat == 110)
            DeathScreen.Instance.OverheatPlayer();

        // Flash heat text color
        if (heat >= 95 && heatTextFlashDebounce <= 0)
        {
            bool white = heatText.color == Color.white ? true : false;
            if (white)
                heatText.color = Color.red;
            else
                heatText.color = Color.white;

            heatTextFlashDebounce = .5f;
        }
        else
            heatText.color = Color.white;
    }

    IEnumerator PlayDialogue()
    {
        dialogueCG.alpha = 1f;

        yield return new WaitForSeconds(Night.nightDialogueStartTime);

        foreach (DialogueMessage dialogue in Night.dialogues)
        {
            dialogueText.text = string.Empty;

            for (int i = 0; i < dialogue.dialogueText.Length; i++)
            {
                dialogueText.text += dialogue.dialogueText[i];
                dialogueAudio.Play();
                yield return new WaitForSeconds(dialogue.letterWaitTime);
            }

            yield return new WaitForSeconds(dialogue.showTime);
        }

        dialogueCG.alpha = 0f;
        dialogueText.text = string.Empty;
    }
}
