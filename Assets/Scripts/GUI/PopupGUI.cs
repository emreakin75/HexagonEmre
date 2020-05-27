using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopupGUI : MonoBehaviour
{
    public Button restartBtn;

    public void Initialize()
    {
        gameObject.SetActive(true);

        restartBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.AddListener(restartBtnClick);
    }

    private void restartBtnClick()
    {
        SceneManager.LoadScene(0);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

}
