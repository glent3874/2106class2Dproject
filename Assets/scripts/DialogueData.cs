using UnityEngine;

[CreateAssetMenu(menuName = "Task/���ȸ��", fileName = "���ȸ��")]
public class DialogueData : ScriptableObject
{
    [Header("���Ȥ��e"), TextArea(2, 5)]
    public string[] dialogueContents;
}
