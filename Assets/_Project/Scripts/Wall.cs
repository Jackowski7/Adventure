using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

	public GameObject _tree;
	public GameObject _rock;
	public bool leftSide = false;

	// Start is called before the first frame update
	void Start()
	{

		for (int x = 0; x < 6; x++)
		{
			GameObject _object;
			if (Random.Range(0, 100) > 20)
			{
				_object = _tree;
			}
			else
			{
				_object = _rock;
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

			GameObject child = Instantiate(_object, transform);
			child.transform.localPosition = pos;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
