using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public int moveLimit;
    public (int x, int y) position;
    GameManager gameManager;
    void Death()
    {
        Destroy(gameObject);
    }

    List<GameObject> GetAvailableTiles()
    {
        List<GameObject> availableTiles = new List<GameObject>();
        foreach (GameObject tile in gameManager.grid)
        {
            if (Math.Abs(position.x - tile.GetComponent<Tile>().position.x) + Math.Abs(position.y - tile.GetComponent<Tile>().position.y) <= moveLimit)
            {
                if ((tile.GetComponentInChildren<CharacterMovement>() == null) && (tile.GetComponentInChildren<EnemyAI>() == null))
                {
                    availableTiles.Add(tile);
                }

            }
        }
        return availableTiles;
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

    List<GameObject> GetNearbyCharsTiles()
    {
        List<GameObject> nearbyChars = new List<GameObject>();
        foreach (GameObject tile in GetAvailableTiles())
        {
            if (tile.GetComponentsInChildren<CharacterMovement>() != null)
            {
                nearbyChars.Add(tile);
            }
        }

        return nearbyChars;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManager;
        position = GetComponentInParent<Tile>().position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
