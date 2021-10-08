using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameoverController : MonoBehaviour
{
    [Header("�����e���ʵe����")]
    public Animator aniFinal;
    [Header("�������D")]
    public Text textFinalTitle;
    [Header("�C���ӧQ�P���Ѥ�r")]
    [TextArea(1, 3)]
    public string stringWin = "�J�f���u�ó��Q�M���F\n�~�򩹫e�a\n(����....)";
    [TextArea(1, 3)]
    public string stringLose = "�u�äӱj�j�F...\n";
    [Header("���s�P���}���s")]
    public KeyCode kcReplay = KeyCode.R;
    public KeyCode kcQuit = KeyCode.Q;

    private bool isGameover;

    private void Update()
    {
        Replay();
        Quit();
    }

    private void Replay()
    {
        if (isGameover && Input.GetKeyDown(kcReplay)) SceneManager.LoadScene("�C������");
    }

    private void Quit()
    {
        if (isGameover && Input.GetKeyDown(kcQuit)) Application.Quit();
    }

    public void ShowGameOverView(bool win)
    {
        isGameover = true;
        aniFinal.enabled = true;

        if (win) textFinalTitle.text = stringWin;
        else textFinalTitle.text = stringLose;
    }
}
