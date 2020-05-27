using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasGUI : MonoBehaviour
{
    public Button goRight;
    public Button goLeft;
    public Button goTop;
    public Button goBottom;

    public Button restartBtn;

    public Text scoreTxt;
    public Text moveTxt;

    public PopupGUI popupGUI;

    public void Initialize()
    {
        goRight.onClick.RemoveAllListeners();
        goRight.onClick.AddListener(goRightClick);
        goLeft.onClick.RemoveAllListeners();
        goLeft.onClick.AddListener(goLeftClick);
        goTop.onClick.RemoveAllListeners();
        goTop.onClick.AddListener(goTopClick);
        goBottom.onClick.RemoveAllListeners();
        goBottom.onClick.AddListener(goBottomClick);
        restartBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.AddListener(restartBtnClick);

        SetMove();
        SetScore();

        popupGUI.Close();
    }

    private void goRightClick()
    {
        Camera.main.transform.Translate(1, 0, 0);
    }
    private void goLeftClick()
    {
        Camera.main.transform.Translate(-1, 0, 0);
    }

    private void goTopClick()
    {
        Camera.main.transform.Translate(0, 1, 0);
    }
    private void goBottomClick()
    {
        Camera.main.transform.Translate(0, -1, 0);
    }
    private void restartBtnClick()
    {
        SceneManager.LoadScene(0);
    }

    public void SetMove()
    {
        moveTxt.text = "Move: " + Utility.gameManager.getMoveCount();
    }
    public void SetScore()
    {
        scoreTxt.text = "Skor: " + Utility.gameManager.getScore();
    }

}
