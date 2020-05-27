using UnityEngine;

public class Initializer : MonoBehaviour
{
    public GUIManager guiManager;

    private void Start()
    {
        // Editor parameters can be changed in GUIManager/MapGUI. On the Editor, it is GUI/Map

        Utility.gameManager = new GameManager();

        Utility.guiManager = guiManager;

        Utility.guiManager.Initialize();

        Destroy(gameObject);
    }

}
