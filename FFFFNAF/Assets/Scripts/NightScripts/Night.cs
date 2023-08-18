using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Nights/Night Template")]
public class Night : ScriptableObject
{
    public int nightNumber = 1;
    [Tooltip("Length of time in seconds. Recommended to use multiples of 60.")] public int nightLength = 180;
    public float beastStartTime = 0f;
    public int beastSpeed = 10;
    public int beastAggression = 1;

    public bool closetBeast = false;

    [Space(15)]

    public string nightStartText = "Night 1";

    [Space(25)]

    public float nightDialogueStartTime = 60f;
    public DialogueMessage[] dialogues;
}

[Serializable]
public struct DialogueMessage
{
    [TextArea] public string dialogueText;
    public float letterWaitTime;
    public float showTime;
}