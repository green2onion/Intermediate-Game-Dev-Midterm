using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
	Slider HPSlider;
	public string type;
	public int attackRange;
	public int HP;
	int HPTemp;
	bool isDead = false;
	public int damage;
	public bool ignoreObstacle;

	public (int x, int y) position;

	bool isAttachedToPointer = false;
	public int moveLimit;
	GameManager gameManager;
	int moveLimitTemp;
	bool canAttack;

	public GameObject copy;
	GameObject copyTemp;

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
		if (!isDead)
		{
			if ((moveLimitTemp > 0) || (canAttack))
			{
				if (!isAttachedToPointer) // start moving
				{
					isAttachedToPointer = true;
					CreateCopy();
					HighlightTiles(GetEmptyTiles(), new Color(0.75f, 1f, 0.5f));
					if (canAttack)
					{
						HighlightTiles(GetEnemyTilesInAttackRange(), new Color(1, 0f, 0f));
					}


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
						Destroy(copyTemp);
					}
					else if (GetEnemyTilesInAttackRange().Contains(GetNearestTile()) && canAttack)
					{
						isAttachedToPointer = false;
						UnHighlightTiles();
						Attack(GetNearestTile().GetComponentInChildren<EnemyAI>());
						Destroy(copyTemp);

					}

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
			float dist = Vector3.Distance(t.transform.position, copyTemp.transform.position);
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

	void CreateCopy()
	{
		copyTemp = Instantiate(copy, transform.position, new Quaternion());
		copyTemp.transform.localScale = transform.localScale;
		copyTemp.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
		copyTemp.GetComponent<SpriteRenderer>().color = new Color(gameObject.GetComponent<SpriteRenderer>().color.r, gameObject.GetComponent<SpriteRenderer>().color.g, gameObject.GetComponent<SpriteRenderer>().color.b, 0.5f);
		copyTemp.GetComponent<Copy>().parent = this;
	}
	// attacking

	void Attack(EnemyAI target)
	{
		target.TakingDamage(damage);
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

						if (position.x == tileScript.position.x && tileScript.position.y > position.y)
						{
							bool hasObstacles = false;
							// horizontal right
							for (int y = position.y + 1; y < tileScript.position.y; y++)
							{
								// if the in-between tile has anything
								if (gameManager.grid[position.x, y].transform.childCount > 1)
								{
									Debug.Log("Obstacle at " + position.x + "," + y);
									hasObstacles = true;
								}
							}
							if (!hasObstacles)
							{
								enemyTiles.Add(tile);
							}
						}
						if (position.x == tileScript.position.x && tileScript.position.y < position.y)
						{
							bool hasObstacles = false;
							// horizontal left
							for (int y = position.y - 1; y > tileScript.position.y; y--)
							{
								// if the in-between tile has anything
								if (gameManager.grid[position.x, y].transform.childCount > 1)
								{
									Debug.Log("Obstacle at " + position.x + "," + y);
									hasObstacles = true;
								}
							}
							if (!hasObstacles)
							{
								enemyTiles.Add(tile);
							}
						}

						if (position.y == tileScript.position.y && tileScript.position.x > position.x)
						{
							bool hasObstacles = false;
							// vertical up
							for (int x = position.x + 1; x < tileScript.position.x; x++)
							{
								// if the in-between tile has anything
								if (gameManager.grid[x, position.y].transform.childCount > 1)
								{
									Debug.Log("Obstacle at " + x + "," + position.y);
									hasObstacles = true;
								}
							}
							if (!hasObstacles)
							{
								enemyTiles.Add(tile);
							}
						}
						if (position.y == tileScript.position.y && tileScript.position.x < position.x)
						{
							bool hasObstacles = false;
							// vertical down
							for (int x = position.x - 1; x > tileScript.position.x; x--)
							{
								// if the in-between tile has anything
								if (gameManager.grid[x, position.y].transform.childCount > 1)
								{
									Debug.Log("Obstacle at " + x + "," + position.y);
									hasObstacles = true;
								}
							}
							if (!hasObstacles)
							{
								enemyTiles.Add(tile);
							}
						}

					}

				}
			}
		}
		foreach (GameObject tile in enemyTiles)
		{
			Debug.Log(tile.GetComponent<Tile>().position);
		}

		return enemyTiles;
	}
	public void TakeDamage(int damage)
	{
		HPTemp -= damage;
		//Debug.Log(type + " HP is" + HPTemp.ToString());
		if (HPTemp <= 0)
		{
			isDead = true;
		}
	}
	// Start is called before the first frame update
	void Start()
	{
		gameManager = GameManager.gameManager;
		HPTemp = HP;
		HPSlider = GameObject.Find("HP Bar " + type).GetComponent<Slider>();
		HPSlider.maxValue = HP;
		HPSlider.value = HPTemp;
	}

	// Update is called once per frame
	void Update()
	{
		if (isAttachedToPointer && copyTemp != null)
		{
			copyTemp.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
		}
		HPSlider.value = HPTemp;
	}
}
