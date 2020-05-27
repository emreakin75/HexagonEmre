using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public MapGUI mapGUI;
    public CanvasGUI canvasGUI;

    public void Initialize()
    {
        mapGUI.Initialize();
        canvasGUI.Initialize();
    }

}
