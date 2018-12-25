using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	CharacterController character;

	public float speed = 1;

	public float verticalDeadzone = .1f;
	public float horizontalDeadzone = .1f;

    // Start is called before the first frame update
    void Start()
    {
		character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") > verticalDeadzone)
		{
			character.SimpleMove(Vector3.forward * speed);
		}
		if (Input.GetAxis("Vertical") < -verticalDeadzone)
		{
			character.SimpleMove(-Vector3.forward * speed);
		}
		if (Input.GetAxis("Horizontal") > horizontalDeadzone)
		{
			character.SimpleMove(Vector3.right * speed);
		}
		if (Input.GetAxis("Horizontal") < -horizontalDeadzone)
		{
			character.SimpleMove(-Vector3.right * speed);
		}
	}
}
