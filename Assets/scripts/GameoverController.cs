using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameoverController : MonoBehaviour
{
    [Header("結束畫面動畫元件")]
    public Animator aniFinal;
    [Header("結束標題")]
    public Text textFinalTitle;
    [Header("遊戲勝利與失敗文字")]
    [TextArea(1, 3)]
    public string stringWin = "入口的守衛都被清除了\n繼續往前吧\n(待續....)";
    [TextArea(1, 3)]
    public string stringLose = "守衛太強大了...\n";
    [Header("重新與離開按鈕")]
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
        if (isGameover && Input.GetKeyDown(kcReplay)) SceneManager.LoadScene("遊戲場景");
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
