using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	public string type;
	public int attackRange;
	public int HP;
	public int damage;
	public bool ignoreObstacle;

	public (int x, int y) position;

	bool isAttachedToPointer = false;
	public int moveLimit;
	GameManager gameManager;
	int moveLimitTemp;
	bool canAttack;



	public void TurnReset()
	{
		moveLimitTemp = moveLimit;
		canAttack = true;
	}

	// moving
	void OnMouseDown() // movement
	{
		Select();
	}
	public void Select()
	{
		if ((moveLimitTemp > 0)||(canAttack))
		{
			if (!isAttachedToPointer) // start moving
			{
				isAttachedToPointer = true;
				HighlightTiles(GetEmptyTiles(), new Color(0.75f, 1f, 0.5f));
				HighlightTiles(GetEnemyTilesInAttackRange(), new Color(1, 0f, 0f));

			}
			else
			{
				if (GetEmptyTiles().Contains(GetNearestTile())) // end moving
				{
					isAttachedToPointer = false;
					transform.parent = GetNearestTile().transform;
					transform.localPosition = new Vector3(0, 0.4f, 0);
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
	List<GameObject> GetEmptyTiles()
	{
		List<GameObject> availableTiles = new List<GameObject>();
		foreach (GameObject tile in gameManager.grid)
		{
			if (Math.Abs(position.x - tile.GetComponent<Tile>().position.x) + Math.Abs(position.y - tile.GetComponent<Tile>().position.y) <= moveLimitTemp)
			{
				if ((tile.GetComponentInChildren<CharacterMovement>() == null) && (tile.GetComponentInChildren<EnemyAI>() == null) && (tile.GetComponentInChildren<Building>() == null))
				{
					availableTiles.Add(tile);
				}
			}
		}
		availableTiles.Add(transform.parent.gameObject);
		return availableTiles;
	}
	void HighlightTiles(List<GameObject> tiles, Color color)
	{
		foreach (GameObject tile in tiles)
		{
			tile.GetComponent<SpriteRenderer>().color = color;
		}
	}
	void UnHighlightTiles()
	{
		foreach (GameObject tile in gameManager.grid)
		{
			tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
		}
	}

	// attacking

	void Attack(GameObject target)
	{
		canAttack = false;
	}
	List<GameObject> GetEnemyTilesInAttackRange()
	{
		List<GameObject> enemyTiles = new List<GameObject>();
		foreach (GameObject tile in gameManager.grid)
		{
			Tile tileScript = tile.GetComponent<Tile>();
			// if target is horizontally within attackrange, (exclusive) or vertically within attackrange
			if (((Math.Abs(position.x - tileScript.position.x) <= attackRange) && (position.y == tileScript.position.y)) || ((Math.Abs(position.y - tileScript.position.y) <= attackRange) && (position.x == tileScript.position.x)))
			{
				// if target tile only has enemy
				if ((tile.GetComponentInChildren<CharacterMovement>() == null) && (tile.GetComponentInChildren<EnemyAI>() != null) && (tile.GetComponentInChildren<Building>() == null))
				{
					if (ignoreObstacle)
					{
						enemyTiles.Add(tile);
					}
					else // check if there is any obstacle between the player character and the enemy
					{
						bool hasObstacle = false;

						if (position.x == tileScript.position.x)
						{
							// horizontal right
							for (int y = position.y + 1; y < tileScript.position.y; y++)
							{
								// if the in-between tile has anything
								if (gameManager.grid[position.x, y].transform.childCount >= 1)
								{
									hasObstacle = true;
								}
							}
							// horizontal left
							for (int y = position.y - 1; y > tileScript.position.y; y--)
							{
								// if the in-between tile has anything
								if (gameManager.grid[position.x, y].transform.childCount >= 1)
								{
									hasObstacle = true;
								}
							}

						}
						else if (position.y == tileScript.position.y)
						{
							// vertical up
							for (int x = position.x + 1; x < tileScript.position.x; x++)
							{
								// if the in-between tile has anything
								if (gameManager.grid[x, position.y].transform.childCount >= 1)
								{
									hasObstacle = true;
								}
							}
							// vertical down
							for (int x = position.x - 1; x > tileScript.position.x; x--)
							{
								// if the in-between tile has anything
								if (gameManager.grid[x, position.y].transform.childCount >= 1)
								{
									hasObstacle = true;
								}
							}
						}
						if (!hasObstacle)
						{
							enemyTiles.Add(tile);
						}
					}

				}
			}
		}
		return enemyTiles;
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
