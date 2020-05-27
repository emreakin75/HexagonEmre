using UnityEngine;

public class HexGUI : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public HexCanvasGUI hexCanvasGUI;

    private Color color;
    private bool isBomb;
    private int bombCounter;
    public void Initialize(Color color)
    {
        SetBombData(false);
        SetColor(color);

        hexCanvasGUI.Initialize();
    }

    public Color getColor()
    {
        return color;
    }

    public void SetColor(Color color)
    {
        this.color = color;
        meshRenderer.material = Utility.guiManager.mapGUI.materials[(int)color];
    }

    private void SetBombData(bool isBomb, int bombCounter = -1)
    {
        this.isBomb = isBomb;
        this.bombCounter = bombCounter;
    }
    public void SetBomb(bool isBomb, int bombCounter = -1)
    {
        SetBombData(isBomb, bombCounter);
        hexCanvasGUI.EnableBomb(isBomb, bombCounter);
    }
    public void PlayTurn()
    {
        if(isBomb)
        {
            bombCounter--;
            if (bombCounter <= 0)
                Utility.gameManager.GameOver();
            else
                hexCanvasGUI.EnableBomb(true, bombCounter);
        }
    }

    public void EnableStar(bool enable)
    {
        hexCanvasGUI.EnableStar(enable);
        if(enable)
        {
            if (isBomb)
                SetBomb(false);
        }
    }

    public bool getIsBomb()
    {
        return isBomb;
    }
    public int getBombCounter()
    {
        return bombCounter;
    }

    public void ChangeHexa(HexData hexData)
    {
        SetColor(hexData.color);
        SetBomb(hexData.isBomb, hexData.bombCounter);
    }

    public HexData getHexData()
    {
        return new HexData(color, isBomb, bombCounter);
    }

}
