using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    public GameObject[,] grid;
    public GameObject[] tile;
   
    int[,] layout =
    {
        {0,0,0,0,0,0 },
        {0,0,0,0,0,0 },
        {0,0,0,0,0,0 },
        {0,0,0,0,0,0 },
        {0,0,0,0,0,0 },
        {0,0,0,0,0,0 },
    };

    public int TurnLimit;
    public int HP;

    int turn = 0;
    public GameObject[] players;

    public Text turnsText;
    void SpawnGrid()
    {
        int gridRow = layout.GetLength(0);
        int gridCol = layout.GetLength(1);
        grid = new GameObject[gridRow, gridCol];
        for (int x = 0; x < gridRow; x++)
        {
            for (int y = 0; y < gridCol; y++)
            {
                grid[x, y] = Instantiate(tile[layout[x, y]], new Vector3((x - y) * 1, (x + y) * 0.7f), new Quaternion());
                
                /* screen.x = (map.x - map.y) * TILE_WIDTH_HALF;
                   screen.y = (map.x + map.y) * TILE_HEIGHT_HALF;
                */
                grid[x, y].GetComponent<Tile>().position = (x, y);
            }
        }
    }
    void SpawnPlayer()
    {
        for (int i = 0; i<players.Length;i++)
        {
            players[i] = Instantiate(players[i], grid[i, i].transform);
            players[i].GetComponent<CharacterMovement>().position = (i, i);
        }

    }

    public void NextTurn()
    {
        turn++;
        turnsText.text = "Turns Left: " + (TurnLimit - turn).ToString();
        foreach(GameObject g in players)
        {
            g.GetComponent<CharacterMovement>().TurnReset();
        }
        
    }
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = this;
        SpawnGrid();
        SpawnPlayer();
        NextTurn();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
