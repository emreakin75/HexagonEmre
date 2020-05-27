using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HexCanvasGUI : MonoBehaviour
{
    public GameObject[] lineGOs;
    public GameObject starGO;
    public GameObject bombGO;
    public Image bombImage;

    public Canvas canvas;
    public void Initialize()
    {
        gameObject.SetActive(false);

        ResetLines();
        EnableStar(false);
        EnableBomb(false);

        canvas.worldCamera = Camera.main;
    }

    public void ResetLines()
    {
        for (int i = 0; i < lineGOs.Length; i++)
            lineGOs[i].SetActive(false);

        SetGO();
    }
    public void SetLines(List<HexaDirection> notUsedDirections)
    {
        List<int> diretionInts = new List<int>();
        for (int i = 0; i < notUsedDirections.Count; i++)
            diretionInts.Add((int)notUsedDirections[i]);

        for (int i = 0; i < lineGOs.Length; i++)
            lineGOs[i].SetActive(!diretionInts.Contains(i));

        SetGO();
    }

    public void EnableStar(bool enable)
    {
        starGO.SetActive(enable);

        SetGO();
    }

    public void EnableBomb(bool enable, int counter = -1)
    {
        bombGO.SetActive(enable);
        if(counter != -1)
            bombImage.sprite = Utility.guiManager.mapGUI.bombNumberSprites[counter - 1];

        SetGO();
    }

    private void SetGO()
    {
        CancelInvoke();
        Invoke("SetGOLocal", 0.01f);
    }
    private void SetGOLocal()
    {
        bool isEnabled = false;
        for (int i = 0; i < lineGOs.Length; i++)
            if (lineGOs[i].gameObject.activeSelf)
                isEnabled = true;

        if (bombGO.activeSelf)
            isEnabled = true;

        if (starGO.activeSelf)
            isEnabled = true;

        gameObject.SetActive(isEnabled);

    }

}
