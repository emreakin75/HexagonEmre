using UnityEngine;

public class Utility : MonoBehaviour
{
    public static GUIManager guiManager;
    public static GameManager gameManager;

    public static GameObject InstantiatePrefab(GameObject prefab, Transform content)
    {
        GameObject go = Instantiate(prefab, Vector3.zero, new Quaternion(0, 0, 0, 0)) as GameObject;
        go.transform.SetParent(content, false);
        go.GetComponent<RectTransform>().localScale = Vector3.one;
        return go;
    }

    private static readonly System.Random random = new System.Random();
    private static readonly object syncLock = new object();
    public static int GetRandomNumber(int min, int max)
    {
        lock (syncLock)
        {
            return random.Next(min, max);
        }
    }

}
