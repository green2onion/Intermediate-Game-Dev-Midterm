using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[,] grid;
    public GameObject tile;
    public int gridRow;
    public int gridCol;
    public GameObject player;
    void SpawnGrid()
    {
        grid = new GameObject[gridRow, gridCol];
        for (int x = 0; x < gridRow; x++)
        {
            for (int y = 0; y < gridCol; y++)
            {
                grid[x, y] = Instantiate(tile, new Vector3((x - y) * 1, (x + y) * 0.7f), new Quaternion());
                grid[x, y].GetComponentInChildren<TextMesh>().text = x.ToString() + "," + y.ToString();
                /* screen.x = (map.x - map.y) * TILE_WIDTH_HALF;
                   screen.y = (map.x + map.y) * TILE_HEIGHT_HALF;
                */
            }
        }
    }
    void SpawnPlayer()
    {
        player = Instantiate(player, grid[1, 1].transform);
    }
    // Start is called before the first frame update
    void Awake()
    {
        SpawnGrid();
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
