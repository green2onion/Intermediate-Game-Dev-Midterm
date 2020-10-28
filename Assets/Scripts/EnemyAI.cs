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
	public int damage;
	bool isMoving = false;
	bool reachedtileAdjacentToTarget = false;
	public bool turnFinished = false;
	GameObject tileAdjacentToTarget;
	GameObject target;
	public void TurnStart()
	{
		List<GameObject> availableTiles = GetAvailableTiles();
		target = FindNearestTarget(GetNearbyBuildingTiles(), GetNearbyCharsTiles());
		//Debug.Log("my target is " + target.GetComponent<Tile>().position.ToString());
		tileAdjacentToTarget = FindNearestTileAdjacentToTarget(availableTiles);
		//Debug.Log("my destination is " + tileAdjacentToTarget.GetComponent<Tile>().position.ToString());
		isMoving = true;
		reachedtileAdjacentToTarget = false;
		turnFinished = false;
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
		if (target.GetComponentInChildren<Building>()!=null)
		{
			gameManager.TakeDamage(damage);
			Debug.Log("attacked" + target.name);
		}
		if (target.GetComponentInChildren<CharacterMovement>() != null)
		{
			target.GetComponentInChildren<CharacterMovement>().TakeDamage(damage);
			Debug.Log("attacked" + target.name);
		}
	}

	void GoToTarget(GameObject target)
	{

		float interpolation = speed * Time.deltaTime;
		Vector3 position = transform.position;
		position.x = Mathf.Lerp(transform.position.x, target.transform.position.x, interpolation);
		position.y = Mathf.Lerp(transform.position.y, target.transform.position.y + 0.4f, interpolation);
		transform.position = position;
		transform.parent = target.transform;
		this.position = GetComponentInParent<Tile>().position;

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
	GameObject FindNearestTileAdjacentToTarget(List<GameObject> availableTiles)
	{
		List<GameObject> adjacentTiles = new List<GameObject>();
		GameObject nearestTarget = null;
		float minDist = Mathf.Infinity;
		foreach (GameObject tile in availableTiles)
		{
			if (tile.transform.childCount <= 1 || tile == transform.parent.gameObject)
			{
				float dist = Math.Abs(target.GetComponent<Tile>().position.x - tile.GetComponent<Tile>().position.x) + Math.Abs(target.GetComponent<Tile>().position.y - tile.GetComponent<Tile>().position.y);
				if (dist < minDist)
				{
					nearestTarget = tile;
					minDist = dist;
				}
			}

		}

		return nearestTarget;
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
		//Debug.Log("nearest tile position: " + nearestTile.GetComponent<Tile>().position);
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
	List<GameObject> GetNearbyCharsTiles()
	{
		List<GameObject> nearbyCharTiles = new List<GameObject>();
		foreach (GameObject player in gameManager.players)
		{
			//if (Math.Abs(position.x - player.GetComponent<CharacterMovement>().position.x) + Math.Abs(position.y - player.GetComponent<CharacterMovement>().position.y) <= moveLimit)
			//{
				nearbyCharTiles.Add(player.transform.parent.gameObject);
			//}
		}

		return nearbyCharTiles;
	}
	List<GameObject> GetNearbyBuildingTiles()
	{
		List<GameObject> nearbyBuildings = new List<GameObject>();
		foreach (GameObject building in gameManager.buildings)
		{
			//if (Math.Abs(position.x - building.GetComponent<Building>().position.x) + Math.Abs(position.y - building.GetComponent<Building>().position.y) <= moveLimit)
			//{
				nearbyBuildings.Add(building.transform.parent.gameObject);
			//}
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
			//Debug.Log("target tile position: " + target.GetComponent<Tile>().position);
			if (GetNearestTile().GetComponent<Tile>().position == tileAdjacentToTarget.GetComponent<Tile>().position)
			{
				isMoving = false;
				transform.localPosition = new Vector3(0, 0.4f, 0);
				//Debug.Log("arrived at " + target.GetComponent<Tile>().position);
				reachedtileAdjacentToTarget = true;
			}
		}
		else if (reachedtileAdjacentToTarget && !turnFinished)
		{
			Attack(target);
			gameManager.NextEnemy();
			turnFinished = true;
		}

	}

}
