using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private RectTransform buttonOutline;
    private PositionConstraint buttonOutlinePosConstraint;

    private static bool usingController;

    private GameObject lastUISelection;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance == null)
        {
            GameObject managerObject = Instantiate(Resources.Load<GameObject>("Utils/UIManager"));
            managerObject.name = "UIManager";

            UIManager manager = managerObject.GetComponent<UIManager>();
            Instance = manager;

            DontDestroyOnLoad(managerObject);
        }
    }

    private void Start()
    {
        //buttonOutline.gameObject.SetActive(false);
        //buttonOutlinePosConstraint = buttonOutline.GetComponent<PositionConstraint>();
    }

    private void Update()
    {
        CheckForController();

        if (usingController)
        {
            /*GameObject currentSelection = EventSystem.current.currentSelectedGameObject;
            if (currentSelection != null)
                lastUISelection = currentSelection;

            if (currentSelection == null)
                EventSystem.current.SetSelectedGameObject(lastUISelection);*/
        }
    }

    private void CheckForController()
    {
        if (Gamepad.all.Count > 0)
        {
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.buttonEast.wasPressedThisFrame || // A button
                    gamepad.buttonWest.wasPressedThisFrame || // B button
                    gamepad.buttonNorth.wasPressedThisFrame || // X button
                    gamepad.buttonSouth.wasPressedThisFrame || // Y button
                    gamepad.leftShoulder.wasPressedThisFrame || // L1
                    gamepad.rightShoulder.wasPressedThisFrame || // R1
                    gamepad.leftStickButton.wasPressedThisFrame || // L3
                    gamepad.rightStickButton.wasPressedThisFrame) // R3
                {
                    usingController = true;
                }
            }
        }
    }

    private void UpdateButtonOutline(DarkeningButton button)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        buttonOutline.sizeDelta = rectTransform.sizeDelta + new Vector2(10f, 10f);

        for (int i = 0; i < buttonOutlinePosConstraint.sourceCount; i++)
            buttonOutlinePosConstraint.RemoveSource(0);

        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = button.transform;
        constraintSource.weight = 1f;
        buttonOutlinePosConstraint.AddSource(constraintSource);
    }

    private void PlayClickSound(DarkeningButton button)
    {
        SoundManager.Instance.PlayAudio(button.clickSound, false, 1f);
    }

    public void AddButton(DarkeningButton button)
    {
        button.onClick.AddListener(() => PlayClickSound(button));

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        //AddEventTrigger(trigger, EventTriggerType.Select, () => UpdateButtonOutline(button));
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }
}
