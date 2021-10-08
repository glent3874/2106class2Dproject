using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [Header("���ȸ��")]
    public DialogueData data;
    [Header("�L�X���j"), Range(0, 3)]
    public float interval = 0.2f;
    [Header("�T�{�ϥ�")]
    public GameObject ConfirnIcon;
    [Header("��r����: ���Ȥ��e")]
    public Text textContent;
    [Header("�~���ܪ�����")]
    public KeyCode kcContinue = KeyCode.Space;

    private CanvasGroup groupDialogue;

    private void Start()
    {
        groupDialogue = transform.GetChild(0).GetComponent<CanvasGroup>();

        StartDialogue();
    }

    private void StartDialogue()
    {
        StartCoroutine(ShowEveryDialogue());
    }

    private IEnumerator ShowEveryDialogue()
    {
        groupDialogue.alpha = 1;
        textContent.text = "";

        for (int i = 0; i < data.dialogueContents.Length; i++)
        {
            for (int j = 0; j < data.dialogueContents[i].Length; j++)
            {
                textContent.text += data.dialogueContents[i][j];
                yield return new WaitForSeconds(interval);
            }

            ConfirnIcon.SetActive(true);

            while(!Input.GetKeyDown(kcContinue))
            {
                yield return null;
            }

            textContent.text = "";
            ConfirnIcon.SetActive(false);

            if (i == data.dialogueContents.Length - 1) groupDialogue.alpha = 0;
        }
    }
}
