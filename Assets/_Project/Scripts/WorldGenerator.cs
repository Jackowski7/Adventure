using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldGenerator : MonoBehaviour
{

	GameObject player;

	public GameObject _node;

	public Transform worldFolder;
	public Transform water;
	public Transform baseGround;
	public Transform sun;

	public NavMeshSurface surface;

	[Space(10)]
	[Header("Map Settings")]

	public int rowWidth = 10;
	int previousRowWidth;

	[HideInInspector]
	public int zPos = 0;
	float zMod = .8f;
	float xMod;


	private void Start()
	{
		previousRowWidth = rowWidth;
		player = GameManager.player;
		player.transform.position = new Vector3(0, 2f, 14f);
	}
	void Update()
	{
		Vector3 playerPos = player.transform.position;

		if (((zPos * zMod) - playerPos.z) < 30)
		{
			GenerateRow();
		}

		if (((zPos * zMod) - playerPos.z) > 41)
		{
			player.transform.position = new Vector3(playerPos.x, playerPos.y, (zPos * zMod) - 41);
		}

		water.transform.position = new Vector3(xMod, 0, zPos * zMod - 22);
		water.transform.localScale = new Vector3(50, 1, 50);
		baseGround.transform.position = new Vector3(xMod, 0, zPos * zMod - 22);
		baseGround.transform.localScale = new Vector3(50, 1, 50);
		//sun.transform.position = playerPos;
	}

	public bool spawnEnemies;
	public int enemyPadding = 2;
	int lastEnemyRow = 0;

	public int treePadding = 2;
	int lastTreeRow = 0;

	public int rockPadding = 2;
	int lastRockRow = 0;

	public bool spawnHoles;
	public int holePadding = 5;
	int lastHoleRow = 0;

	void GenerateRow()
	{
		GameObject row = new GameObject("Row" + zPos);
		row.transform.parent = worldFolder.transform;
		Row rowScript = row.AddComponent<Row>();
		rowScript.zPos = zPos;

		// move map right if player 'enters' new row towards right
		if ((int)player.transform.position.x > xMod)
		{
			xMod += .5f;
		}
		else if ((int)player.transform.position.x < xMod)
		{
			xMod -= .5f;
		}
		else
		{
			if (Random.value == 1)
			{
				xMod -= .5f;
			}
			else
			{
				xMod += .5f;
			}

		}

		bool spawnEnemy = false;
		bool spawnTree = false;
		bool spawnRock = false;
		bool spawnHole = false;

		spawnEnemy = false;
		if (zPos - lastEnemyRow > enemyPadding && spawnEnemies)
		{
			if (Random.Range(0, 100) > 70)
			{
				spawnEnemy = true;
				lastEnemyRow = zPos;
			}
		}

		spawnTree = false;
		if (zPos - lastTreeRow > treePadding && !spawnEnemy)
		{
			if (Random.Range(0, 100) > 70)
			{
				spawnTree = true;
				lastTreeRow = zPos;
			}
		}

		spawnRock = false;
		if (zPos - lastRockRow > rockPadding && !spawnEnemy && !spawnTree)
		{
			if (Random.Range(0, 100) > 70)
			{

				spawnRock = true;
				lastRockRow = zPos;
			}
		}

		spawnHole = false;
		if (zPos - lastHoleRow > holePadding && !spawnEnemy && !spawnTree && !spawnRock && spawnHoles)
		{
			if (Random.Range(0, 100) > 70)
			{
				spawnHole = true;
				lastHoleRow = zPos;
			}
		}

		zPos++;
		StartCoroutine(PlaceTiles(row, zPos, xMod, spawnEnemy, spawnTree, spawnRock, spawnHole));
	}

	int navMeshCounter = 5;
	IEnumerator PlaceTiles(GameObject row, int _zPos, float xMod, bool _spawnEnemy, bool _spawnTree, bool _spawnRock, bool _spawnHole)
	{

		int thisRowWidth = previousRowWidth;

		if (rowWidth > previousRowWidth)
			thisRowWidth = previousRowWidth + 1;

		if (rowWidth < previousRowWidth)
			thisRowWidth = previousRowWidth - 1;


		int spawnX = Random.Range(2, thisRowWidth - 2);

		for (int x = -3; x <= thisRowWidth + 3; x++)
		{
			float zPos = _zPos * zMod;
			GameObject node = Instantiate(_node, new Vector3(((int)(x - thisRowWidth / 2) + xMod - 1), -.5f, zPos), Quaternion.Euler(new Vector3(0, 30, 0)), row.transform);
			Node nodeScript = node.GetComponent<Node>();

			int _x = x;
			if (_x > rowWidth / 2)
				_x--;

			nodeScript.distanceFromWall = Mathf.Abs(Mathf.Abs((rowWidth / 2) - _x) - (rowWidth / 2));
			nodeScript.xVal = x;

			if (x <= (int)(thisRowWidth / 2))
			{
				nodeScript.leftSide = true;
			}
			else
			{
				nodeScript.leftSide = false;
			}

			if (x < 0 || x > thisRowWidth)
			{
				nodeScript.SpawnWall();
			}
			else
			{
				if (_spawnEnemy && x == spawnX)
				{
					nodeScript.SpawnEnemy();
				}

				if (_spawnTree && x == spawnX)
				{
					nodeScript.SpawnTree();
				}

				if (_spawnRock && x == spawnX)
				{
					nodeScript.SpawnRock();
				}

				if (_spawnHole && x == spawnX)
				{
					nodeScript.SpawnHole();
				}
			}


		}
			yield return new WaitForEndOfFrame();

		navMeshCounter--;

		if (navMeshCounter == 0)
		{
			surface.BuildNavMesh();
			navMeshCounter = 5;
		}

		previousRowWidth = thisRowWidth;
	}



}

/*
 Get player
 for ungenrerated rows in front of player closer than x distance, generate rows of nodes.

	*/
