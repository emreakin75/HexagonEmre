using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickManager : MonoBehaviour
{
    private bool isStarted;
    private bool isClickable;
    private bool isDowned;
    private Vector3 downMousePosition;

    private Vector3 selectedPosition;

    private readonly int minDistance = 200;
    private readonly int bigNumber = 1000000;
    private List<Vector2> selectedCoordinates;

    private void Start()
    {
        isStarted = false;
        isClickable = true;
        isDowned = false;
        selectedCoordinates = new List<Vector2>();
    }

    private void Update()
    {
        if (!isClickable)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            downMousePosition = Input.mousePosition;
            isDowned = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!isStarted)
            {
                if (UpEvent())
                    isStarted = true;

                return;
            }
            if (!isDowned)
                return;

            isDowned = false;

            float distance = Vector3.Distance(downMousePosition, Input.mousePosition);
            if (distance < minDistance)
                UpEvent();
            else
            {
                Vector2 diff = Input.mousePosition - downMousePosition;

                // is vertical diff is bigger than horizontal diff 
                if(Mathf.Abs(diff.x) < Mathf.Abs(diff.y))
                {
                    if (Input.mousePosition.x < selectedPosition.x && diff.y > 0 || Input.mousePosition.x > selectedPosition.x && diff.y < 0)
                        TakeTurn(true);
                    else
                        TakeTurn(false);
                }
                else
                {
                    if (Input.mousePosition.y > selectedPosition.y && diff.x > 0 || Input.mousePosition.y < selectedPosition.y && diff.x < 0)
                        TakeTurn(true);
                    else
                        TakeTurn(false);
                }
            }
        }
    }
    private bool UpEvent()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            selectedPosition = Input.mousePosition;

            List<Vector2> selectedCoordinates = FindClosestNeigbours(hitInfo.collider.transform.position, hitInfo.point);

            ResetPrevPaintedLines();
            this.selectedCoordinates = selectedCoordinates;
            PaintLine(selectedCoordinates);

            return true;
        }

        return false;
    }
    private void TakeTurn(bool isClockwise)
    {
        StopAllCoroutines();
        StartCoroutine(TakeTurnLocal(isClockwise));
    }
    private IEnumerator TakeTurnLocal(bool isClockwise)
    {
        isClickable = false;
        bool isPlayed = false;

        for (int count = 0; count < 3; count++)
        {
            Turn(isClockwise);
            List<Vector2> groupList = CheckGroup();
            if (groupList.Count > 0)
            {
                do
                {
                    yield return RearrangeGroups(groupList);
                    groupList = CheckGroup();
                } while (groupList.Count > 0);

                isPlayed = true;
                break;
            }
            else
                yield return new WaitForSeconds(1f);
        }

        if(isPlayed)
            Utility.gameManager.PlayTurn();

        isClickable = true;
    }
    private IEnumerator RearrangeGroups(List<Vector2> groupList)
    {
        if (groupList.Count > 0)
        {
            for (int i = 0; i < groupList.Count; i++)
                Utility.guiManager.mapGUI.getHex(groupList[i]).EnableStar(true);

            yield return new WaitForSeconds(2f);

            for (int i = 0; i < groupList.Count; i++)
                Utility.guiManager.mapGUI.getHex(groupList[i]).EnableStar(false);

            int score = Utility.gameManager.getScore();
            int perHexaScore = Utility.guiManager.mapGUI.perHexaScore;

            int scoreIncrease = groupList.Count * perHexaScore;
            Utility.gameManager.IncreaseScore(scoreIncrease);

            int newScore = score + scoreIncrease;

            int everyXPointsForBomb = Utility.guiManager.mapGUI.everyXPointsForBomb;
            int bombCount = newScore / everyXPointsForBomb - score / everyXPointsForBomb;

            List<Color> availableColors = Utility.guiManager.mapGUI.getAvailableColors();
            for (int i = 0; i < groupList.Count; i++)
            {
                for (int j = (int)groupList[i].y; j < Utility.guiManager.mapGUI.gridY - 1; j++)
                {
                    HexGUI hexGUI1 = Utility.guiManager.mapGUI.getHex(new Vector2(groupList[i].x, j));
                    HexGUI hexGUI2 = Utility.guiManager.mapGUI.getHex(new Vector2(groupList[i].x, j + 1));

                    hexGUI1.ChangeHexa(hexGUI2.getHexData());
                }

                Color newColor = (Color)Utility.GetRandomNumber(0, availableColors.Count);
                HexGUI hexGUI = Utility.guiManager.mapGUI.getHex(new Vector2(groupList[i].x, Utility.guiManager.mapGUI.gridY - 1));
                hexGUI.SetColor(newColor);

                if(bombCount > 0)
                {
                    bombCount--;
                    hexGUI.SetBomb(true, Utility.guiManager.mapGUI.bombCounter);
                }
                else
                    hexGUI.SetBomb(false);
            }
        }
    }
    private void Turn(bool isClockwise)
    {
        List <int> orderedIndex = new List<int>();

        List<HexGUI> orderedSelectedHexGUIs = new List<HexGUI>();
        for(int a = 0 ; a < selectedCoordinates.Count ; a++)
        {
            float x = a == 0 ? int.MinValue : int.MaxValue;
            float y = int.MaxValue;
            int index = -1;

            for (int i = 0; i < selectedCoordinates.Count; i++)
            {
                if (orderedIndex.Contains(i))
                    continue;
                
                if ((a == 0 && selectedCoordinates[i].x > x || a > 0 && selectedCoordinates[i].x < x) || selectedCoordinates[i].x == x && selectedCoordinates[i].y < y)
                {
                    x = selectedCoordinates[i].x;
                    y = selectedCoordinates[i].y;
                    index = i;
                }
            }

            orderedIndex.Add(index);
        }

        for(int i = 0 ; i < orderedIndex.Count ; i++)
            orderedSelectedHexGUIs.Add(Utility.guiManager.mapGUI.getHex(selectedCoordinates[orderedIndex[i]]));

        if(!isClockwise)
            orderedSelectedHexGUIs.Reverse();

        HexData thirdHexData = orderedSelectedHexGUIs[2].getHexData();
        orderedSelectedHexGUIs[2].ChangeHexa(orderedSelectedHexGUIs[1].getHexData());
        orderedSelectedHexGUIs[1].ChangeHexa(orderedSelectedHexGUIs[0].getHexData());
        orderedSelectedHexGUIs[0].ChangeHexa(thirdHexData);
    }
    private List<Vector2> CheckGroup()
    {
        // çift altüst olanlarda [0, 1], [1, 0], [1, 1] veya [1, 1], [0, 0], [0, 1]
        // tek  altüst olanlarda [0, 0], [0, 1], [1, 0] veya [0, 0], [1, 0], [1, 1]

        List<Vector2> groupList = new List<Vector2>(); 
        for(int i = 0 ; i < Utility.guiManager.mapGUI.gridX ; i++)
            for (int j = 0; j < Utility.guiManager.mapGUI.gridY - 1; j++)
            {
                Vector2 firstCoord = new Vector2(i, j);
                Vector2 secondCoord = new Vector2(i, j + 1);
                Color color1 = Utility.guiManager.mapGUI.getHex(firstCoord).getColor();
                Color color2 = Utility.guiManager.mapGUI.getHex(secondCoord).getColor();
                if (color1 != color2)
                    continue;

                int yDiff = i % 2 == 0 ? 1 : 0;

                List<Vector2> thirdCoords = new List<Vector2>();
                if (i > 0)
                    thirdCoords.Add(new Vector2(i - 1, j + yDiff));
                if (i < Utility.guiManager.mapGUI.gridX - 1)
                    thirdCoords.Add(new Vector2(i + 1, j + yDiff));

                for(int k = 0 ; k < thirdCoords.Count ; k++)
                {
                    Color color3 = Utility.guiManager.mapGUI.getHex(thirdCoords[k]).getColor();
                    if (color1 == color3)
                    {
                        if (!groupList.Contains(firstCoord))
                            groupList.Add(firstCoord);
                        if (!groupList.Contains(secondCoord))
                            groupList.Add(secondCoord);
                        if (!groupList.Contains(thirdCoords[k]))
                            groupList.Add(thirdCoords[k]);
                    }
                }
            }

        return groupList;
    }



    // [2-2] komşuları: [1-2], [1,3], [2-1], [2-3], [3-2], [3-3]
    // [3-2] komşuları: [2-1], [2,2], [3-1], [3-3], [4-1], [4-2]
    // all neighbours
    private Vector2[] oddDiffs = { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, -1), new Vector2(-1, -1) };
    private Vector2[] evenDiffs = { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1), new Vector2(-1, 1), new Vector2(1, 1) };
    private List<Vector2> FindClosestNeigbours(Vector2 position, Vector2 point)
    {
        Vector2 coordinate = Utility.guiManager.mapGUI.getCoordinate(position);

        Vector2[] usedDiffs;
        if (coordinate.x % 2 == 0)
            usedDiffs = evenDiffs;
        else
            usedDiffs = oddDiffs;

        float[] distanceDiffs = new float[usedDiffs.Length];

        Vector2[] finalDiffs = new Vector2[usedDiffs.Length];
        for (int i = 0; i < usedDiffs.Length; i++)
        {
            finalDiffs[i] = coordinate + usedDiffs[i];
            distanceDiffs[i] = Vector2.Distance(point, Utility.guiManager.mapGUI.getPosition(finalDiffs[i]));
        }
        // in order to eliminate three vertical hexa-group
        // [0, -1], [0, 1]
        if (coordinate.x == 0 || coordinate.x == Utility.guiManager.mapGUI.gridX - 1)
        {
            if (distanceDiffs[2] < distanceDiffs[3])
                distanceDiffs[3] = bigNumber;
            else
                distanceDiffs[2] = bigNumber;
        }
        // in order to eliminate three horizontal hexa-group
        // [-1, 0], [1, 0]
        if (coordinate.y == 0 || coordinate.y == Utility.guiManager.mapGUI.gridY - 1)
        {
            if (distanceDiffs[0] < distanceDiffs[1])
                distanceDiffs[1] = bigNumber;
            else
                distanceDiffs[0] = bigNumber;
        }

        List<Vector2> selectedCoordinates = new List<Vector2>();
        selectedCoordinates.Add(coordinate);
        for(int count = 1 ; count <= 2 ; count++)
        {
            int index = 0;
            float min = distanceDiffs[index];
            for (int i = 1; i < distanceDiffs.Length; i++)
                if(distanceDiffs[i] < min)
                {
                    index = i;
                    min = distanceDiffs[i];
                }

            selectedCoordinates.Add(finalDiffs[index]);
            distanceDiffs[index] = bigNumber;
        }

        return selectedCoordinates;
    }
    private void ResetPrevPaintedLines()
    {
        for (int i = 0; i < selectedCoordinates.Count; i++)
            Utility.guiManager.mapGUI.getHex(selectedCoordinates[i]).hexCanvasGUI.ResetLines();
    }
    private void PaintLine(List<Vector2> selectedCoordinates)
    {
        for (int i = 0; i < selectedCoordinates.Count; i++)
        {
            List<Direction> directions = new List<Direction>();
            for (int j = 0; j < selectedCoordinates.Count; j++)
            {
                if (i == j)
                    continue;

                Vector2 diff = selectedCoordinates[i] - selectedCoordinates[j];
                if (diff.x == -1)
                {
                    if (diff.y == -1)
                        directions.Add(Direction.TopRight);
                    else if (diff.y == 0)
                        directions.Add(Direction.Right);
                    else
                        directions.Add(Direction.BottomRight);
                }
                else if (diff.x == 0)
                {
                    if (diff.y == -1)
                        directions.Add(Direction.Top);
                    else
                        directions.Add(Direction.Bottom);
                }
                else if (diff.x == 1)
                {
                    if (diff.y == -1)
                        directions.Add(Direction.TopLeft);
                    else if (diff.y == 0)
                        directions.Add(Direction.Left);
                    else
                        directions.Add(Direction.BottomLeft);
                }
            }

            List<HexaDirection> hexaDirections = new List<HexaDirection>();
            if (selectedCoordinates[i].x % 2 == 1)
            {
                if (directions[0] == Direction.Left && directions[1] == Direction.Top || directions[1] == Direction.Left && directions[0] == Direction.Top)
                {
                    hexaDirections.Add(HexaDirection.TopLeft);
                    hexaDirections.Add(HexaDirection.Top);
                }
                else if (directions[0] == Direction.Top && directions[1] == Direction.Right || directions[1] == Direction.Top && directions[0] == Direction.Right)
                {
                    hexaDirections.Add(HexaDirection.Top);
                    hexaDirections.Add(HexaDirection.TopRight);
                }
                else if (directions[0] == Direction.Right && directions[1] == Direction.BottomRight || directions[1] == Direction.Right && directions[0] == Direction.BottomRight)
                {
                    hexaDirections.Add(HexaDirection.TopRight);
                    hexaDirections.Add(HexaDirection.BottomRight);
                }
                else if (directions[0] == Direction.BottomRight && directions[1] == Direction.Bottom || directions[1] == Direction.BottomRight && directions[0] == Direction.Bottom)
                {
                    hexaDirections.Add(HexaDirection.BottomRight);
                    hexaDirections.Add(HexaDirection.Bottom);
                }
                else if (directions[0] == Direction.Bottom && directions[1] == Direction.BottomLeft || directions[1] == Direction.Bottom && directions[0] == Direction.BottomLeft)
                {
                    hexaDirections.Add(HexaDirection.Bottom);
                    hexaDirections.Add(HexaDirection.BottomLeft);
                }
                else if (directions[0] == Direction.BottomLeft && directions[1] == Direction.Left || directions[1] == Direction.BottomLeft && directions[0] == Direction.Left)
                {
                    hexaDirections.Add(HexaDirection.BottomLeft);
                    hexaDirections.Add(HexaDirection.TopLeft);
                }
            }
            else
            {
                if (directions[0] == Direction.TopLeft && directions[1] == Direction.Top || directions[1] == Direction.TopLeft && directions[0] == Direction.Top)
                {
                    hexaDirections.Add(HexaDirection.TopLeft);
                    hexaDirections.Add(HexaDirection.Top);
                }
                else if (directions[0] == Direction.Top && directions[1] == Direction.TopRight || directions[1] == Direction.Top && directions[0] == Direction.TopRight)
                {
                    hexaDirections.Add(HexaDirection.Top);
                    hexaDirections.Add(HexaDirection.TopRight);
                }
                else if (directions[0] == Direction.TopRight && directions[1] == Direction.Right || directions[1] == Direction.TopRight && directions[0] == Direction.Right)
                {
                    hexaDirections.Add(HexaDirection.TopRight);
                    hexaDirections.Add(HexaDirection.BottomRight);
                }
                else if (directions[0] == Direction.Right && directions[1] == Direction.Bottom || directions[1] == Direction.Right && directions[0] == Direction.Bottom)
                {
                    hexaDirections.Add(HexaDirection.BottomRight);
                    hexaDirections.Add(HexaDirection.Bottom);
                }
                else if (directions[0] == Direction.Bottom && directions[1] == Direction.Left || directions[1] == Direction.Bottom && directions[0] == Direction.Left)
                {
                    hexaDirections.Add(HexaDirection.Bottom);
                    hexaDirections.Add(HexaDirection.BottomLeft);
                }
                else if (directions[0] == Direction.Left && directions[1] == Direction.TopLeft || directions[1] == Direction.Left && directions[0] == Direction.TopLeft)
                {
                    hexaDirections.Add(HexaDirection.BottomLeft);
                    hexaDirections.Add(HexaDirection.TopLeft);
                }
            }

            Utility.guiManager.mapGUI.getHex(selectedCoordinates[i]).hexCanvasGUI.SetLines(hexaDirections);
        }
    }

}
