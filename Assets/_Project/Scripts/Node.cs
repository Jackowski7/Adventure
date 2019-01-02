using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
	int lastGeneratedRow;
	public int distanceFromWall = 0;
	public int xVal;
	public bool leftSide;

	public bool Safe = true;

	NavMeshModifier navMeshModifier;

	private void Awake()
	{
		navMeshModifier = GetComponent<NavMeshModifier>();
	}

	public void SpawnEnemy()
	{
		GameObject enemy = Instantiate(GameManager.enemyPref, transform.position, Quaternion.Euler(Vector3.zero), transform);
	}

	public void SpawnTree()
	{
		Renderer rend = GetComponentInChildren<Renderer>();
		rend.material.color = Color.green;
		navMeshModifier.area = 1;
	}

	public void SpawnRock()
	{
		Renderer rend = GetComponentInChildren<Renderer>();
		rend.material.color = Color.grey;
		navMeshModifier.area = 1;
	}

	public void SpawnHole()
	{
		LayerMask nodes = LayerMask.GetMask("Node");
		Collider[] hits = Physics.OverlapSphere(transform.position, 6f, nodes);

		bool leftSideClear = false;
		if (Random.Range(0, 11) > 5)
		{
			leftSideClear = true;
		}

		foreach (Collider hit in hits)
		{
			Node hitNode = hit.GetComponent<Node>();
			int distance = hitNode.distanceFromWall;
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
				}
			}
		}

		Destroy(gameObject);


		Collider[] _hits = Physics.OverlapSphere(transform.position, 7f, nodes);
		foreach (Collider hit in _hits)
		{
			Node hitNode = hit.GetComponent<Node>();
			hitNode.Safe = false;
		}

	}


	public void SpawnWall()
	{
		Vector3 targetScale = transform.localScale;
		targetScale.y = 2;
		transform.localScale = targetScale;

		Renderer rend = GetComponent<Renderer>();
		rend.material.color = Color.black;
		navMeshModifier.area = 1;
	}

}
