using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	public int moveLimit;
	public (int x, int y) position;
	public int HP;
	public GameManager gameManager;
	public float speed;
	bool isMoving = false;
	bool reachedtileAdjacentToTarget = false;
	public bool turnFinished = false;
	GameObject tileAdjacentToTarget;
	GameObject target;
	public void TurnStart()
	{
		List<GameObject> availableTiles = GetAvailableTiles();
		target = FindNearestTarget(GetNearbyBuildingTiles(), GetNearbyCharsTiles());
		tileAdjacentToTarget = FindNearestTileAdjacentToTarget(FindTilesAdjacentToTarget(target, availableTiles));
		isMoving = true;

	}
	public void TakingDamage(int damage)
	{
		HP -= damage;
		if (HP <= 0)
		{
			Death();
		}
	}
	void Death()
	{
		Destroy(gameObject);
	}
	void Attack(GameObject target)
	{

	}

	bool GoToTarget(GameObject target)
	{
		Debug.Log("my destination is " + target.GetComponent<Tile>().position.ToString());
		if (transform.position.x != target.transform.position.x)
		{
			MoveX(target);
			return false;
		}
		else if (transform.position.y != target.transform.position.y)
		{
			MoveY(target);
			return false;
		}
		else
		{
			transform.parent = GetNearestTile().transform;
			transform.localPosition = new Vector3(0, 0.4f, 0);
			position = GetComponentInParent<Tile>().position;
			return true;
		}

	}
	void MoveX(GameObject target)
	{
		Debug.Log("is moving X");
		float interpolation = speed * Time.deltaTime;
		Vector3 position = transform.position;
		position.x = Mathf.Lerp(transform.position.x, target.transform.position.x, interpolation);
		position.y = Mathf.Lerp(transform.position.y, target.transform.position.y, interpolation);
		transform.position = position;
	}
	void MoveY(GameObject target)
	{
		Debug.Log("is moving Y");
		float interpolation = speed * Time.deltaTime;
		Vector3 position = transform.position;
		position.x = Mathf.Lerp(transform.position.x, target.transform.position.x, interpolation);
		position.y = Mathf.Lerp(transform.position.y, target.transform.position.y, interpolation);
		transform.position = position;
	}

	GameObject FindNearestTarget(List<GameObject> buildings, List<GameObject> players)
	{
		GameObject nearestTarget = null;
		float minDist = Mathf.Infinity;
		List<GameObject> targets = new List<GameObject>();
		targets.AddRange(buildings);
		targets.AddRange(players);
		foreach (GameObject tile in targets)
		{
			float dist = Math.Abs(position.x - tile.GetComponent<Tile>().position.x) + Math.Abs(position.y - tile.GetComponent<Tile>().position.y);
			if (dist < minDist)
			{
				nearestTarget = tile;
				minDist = dist;
			}
		}

		return nearestTarget;
	}
	List<GameObject> FindTilesAdjacentToTarget(GameObject target, List<GameObject> availableTiles)
	{
		List<GameObject> adjacentTiles = new List<GameObject>();
		foreach (GameObject tile in availableTiles)
		{
			if (Math.Abs(target.GetComponent<Tile>().position.x - tile.GetComponent<Tile>().position.x) + Math.Abs(target.GetComponent<Tile>().position.y - tile.GetComponent<Tile>().position.y) <= 1) // nearby within 1 tile
			{
				adjacentTiles.Add(tile);
			}
		}

		return adjacentTiles;
	}
	GameObject FindNearestTileAdjacentToTarget(List<GameObject> adjacentTiles)
	{
		GameObject nearestTarget = null;
		float minDist = Mathf.Infinity;
		foreach (GameObject tile in adjacentTiles)
		{
			float dist = Math.Abs(position.x - tile.GetComponent<Tile>().position.x) + Math.Abs(position.y - tile.GetComponent<Tile>().position.y);
			if (dist < minDist)
			{
				nearestTarget = tile;
				minDist = dist;
			}
		}

		return nearestTarget;
	}
	List<GameObject> GetAvailableTiles()
	{
		List<GameObject> availableTiles = new List<GameObject>();
		foreach (GameObject tile in gameManager.grid)
		{
			if (Math.Abs(position.x - tile.GetComponent<Tile>().position.x) + Math.Abs(position.y - tile.GetComponent<Tile>().position.y) <= moveLimit)
			{

				availableTiles.Add(tile);

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
	List<GameObject> GetNearbyBuildingTiles()
	{
		List<GameObject> nearbyBuildings = new List<GameObject>();
		foreach (GameObject tile in GetAvailableTiles())
		{
			if (tile.GetComponentsInChildren<Building>() != null)
			{
				nearbyBuildings.Add(tile);
			}
		}

		return nearbyBuildings;
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
		if (isMoving)
		{

				GoToTarget(tileAdjacentToTarget);
				//isMoving = false;
				

			}
		}
	/*
		else if (reachedtileAdjacentToTarget)
		{
			Attack(target);
			turnFinished = true;
		}
	}
	*/
}
