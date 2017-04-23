using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject rock;
    public GameObject sand;
    public GameObject dirt;
    public GameObject grass;
    public GameObject tree;
    public GameObject artifact;

    public TextAsset[] levelFiles;
    public int levelIndex = 0;

    public char[,,] level;
    private GameObject[,,] tiles;
    private GameObject goal;

    void Start()
    {
        Camera.main.GetComponent<FlyAwayAnimator>().reverse = true;
        Camera.main.GetComponent<FlyAwayAnimator>().StartCoroutine("BeginAnimation", new System.Action(LevelReady));
        if (level == null || level.Length == 0)
        {
            SetupLevel();
        }
        if (tiles == null || tiles.Length == 0)
        {
            UpdateLevel();
        }
    }

    void SetupLevel()
    {
        string[] horizSlices = levelFiles[levelIndex].text.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        Debug.Log(horizSlices.Length.ToString() + " slices");
        string[] firstSliceSplit = horizSlices[0].Split('\n');
        Debug.Log(firstSliceSplit.Length.ToString() + " columns");
        level = new char[firstSliceSplit[0].Length, horizSlices.Length, firstSliceSplit.Length];
        Debug.Log(firstSliceSplit[0].Length.ToString() + " rows");
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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            DiggerCharacter chara = player.GetComponent<DiggerCharacter>();
            if (chara)
            {
                if (chara.prompt)
                {
                    Object.Destroy(chara.prompt);
                }
                if (levelIndex < chara.prompts.Length && chara.prompts[levelIndex] != null)
                {
                    chara.prompt = Object.Instantiate(chara.prompts[levelIndex], GameObject.FindObjectOfType<Canvas>().transform);
                }
                else
                {
                    chara.prompt = null;
                }
            }
        }
    }

    public void UpdateLevel()
    {
        transform.position = new Vector3();
        if (tiles != null)
        {
            foreach (GameObject tile in tiles)
            {
                Object.Destroy(tile);
            }
        }
        tiles = new GameObject[level.GetLength(0),level.GetLength(1),level.GetLength(2)];
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                for (int z = 0; z < level.GetLength(2); z++)
                {
                    GameObject template;
                    switch (level[x, y, z])
                    { // TODO optimize by looking at neighbors
                        case 'd':
                            template = dirt;
                            break;
                        case 'g':
                            template = grass;
                            break;
                        case 's':
                            template = sand;
                            break;
                        case 'r':
                            template = rock;
                            break;
                        case 't':
                            template = tree;
                            break;
                        case 'a':
                            template = artifact;
                            break;
                        default:
                            template = null;
                            break;
                    }
                    if (template != null)
                    {
                        tiles[x, y, z] = Object.Instantiate(template, transform);
                        tiles[x, y, z].transform.localPosition = new Vector3(x, y, z);
                        if (level[x,y,z] == 'a')
                        {
                            goal = tiles[x, y, z];
                        }
                    }
                }
            }
        }
        transform.position = new Vector3(level.GetLength(0) * -0.5f + 0.5f, 0, level.GetLength(2) * -0.5f + 0.5f);
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
        return SafeGet(x, y, z) != ' ' && SafeGet(x,y,z) != 'a' && SafeGet(x, y, z) != 'p';
    }

    public bool CanStandOn(int x, int y, int z)
    {
        return SafeGet(x, y, z) != ' ' && SafeGet(x, y, z) != 't';
    }

    public bool CanDig(int x, int y, int z)
    {
        char c = SafeGet(x, y, z);
        return c == 's' || c == 'd' || c == 'g';
    }

    public void DoLevelEnd()
    {
        Debug.Log("Level end");
        GetComponent<AudioSource>().Play();
        Object.Destroy(goal);
        Camera.main.GetComponent<FlyAwayAnimator>().reverse = false;
        Camera.main.GetComponent<FlyAwayAnimator>().StartCoroutine("BeginAnimation", new System.Action(LoadNextLevel));
    }

    public void LoadNextLevel()
    {
        Debug.Log("Loading next level");
        levelIndex++;
        if (levelIndex >= levelFiles.Length)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
        else
        {
            SetupLevel();
            UpdateLevel();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            DiggerCharacter chara = player.GetComponent<DiggerCharacter>();
            Camera.main.GetComponent<CameraController>().angle = -0.75f * Mathf.PI;
            player.transform.localPosition = chara.targetPosition = new Vector3(0, 1, 0);
            Camera.main.GetComponent<FlyAwayAnimator>().reverse = true;
            Camera.main.GetComponent<FlyAwayAnimator>().StartCoroutine("BeginAnimation", new System.Action(LevelReady));
        }
    }

    public void LevelReady()
    {
        Debug.Log("Level ready");
        GameObject.FindGameObjectWithTag("Player").GetComponent<DiggerCharacter>().enableInput = true;
    }
}
