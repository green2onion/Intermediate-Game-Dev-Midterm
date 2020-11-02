using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

	public int turnLimit;
	public int levelHPMax;
	int levelHPTemp;
	public Slider levelHP;

	int turn = 0;
	public GameObject[] players;
	public GameObject[] enemies;
	List<GameObject> activeEnemies;
	public GameObject building;
	public List<GameObject> buildings;
	public Text turnsText;
	public bool turnOver = true;

	public Canvas youWin;
	public Canvas youLose;
	int playerDeaths = 0;
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
	(int x, int y) GetAvailableTilePosition()
	{
		List<GameObject> availableTiles = new List<GameObject>();
		foreach (GameObject tile in grid)
		{
			if ((tile.GetComponentInChildren<CharacterMovement>() == null) && (tile.GetComponentInChildren<EnemyAI>() == null) && (tile.GetComponentInChildren<Building>() == null))
			{
				availableTiles.Add(tile);
			}
		}
		GameObject targetTile = availableTiles[Random.Range(0, availableTiles.Count)];
		(int x, int y) position = targetTile.GetComponent<Tile>().position;
		return position;
	}
	void SpawnPlayer()
	{

		for (int i = 0; i < players.Length; i++)
		{
			(int x, int y) position = GetAvailableTilePosition();
			players[i] = Instantiate(players[i], grid[position.x, position.y].transform);
			players[i].transform.localPosition = new Vector3(0, 0.4f, 0);
			players[i].GetComponent<CharacterMovement>().position = (position.x, position.y);
		}

	}
	void SpawnBuildings()
	{
		for (int i = 0; i < 4; i++)
		{
			(int x, int y) position = GetAvailableTilePosition();
			GameObject newBuilding = Instantiate(building, grid[position.x, position.y].transform);
			newBuilding.transform.localPosition = new Vector3(0, 0.4f, 0);
			buildings.Add(newBuilding);
			newBuilding.GetComponent<Building>().position = position;
		}
	}
	void SpawnEnemies()
	{
		for (int i = 0; i < enemies.Length; i++)
		{
			(int x, int y) position = GetAvailableTilePosition();
			GameObject enemy = Instantiate(enemies[i], grid[position.x, position.y].transform);
			enemy.transform.localPosition = new Vector3(0, 0.4f, 0);
			enemy.GetComponent<EnemyAI>().position = position;
			enemy.GetComponent<EnemyAI>().gameManager = this;
		}
	}
	public void NextTurn()
	{
		if (turn < 10)
		{
			if (turnOver)
			{
				turnOver = false;
				turn++;
				turnsText.text = "Turns Left: " + (turnLimit - turn).ToString();
				foreach (GameObject g in players)
				{
					g.GetComponent<CharacterMovement>().TurnReset();
				}
				activeEnemies.Clear();
				SpawnEnemies();
				foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
				{
					activeEnemies.Add(enemy);
				}
				activeEnemies[0].GetComponent<EnemyAI>().TurnStart();
			}

		}
		else
		{
			youWin.gameObject.SetActive(true);
		}


	}

	public void Restart()
	{
		SceneManager.LoadScene(0);
	}
	public void NextEnemy()
	{
		if (activeEnemies.Count > 1)
		{
			activeEnemies.RemoveAt(0);
			activeEnemies[0].GetComponent<EnemyAI>().TurnStart();
		}
		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			if (enemy.GetComponent<EnemyAI>().turnFinished)
			{
				turnOver = true;
			}
			else
			{
				turnOver = false;
				break;
			}
		}


	}

	public void SelectFromMenu(string type)
	{
		foreach (GameObject g in players)
		{
			if (g.GetComponent<CharacterMovement>().type == type)
			{
				g.GetComponent<CharacterMovement>().Select();
			}
		}
	}

	public void TakeDamage(int damage)
	{
		levelHPTemp -= damage;
		levelHP.value = levelHPTemp;
		if (levelHPTemp <= 0)
		{
			youLose.gameObject.SetActive(true);
		}
	}
	public void PlayerDeath()
	{
		if (playerDeaths >= 3)
		{
			youLose.gameObject.SetActive(true);
		}
	}
	// Start is called before the first frame update
	void Awake()
	{
		gameManager = this;
		SpawnGrid();

	}
	void Start()
	{
		activeEnemies = new List<GameObject>();
		buildings = new List<GameObject>();
		SpawnBuildings();
		SpawnPlayer();
		NextTurn();
		levelHPTemp = levelHPMax;
		levelHP.maxValue = levelHPMax;
		levelHP.value = levelHPTemp;
	}
	// Update is called once per frame
	void Update()
	{

	}
}
