using System;
using UnityEngine;
using System.Collections.Generic;

public class MapGUI : MonoBehaviour
{
    public GameObject hexPrefab;
    public Transform hexContent;

    public int gridX;
    public int gridY;

    public int perHexaScore;
    public int everyXPointsForBomb;
    public int bombCounter;

    [Range(2, 5)]
    public int availableColorCount;
    public Material[] materials;
    public Sprite[] bombNumberSprites;

    private float xDiffOffset = 0.8f;
    private float yDiffOffset = 0.9f;
    private float yDiffOddOffset = -0.45f;

    private List<List<HexGUI>> hexs;
    private Dictionary<Vector2, Vector2> posCoordinateMap;
    private Dictionary<Vector2, Vector2> coordinatePosMap;

    public void Initialize()
    {
        ClearContent();

        hexs = new List<List<HexGUI>>();
        posCoordinateMap = new Dictionary<Vector2, Vector2>();
        coordinatePosMap = new Dictionary<Vector2, Vector2>();

        for (int y = 0 ; y < gridY ; y++)
        {
            List<HexGUI> rowItem = new List<HexGUI>();
            hexs.Add(rowItem);
            for (int x = 0; x < gridX; x++)
            {
                Vector2 coordinate = new Vector2(x, y);

                GameObject go = Instantiate(hexPrefab, hexContent);
                go.name = "Hexa_" + x + "_" + y;
                HexGUI hexGUI = go.GetComponent<HexGUI>();
                hexGUI.Initialize(getStartingColor(coordinate));

                float xDiff = xDiffOffset * x;
                float yDiff = yDiffOffset * y;
                if (x % 2 == 1)
                    yDiff += yDiffOddOffset;

                Vector2 position = new Vector2(xDiff, yDiff);
                go.transform.localPosition = position;
                rowItem.Add(hexGUI);

                posCoordinateMap.Add(go.transform.position, coordinate);
                coordinatePosMap.Add(coordinate, go.transform.position);
            }
        }
    }
    private void ClearContent()
    {
        for (int i = 0; i < hexContent.childCount; i++)
            Destroy(hexContent.GetChild(i).gameObject);
    }

    // already passed neighbour relative coordinates
    private Neighbour[] oddNeighbours = new Neighbour[3]
    {
        new Neighbour
        {
            neigbour1 = new Vector2(-1, -1),
            neigbour2 = new Vector2(0, -1)
        },
        new Neighbour
        {
            neigbour1 = new Vector2(0, -1),
            neigbour2 = new Vector2(1, -1)
        },
        new Neighbour
        {
            neigbour1 = new Vector2(-1, 0),
            neigbour2 = new Vector2(-1, -1)
        }
    };
    private Neighbour[] evenNeighbours = new Neighbour[1]
    {
        new Neighbour
        {
            neigbour1 = new Vector2(-1, 0),
            neigbour2 = new Vector2(0, -1)
        }
    };
    private Color getStartingColor(Vector2 position)
    {
        // we eliminate starting 3-hexa groups
        Neighbour[] neigbours;
        if (position.x % 2 == 1)
            neigbours = oddNeighbours;
        else
            neigbours = evenNeighbours;

        List<Color> availableColors = getAvailableColors();
        for (int i = 0 ; i < neigbours.Length ; i++)
        {
            Vector2 neighbour1 = position + neigbours[i].neigbour1;
            Vector2 neighbour2 = position + neigbours[i].neigbour2;

            if (neighbour1.x < 0 || neighbour2.x < 0 || neighbour1.y < 0 || neighbour2.y < 0 || neighbour1.x >= gridX || neighbour2.x >= gridX || neighbour1.y >= gridY || neighbour2.y >= gridY)
                continue;

            // if both touching neighbour has same color, we cannot pick that color.
            Color colorNeighbour1 = getHex(neighbour1).getColor();
            Color colorNeighbour2 = getHex(neighbour2).getColor();
            if (colorNeighbour1 == colorNeighbour2)
                availableColors.Remove(colorNeighbour1);
        }

        return availableColors[Utility.GetRandomNumber(0, availableColors.Count)];
    }

    public List<Color> getAvailableColors()
    {
        int colorCount = Enum.GetValues(typeof(Color)).Length;
        List<Color> availableColors = new List<Color>();
        for (int i = 0; i < colorCount && i < availableColorCount; i++)
            availableColors.Add((Color)i);

        return availableColors;
    }

    public Vector2 getCoordinate(Vector2 position)
    {
        return posCoordinateMap[position];
    }
    public Vector2 getPosition(Vector2 coordinate)
    {
        if (coordinatePosMap.ContainsKey(coordinate))
            return coordinatePosMap[coordinate];
        else
            return new Vector2(-1000000, 0);
    }
    public HexGUI getHex(Vector2 coordinate)
    {
        return hexs[(int)coordinate.y][(int)coordinate.x];
    }

    public void PlayTurn()
    {
        for (int i = 0; i < hexs.Count; i++)
            for (int j = 0; j < hexs[i].Count; j++)
                hexs[i][j].PlayTurn();
    }

}
