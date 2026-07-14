using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> dialogueLines;

    [SerializeField] private int dialogueID;
    public int DialogueID => dialogueID;
}

[Serializable]
public class DialogueLine
{
    [TextArea(4, 6)]
    public string text;
    public Sprite portrait;
}