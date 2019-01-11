using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldGenerator : MonoBehaviour
{

	GameObject player;

	public GameObject _node;

	public Transform worldFolder;
	public Transform sun;

	public NavMeshSurface surface;

	[Space(10)]
	[Header("Map Settings")]

	public int rowWidth = 10;
	int initialRowWidth;
	int previousRowWidth;

	[HideInInspector]
	public int zPos = 0;
	float zMod = .865f;
	float xMod;
	Vector3 _sunTargetRot;

	public List<int> biomeArray = new List<int> { };
	public int biomeLength;

	private void Awake()
	{
		for (int x = 0; x < 100; x++)
		{
			biomeArray.Add(Random.Range(0, 3));
		}
	}

	private void Start()
	{
		initialRowWidth = rowWidth;
		previousRowWidth = rowWidth;
		player = GameManager.player;
		_sunTargetRot = sun.transform.rotation.eulerAngles;
	}
	void Update()
	{
		Vector3 playerPos = player.transform.position;

		if (((zPos * zMod) - playerPos.z) < 25)
		{
			GenerateRow();
			_sunTargetRot.x += 1f;
		}

		if (((zPos * zMod) - playerPos.z) > 38)
		{
			player.transform.position = new Vector3(playerPos.x, playerPos.y, (zPos * zMod) - 38);
		}

		Quaternion sunTargetRot = Quaternion.Euler(_sunTargetRot);
		sun.transform.rotation = Quaternion.Slerp(sun.transform.rotation, sunTargetRot, .1f);

	}

	public bool spawnEnemies;
	public int enemyPadding = 2;
	int lastEnemyRow = 10;

	public int decorPadding = 2;
	int lastDecorRow = 0;

	public bool spawnHoles;
	public int holePadding = 5;
	int lastHoleRow = 0;

	void GenerateRow()
	{
		GameObject row = new GameObject("Row" + zPos);
		row.transform.parent = worldFolder.transform;
		OutOfRangeDeleter rowScript = row.AddComponent<OutOfRangeDeleter>();
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
		bool spawnDecor = false;
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

		spawnDecor = false;
		if (zPos - lastDecorRow > decorPadding && !spawnEnemy)
		{
			if (Random.Range(0, 100) > 70)
			{
				spawnDecor = true;
				lastDecorRow = zPos;
			}
		}

		spawnHole = false;
		if (zPos - lastHoleRow > holePadding && !spawnEnemy && !spawnDecor && spawnHoles)
		{
			if (Random.Range(0, 100) > 70)
			{
				spawnHole = true;
				lastHoleRow = zPos;
			}
		}

		zPos++;
		StartCoroutine(PlaceTiles(row, zPos, xMod, spawnEnemy, spawnDecor, spawnHole));

	}

	int navMeshCounter = 5;
	IEnumerator PlaceTiles(GameObject row, int _zPos, float xMod, bool _spawnEnemy, bool _spawnDecor, bool _spawnHole)
	{
		yield return new WaitForEndOfFrame();

		int thisRowWidth = previousRowWidth;

		if (rowWidth > previousRowWidth)
			thisRowWidth = previousRowWidth + 1;

		if (rowWidth < previousRowWidth)
			thisRowWidth = previousRowWidth - 1;

		int spawnX = Random.Range(2, thisRowWidth - 2);

		if (_zPos == 20)
		{
			player.transform.position = new Vector3(rowWidth / 2 * xMod, 1.5f, (_zPos * zMod));
		}

		for (int x = -1; x <= thisRowWidth + 1; x++)
		{
			float zPos = _zPos * zMod;
			GameObject node = Instantiate(_node, new Vector3(((int)(x - thisRowWidth / 2) + xMod - 1), -.5f, zPos), Quaternion.Euler(new Vector3(0, 30, 0)), row.transform);
			Node nodeScript = node.GetComponentInChildren<Node>();
			nodeScript.worldGenerator = GetComponent<WorldGenerator>();
			nodeScript.lastGeneratedRow = _zPos;

			int biomeCount = (int)((zPos + Random.Range(0, 2)) / biomeLength);
			int biome = biomeArray[biomeCount];

			int _x = x;
			if (_x > rowWidth / 2)
				_x--;

			nodeScript.distanceFromWall = Mathf.Abs(Mathf.Abs((rowWidth / 2) - _x) - (rowWidth / 2));
			nodeScript.xVal = x;

			if ((zPos % 360) + Random.Range(0, 15) > 100 && (zPos % 360) < 280)
				nodeScript.night = true;

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
				nodeScript.SpawnWall((int)zPos);
			}
			else
			{
				if (_spawnEnemy && x == spawnX)
				{
					nodeScript.SpawnEnemy(biome);
				}

				if (_spawnDecor && x == spawnX)
				{
					nodeScript.SpawnDecor(biome);
				}
				else
				{
					nodeScript.SpawnGround(biome);
				}

				if (_spawnHole && x == spawnX)
				{
					nodeScript.SpawnHole(biome);
				}
			}
		}


		navMeshCounter--;

		if (navMeshCounter == 0)
		{
			surface.BuildNavMesh();
			navMeshCounter = 5;

			rowWidth = Random.Range(initialRowWidth - 5, initialRowWidth + 5);

		}

		previousRowWidth = thisRowWidth;
	}

}

/*
 Get player
 for ungenrerated rows in front of player closer than x distance, generate rows of nodes.

	*/
