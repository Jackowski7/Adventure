using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{

    void Start()
    {

		int numChildren = transform.childCount;

		for (int x = 0; x < numChildren; x++)
		{
			GameObject child = transform.GetChild(x).gameObject;
			int rotateAmount = Random.Range(0, 5);
			child.transform.Rotate(new Vector3(0, rotateAmount * 60, 0));
		}
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
