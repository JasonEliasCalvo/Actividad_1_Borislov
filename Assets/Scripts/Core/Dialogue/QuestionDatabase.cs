using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Dialogue System/Question Database")]
public class QuestionDatabase : ScriptableObject
{
    public List<QuestionData> questions = new();
}

[System.Serializable]
public class QuestionEventEntry
{
    public int id;
    [Space]
    public UnityEvent onChoiceCorrectEnd;
    public UnityEvent onChoiceIncorrectEnd;
}
