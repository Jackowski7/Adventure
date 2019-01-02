using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{

	public int zPos;

	void Update()
	{
		if ((GameManager.worldGenerator.zPos - zPos) > 55)
		{
			Destroy(gameObject);
		}
	}
}
