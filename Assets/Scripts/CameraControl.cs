using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    GameManager gameManager;
    GameObject[,] grid;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        grid = gameManager.grid;
        transform.position = new Vector3(grid[grid.GetLength(0) / 2, grid.GetLength(0) / 2].transform.position.x, grid[grid.GetLength(0) / 2, grid.GetLength(0) / 2].transform.position.y, -10);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
