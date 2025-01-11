using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public string alternateText = string.Empty;

    public float cooldownTime = 5f;
    public float visibilityRange = 5f;
    public float openSpeed = 15f;
    public float thresholdAngle = 30f;

    [Space(10)]
    public Sound sound = null;

    [Space(25)]
    public TextMeshProUGUI text;
    public GameObject targetObject;
    public string targetScript;
    public string targetMethod;
    public List<object> methodParameters = new List<object>();

    private float visibilityDebounce;

    private Vector3 initialSize;
    private Vector3 initialPos;
    private string initialText;

    private bool textAlternated = false;
    private bool isOpen = false;

    private Camera mainCam;

    private void Start()
    {
        initialSize = transform.localScale;
        initialPos = transform.position;
        initialText = text.text;

        transform.localScale = Vector3.zero;

        mainCam = Camera.main;

        Keybinds.Instance.interact.performed += Interacted;
    }

    private void Interacted(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isOpen || visibilityDebounce > 0) return;

        visibilityDebounce = cooldownTime;

        if (targetObject == null)
            return;

        Type thisType = Type.GetType(targetScript.ToString());
        MethodInfo theMethod = thisType.GetMethod(targetMethod.ToString(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        object[] parameters = methodParameters.ToArray();
        theMethod.Invoke(targetObject.GetComponent(thisType), parameters);

        if (alternateText != string.Empty && !textAlternated)
            text.text = alternateText;
        else if (textAlternated)
            text.text = initialText;

        if (sound != null)
            SoundManager.Instance.PlayAudio(sound.name, true, 1f, transform);

        textAlternated = !textAlternated;
    }

    private void Update()
    {
        visibilityDebounce -= Time.deltaTime;

        SetRotation();
        SetPosition();
        SetVisibility();
    }

    private void SetVisibility()
    {
        float distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
        float angle = Vector3.Angle(mainCam.transform.forward, transform.position - mainCam.transform.position);
        bool visible = distance < visibilityRange && visibilityDebounce <= 0 && angle < thresholdAngle && !Player.Instance.lockMovement ? true : false;
        Vector3 newSize = visible ? initialSize : Vector3.zero;
        isOpen = visible;
        transform.localScale = Vector3.Lerp(transform.localScale, newSize, Time.deltaTime * openSpeed);
    }

    private void SetPosition()
    {
        transform.position = initialPos;
    }

    private void SetRotation()
    {
        transform.LookAt(Player.Instance.transform.position);

        Vector3 newRot = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Euler(newRot);
    }

    private void OnDestroy()
    {
        Keybinds.Instance.interact.performed -= Interacted;
    }
}