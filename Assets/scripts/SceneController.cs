using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadGameScene()
    {
        Invoke("DelayLoadGameScene", 1);
    }

    private void DelayLoadGameScene()
    {
        SceneManager.LoadScene("�C������");
    }

    public void QuitGame()
    {
        Invoke("DelayQuitGame", 1);
    }

    private void DelayQuitGame()
    {
        Application.Quit();
        print("���}");
    }
}
