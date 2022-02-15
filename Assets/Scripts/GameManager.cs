using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance;

    public GameObject tilePrefab;
    public int minShuffleNumber = 0;

    public int[] b = {1,2,3,4,-1,6,7,8,9};
    int[] solution = {6,7,8,4,-1,5,1,2,3};

    [Header("TIMER")]
    public TextMeshProUGUI timerText;
    public float completionTime;
    float timer = 180;
    bool isTimerActive = false;
    public List<Tile> tiles = new List<Tile>();
    public Dictionary<int, Tile> tilesByID = new Dictionary<int, Tile>();
    public Vector3 emptySlot;

    [Header("ICONS")]
    public List<Sprite> AndroidIcons = new List<Sprite>();
    public List<Sprite> IOSIcons = new List<Sprite>();

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //Instantiate tiles
        InstantiateTiles();

        tiles.AddRange(GetComponentsInChildren<Tile>());

        //Shuffle magic square
        ShuffleTiles();
        UpdateBoard();

        //Start timer
        timer = completionTime;
        isTimerActive = true;

        //Allow drag and drop
        DragController.Instance.isDragAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Timer
        if(isTimerActive)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                timer = 0f;
                isTimerActive = false;
                GameOver();
            }
            DisplayTimer();
        }
    }

    void InstantiateTiles()
    {
        Vector3 tileSize = tilePrefab.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size;
        for(int i = 0; i < 8; i++)
        {
            Instantiate(tilePrefab, transform.position , Quaternion.identity).transform.SetParent(this.transform);
        }
    }

    void InitializeTiles()
    {
        int tileIndex = 0;
        for(int i = 0; i < b.Length; i++)
        {
            if(b[i] != -1)
            {
                tiles[tileIndex].SetIcon(AndroidIcons[b[i] - 1]);
                tiles[tileIndex].tileID = b[i] - 1;
                tilesByID.Add(b[i] - 1, tiles[tileIndex]);
                tiles[tileIndex].transform.position = new Vector3(i % 3, 0, (int)(i / 3));
                tileIndex++;
            }
        }
    }

    void ShuffleTiles(int shuffleCount = -1)
    {
        if(shuffleCount == -1)
        {
            for(shuffleCount = 1; shuffleCount <= minShuffleNumber; shuffleCount++)
            {
                ExchangeRandomTiles();
            }
        }
        else
        {
            ExchangeRandomTiles();
            shuffleCount++;
        }

        CheckSolvability(shuffleCount);
    }

    void ExchangeRandomTiles()
    {
        int rdm1 = Random.Range(0, b.Length);
        int rdm2 = Random.Range(0, b.Length);
        while(rdm1 == rdm2)
        {
            rdm2 = Random.Range(0, b.Length);
        }

        ExchangeTiles(rdm1, rdm2);
    }

    void ExchangeTiles(int x, int y)
    {
        int temp = b[x];
        b[x] = b[y];
        b[y] = temp;
    }

    void CheckSolvability(int shuffleCount)
    {
        for(int i = 0; i < b.Length; i++)
        {
            if(b[i] == -1)
            {
                if(i % 2 == shuffleCount % 2)
                {
                    InitializeTiles();
                    return;
                }
                else
                {
                    ShuffleTiles(shuffleCount);
                    return;
                }
            }
        }
    }

    public void UpdateBoard(int tileID = -1)
    {
        if(tileID != -1)
        {
            //Perform movement
            ExchangeTiles(GetTilePosFromID(tileID), GetTilePosFromID(-1));
        }

        //Check if the mystic square is complete
        if(CheckForVictory())
        {
            Victory();
        }

        int emptyIndex = -1;
        for(int i = 0; i < b.Length; i++)
        {
            if(b[i] != -1)
            {
                tilesByID[b[i] - 1].GetComponent<Draggable>().canBeDragged = false;
            }
            else
            {
                emptyIndex = i;
            }
        }

        //Update emptySlot position
        emptySlot = new Vector3(emptyIndex % 3, 0, (int)(emptyIndex / 3));

        //Update draggability on cordinal points
        if(emptyIndex - 1 >= 0 && emptyIndex != 3)
        {
            tilesByID[b[emptyIndex - 1] - 1].GetComponent<Draggable>().canBeDragged = true;
        }
        if(emptyIndex + 1 < b.Length && emptyIndex != 5)
        {
            tilesByID[b[emptyIndex + 1] - 1].GetComponent<Draggable>().canBeDragged = true;
        }
        if(emptyIndex - 3 >= 0)
        {
            tilesByID[b[emptyIndex - 3] - 1].GetComponent<Draggable>().canBeDragged = true;
        }
        if(emptyIndex + 3 < b.Length)
        {
            tilesByID[b[emptyIndex + 3] - 1].GetComponent<Draggable>().canBeDragged = true;
        }
    }

    int GetTilePosFromID(int tileID)
    {
        for(int i = 0; i < b.Length; i++)
        {
            if(b[i] - 1 == tileID || (tileID == -1 && b[i] == -1))
            {
                return i;
            }
        }
        return -1;
    }

    void DisplayTimer()
    {
        int m = (int)(timer / 60);
        int s = (int)(timer % 60);
        timerText.text = s < 10 ? m + ":0" + s : m + ":" + s;
    }

    bool CheckForVictory()
    {
        for(int i = 0; i < b.Length; i++)
        {
            if(b[i] != solution[i])
            {
                return false;
            }
        }
        return true;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        DragController.Instance.isDragAllowed = false;
    }

    void Victory()
    {
        isTimerActive = false;
        DragController.Instance.isDragAllowed = false;
        Debug.Log("VICTORY!");
    }
}
