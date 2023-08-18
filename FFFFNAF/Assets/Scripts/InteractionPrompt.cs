using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public string alternateText = string.Empty;

    public string keyRequired = "E";

    public float cooldownTime = 5f;
    public float visibilityRange = 5f;
    public float openSpeed = 15f;
    public float thresholdAngle = 30f;

    [Space(10)]
    public SoundEvent.Sound sound = SoundEvent.Sound.None;

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

        mainCam = Camera.main;
    }

    void Update()
    {
        visibilityDebounce -= Time.deltaTime;

        SetRotation();
        SetPosition();
        SetVisibility();
        CheckInput();
    }

    void SetVisibility()
    {
        float distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
        float angle = Vector3.Angle(mainCam.transform.forward, transform.position - mainCam.transform.position);
        bool visible = distance < visibilityRange && visibilityDebounce <= 0 && angle < thresholdAngle && !Player.Instance.lockMovement ? true : false;
        Vector3 newSize = visible ? initialSize : Vector3.zero;
        isOpen = visible;
        transform.localScale = Vector3.Lerp(transform.localScale, newSize, Time.deltaTime * openSpeed);
    }
    
    void SetPosition()
    {
        transform.position = initialPos;
    }

    void SetRotation()
    {
        transform.LookAt(Player.Instance.transform.position);

        Vector3 newRot = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Euler(newRot);
    }

    void CheckInput()
    {
        if (isOpen && visibilityDebounce <= 0)
        {
            if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), keyRequired)))
            {
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

                if (sound != SoundEvent.Sound.None)
                    SoundEvent.PlaySound(sound, transform.position, false, null, 1);

                textAlternated = !textAlternated;
            }
        }
    }
}