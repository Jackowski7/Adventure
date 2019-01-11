using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfRangeDeleter : MonoBehaviour
{
	public int zPos;

	void Update()
	{
		if ((GameManager.worldGenerator.zPos - zPos) > 45)
		{
			Destroy(gameObject);
		}
	}
}
