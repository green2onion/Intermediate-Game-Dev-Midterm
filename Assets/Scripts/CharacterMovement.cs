using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public (int x, int y) position;
    bool isAttachedToPointer = false;
    public int moveLimit;
    GameManager gameManager;
    int moveLimitTemp;
    public void TurnReset()
    {
        moveLimitTemp = moveLimit;
    }

    void OnMouseDown() // movement
    {
        if (moveLimitTemp > 0)
        {
            if (!isAttachedToPointer) // start moving
            {
                isAttachedToPointer = true;
                HighlightAvailableTiles(GetAvailableTiles());
            }
            else
            {
                if (GetAvailableTiles().Contains(GetNearestTile())) // end moving
                {
                    isAttachedToPointer = false;
                    transform.parent = GetNearestTile().transform;
                    transform.localPosition = Vector3.zero;
                    moveLimitTemp -= Math.Abs(position.x - GetComponentInParent<Tile>().position.x) + Math.Abs(position.y - GetComponentInParent<Tile>().position.y);
                    position = GetComponentInParent<Tile>().position;
                    UnHighlightTiles();


                }

            }
        }

    }
    GameObject GetNearestTile()
    {
        GameObject nearestTile = null;
        float minDist = Mathf.Infinity;
        foreach (GameObject t in gameManager.grid)
        {
            float dist = Vector3.Distance(t.transform.position, transform.position);
            if (dist < minDist)
            {
                nearestTile = t;
                minDist = dist;
            }
        }
        return nearestTile;
    }
    List<GameObject> GetAvailableTiles()
    {
        List<GameObject> availableTiles = new List<GameObject>();
        foreach (GameObject tile in gameManager.grid)
        {
            if (Math.Abs(position.x - tile.GetComponent<Tile>().position.x) + Math.Abs(position.y - tile.GetComponent<Tile>().position.y) <= moveLimitTemp)
            {
                availableTiles.Add(tile);
            }
        }
        return availableTiles;
    }
    void HighlightAvailableTiles(List<GameObject> tiles)
    {
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<SpriteRenderer>().color = new Color(0.75f, 1f, 0.5f);
        }
    }
    void UnHighlightTiles()
    {
        foreach (GameObject tile in gameManager.grid)
        {
            tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttachedToPointer)
        {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        }
    }
}
