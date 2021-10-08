using UnityEngine;

[CreateAssetMenu(menuName = "Task/任務資料", fileName = "任務資料")]
public class DialogueData : ScriptableObject
{
    [Header("任務內容"), TextArea(2, 5)]
    public string[] dialogueContents;
}
