public class GameManager
{
    private int moveCount;
    private int score;

    public GameManager()
    {
        moveCount = 0;
        score = 0;
    }

    public int getMoveCount()
    {
        return moveCount;
    }
    public int getScore()
    {
        return score;
    }

    public void IncreaseMove()
    {
        moveCount++;
        Utility.guiManager.canvasGUI.SetMove();
    }
    public void IncreaseScore(int increaseAmount)
    {
        score += increaseAmount;
        Utility.guiManager.canvasGUI.SetScore();
    }

    public void PlayTurn()
    {
        IncreaseMove();
        Utility.guiManager.mapGUI.PlayTurn();
    }
    public void GameOver()
    {
        Utility.guiManager.canvasGUI.popupGUI.Initialize();
    }

}
