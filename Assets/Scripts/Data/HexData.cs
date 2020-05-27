public class HexData
{
    public Color color;
    public bool isBomb;
    public int bombCounter;

    public HexData() { }
    public HexData(Color color, bool isBomb, int bombCounter)
    {
        this.color = color;
        this.isBomb = isBomb;
        this.bombCounter = bombCounter;
    }

}
