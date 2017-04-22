using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject blockTemplate;
    [TextArea(3, 10)]
    public string initialLevel;

    public char[,,] level;

    void Start()
    {
        if (level == null || level.Length == 0)
        {
            SetupLevel();
        }
        if (transform.childCount == 0)
        {
            UpdateLevel();
        }
    }

    void SetupLevel()
    {
        string[] horizSlices = initialLevel.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] firstSliceSplit = horizSlices[0].Split('\n');
        level = new char[horizSlices.Length, firstSliceSplit.Length, firstSliceSplit[0].Length];
        int x = 0, y = 0, z = 0;
        foreach (string slice in horizSlices)
        {
            x = 0;
            z = 0;
            foreach (char c in slice)
            {
                if (c == '\n')
                {
                    x = 0;
                    z++;
                }
                else
                {
                    if (x >= level.GetLength(0) || z >= level.GetLength(2))
                        continue;
                    level[x, y, z] = c;
                    x++;
                }
            }
            y++;
        }
    }

    void UpdateLevel()
    {
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                for (int z = 0; z < level.GetLength(2); z++)
                {
                    if (level[x, y, z] == 'x')
                    { // TODO optimize by looking at neighbors
                        Object.Instantiate(blockTemplate, new Vector3(x, y, z), new Quaternion(), transform);
                    }
                }
            }
        }
    }

    public char SafeGet(int x, int y, int z)
    {
        if (level == null || level.Length == 0)
        {
            SetupLevel();
        }
        if (x >= level.GetLength(0) || y >= level.GetLength(1) || z >= level.GetLength(2) || x < 0 || y < 0 || z < 0)
        {
            return ' ';
        }
        return level[x, y, z];
    }

    public bool Collides(int x, int y, int z)
    {
        return SafeGet(x, y, z) != ' ';
    }
}
