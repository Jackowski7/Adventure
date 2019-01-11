using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
	public WorldGenerator worldGenerator;

	public int lastGeneratedRow;
	public int distanceFromWall = 0;
	public int xVal;
	public bool leftSide;

	public bool Safe = true;

	public List<GameObject> DesertDecor = new List<GameObject> { };
	public GameObject _groundDesert;
	public GameObject _holeDesert;

	public List<GameObject> ForestDecor = new List<GameObject> { };
	public GameObject _groundForest;
	public GameObject _holeForest;

	public List<GameObject> WinterDecor = new List<GameObject> { };
	public GameObject _groundWinter;
	public GameObject _holeWinter;

	public GameObject _firefly;

	public bool isWater = false;

	public bool night = false;
	public int biomeCount;

	NavMeshModifier navMeshModifier;

	private void Awake()
	{
		navMeshModifier = GetComponent<NavMeshModifier>();
	}

	public void SpawnGround(int biome)
	{
		if (biome == 0) // forest
		{
			GameObject ground = Instantiate(_groundForest, transform.position, transform.rotation, transform);
			if (night)
			{
				if (Random.Range(0, 100) > 95)
				{
					GameObject firefly = Instantiate(_firefly, transform.position, transform.rotation, transform);
				}
			}
		}
		else if (biome == 1) // desert
		{
			GameObject ground = Instantiate(_groundDesert, transform.position, transform.rotation, transform);
		}
		else if (biome == 2) // winter
		{
			GameObject ground = Instantiate(_groundWinter, transform.position, transform.rotation, transform);
		}


	}

	public void SpawnEnemy(int biome)
	{
		GameObject enemy = Instantiate(GameManager.enemyPref, transform.position, transform.rotation, transform);
	}

	public void SpawnDecor(int biome)
	{
		GameObject _decor = null;

		if (biome == 0) // forest
		{
			_decor = ForestDecor[Random.Range(0, ForestDecor.Count)];
		}
		else if (biome == 1) // desert
		{
			_decor = DesertDecor[Random.Range(0, DesertDecor.Count)];
		}
		else if (biome == 2) // winter
		{
			_decor = WinterDecor[Random.Range(0, WinterDecor.Count)];
		}
		GameObject decor = Instantiate(_decor, transform.position, Quaternion.identity, transform);
	}

	public void SpawnHole(int biome)
	{
		StartCoroutine(_SpawnHole(biome));
	}

	IEnumerator _SpawnHole(int biome)
	{
		yield return new WaitForSeconds(.5f);

		GameObject hole = null;

		if (biome == 0) // forest
		{
			hole = Instantiate(_holeForest, transform.position, Quaternion.identity, transform.parent.parent.transform);
		}
		else if (biome == 1) // desert
		{
			hole = Instantiate(_holeDesert, transform.position, Quaternion.identity, transform.parent.parent.transform);
		}
		else if (biome == 2) // winter
		{
			hole = Instantiate(_holeWinter, transform.position, Quaternion.identity, transform.parent.parent.transform);
		}

		hole.GetComponent<OutOfRangeDeleter>().zPos = lastGeneratedRow + 20;

		int holeRange = Random.Range(2, 7);

		LayerMask nodes = LayerMask.GetMask("Node", "Wall");
		Collider[] hits = Physics.OverlapSphere(transform.position, holeRange, nodes);

		bool leftSideClear = false;
		if (!leftSide)
		{
			leftSideClear = true;
		}

		foreach (Collider hit in hits)
		{

			Node hitNode = hit.GetComponentInParent<Node>();
			bool makeHole = false;

			if (leftSideClear)
			{
				if (hitNode.leftSide == false)
				{
					makeHole = true;
				}
			}
			else
			{
				if (hitNode.leftSide == true)
				{
					makeHole = true;
				}
			}

			if (makeHole)
			{
				if (Random.Range(0, 100) > 15)
				{
					Destroy(hit.gameObject);
					//sometimes this destroys walls or something? weird horizontal holes in lakes
				}
			}

		}

		Destroy(gameObject);

	}


	public void SpawnWall(int zPos)
	{
		transform.rotation = Quaternion.Euler(Vector3.forward);

		for (int x = 0; x < 10; x++)
		{
			int biomeCount = (int)((zPos + Random.Range(0, 2)) / worldGenerator.biomeLength);
			int biome = worldGenerator.biomeArray[biomeCount];

			GameObject _decor = null;

			if (biome == 0) // forest
			{
				_decor = ForestDecor[Random.Range(0, ForestDecor.Count)];
			}
			else if (biome == 1) // desert
			{
				_decor = DesertDecor[Random.Range(0, DesertDecor.Count)];
			}
			else if (biome == 2) // winter
			{
				_decor = WinterDecor[Random.Range(0, DesertDecor.Count)];
			}

			Vector3 pos = Vector3.zero;
			if (leftSide)
			{
				pos.x = -x;
			}
			else
			{
				pos.x = x;
			}

			//pos.y += x * .2f;

			GameObject child = Instantiate(_decor, transform.position, Quaternion.identity, transform);
			child.transform.localPosition = pos;
		}

	}

}
