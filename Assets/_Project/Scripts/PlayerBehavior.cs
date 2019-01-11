using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using InControl;

public class PlayerBehavior : MonoBehaviour
{
	MyCharacterActions actions;

	public Vector3 cameraOffset;
	GameObject compass;
	public GameObject _targetReticle;
	public static GameObject targetReticle;
	public GameObject _potentialTargetReticle;
	public static GameObject potentialTargetReticle;

	public ParticleSystem snow;


	public GameObject _targetDirectionReticle;
	public static GameObject targetDirectionReticle;
	//public GameObject _potentialTargetDirectionReticle;
	//public static GameObject potentialTargetDirectionReticle;


	public int HP = 10;

	GameObject target;
	GameObject potentialTarget;
	public float targetRadius;

	[Space(20)]
	[Header("Player Stats")]
	//movement variables
	public float speed = 1f;


	//control settings
	float verticalDeadzone = .1f;
	float horizontalDeadzone = .1f;


	//score type things
	int distanceReached = 0;
	int enemiesKilled = 0;


	// Start is called before the first frame update
	void Start()
	{
		compass = transform.Find("Compass").gameObject;
		targetReticle = Instantiate(_targetReticle);
		potentialTargetReticle = Instantiate(_potentialTargetReticle);

		snow.Stop();

		targetDirectionReticle = Instantiate(_targetDirectionReticle, transform);
		//potentialTargetDirectionReticle = Instantiate(_potentialTargetDirectionReticle, transform);

		actions = new MyCharacterActions();
		actions.Attack.AddDefaultBinding(InputControlType.RightTrigger);
		actions.Attack.AddDefaultBinding(InputControlType.Action1);
		actions.Attack.AddDefaultBinding(Key.Pad1);

		actions.Target.AddDefaultBinding(Key.Pad2);
		actions.Target.AddDefaultBinding(InputControlType.Action2);

		actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
		actions.Left.AddDefaultBinding(Key.LeftArrow);
		actions.Right.AddDefaultBinding(Key.RightArrow);
		actions.Up.AddDefaultBinding(Key.UpArrow);
		actions.Down.AddDefaultBinding(Key.DownArrow);
		actions.Left.AddDefaultBinding(Key.A);
		actions.Right.AddDefaultBinding(Key.D);
		actions.Up.AddDefaultBinding(Key.W);
		actions.Down.AddDefaultBinding(Key.S);
	}

	Vector3 returnPoint;
	Vector3 moveDir;
	float lookVal;
	Vector3 lookDirection;
	Quaternion targetRotation;
	Vector3 cameraTarget;

	bool disableMouse = false;
	bool cantMove = false;
	bool fallTimeout = false;

	private void FixedUpdate()
	{
		if (!cantMove)
		{
			float LXInput = actions.Move.Value.x;
			float LYInput = actions.Move.Value.y;
			moveDir = new Vector3(LXInput, 0, LYInput);
			transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
		}
	}

	void Update()
	{
		cameraTarget = transform.position + cameraOffset;
		GameManager.mainCamera.transform.position = Vector3.Slerp(GameManager.mainCamera.transform.position, cameraTarget, .05f);

		distanceReached = (int)transform.position.y;

		InputDevice device = InputManager.ActiveDevice;

		if (InputManager.AnyKeyIsPressed)
		{
			disableMouse = false;
			TouchManager.ControlsEnabled = false;
		}

		if (InputManager.ActiveDevice.RightTrigger)
		{
			TouchManager.ControlsEnabled = false;
		}

		float lookX = Mathf.Abs(actions.Move.X);
		float lookY = Mathf.Abs(actions.Move.Y);
		lookVal = lookX + lookY;

		FindTargets();

		if (target != null) // if target, rotate towards target
		{
			Vector3 targetPoint = target.transform.position;
			targetPoint.y = 0;
			Vector3 fromPos = transform.position;
			fromPos.y = 0;
			targetRotation = Quaternion.LookRotation(targetPoint - fromPos);
		}
		else if (lookVal > .15f) // if no target, and we're moving, look where we're moving
		{
			lookDirection = moveDir;
			lookDirection.y = 0;
			targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15 * Time.deltaTime);

		//get last safe tile to respawn on
		LayerMask nodeLayer = LayerMask.GetMask("Node");
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, nodeLayer))
		{
			Node node = hit.transform.GetComponentInParent<Node>();
			if (node.Safe)
			{
				returnPoint = node.transform.position;
			}
		}

		if (actions.Attack.WasPressed)
		{
			if (!attacking)
			{
				attacking = true;
				Attack();
				comboTime = Time.time;
				comboBroken = false;
				combo = 0;
			}
			else if (Time.time - comboTime < 1 / (attackSpeed * 1.8))
			{
				comboBroken = true;
			}
			else if (Time.time - comboTime > 1 / (attackSpeed * 1.8) && Time.time - comboTime < 1 / attackSpeed)
			{
				if (!comboBroken)
				{
					combo++;
					if (Attack(combo) || combo < 2)
					{
						comboTime = Time.time;
					}
					else
					{
						comboBroken = true;
					}
				}
			}

		}

		if (attacking)
		{
			if (Time.time - comboTime > 1 / attackSpeed)
			{

				attacking = false;
				Debug.Log(combo);
				combo = 0;

			}
		}

		if (transform.position.y < -1 && !fallTimeout)
		{
			StartCoroutine(FallOff());
		}

	}

	float attackTime = 0;
	float comboTime = 0;
	bool comboBroken = false;
	int combo = 0;

	//attack variables
	public float attackSpeed = 1;
	public float _attackDamage = 4;
	public float _knockback = 1;
	public Vector2 _attackRadius = new Vector3(.4f, .5f);

	bool attacking = false;

	bool Attack(int combo = 0)
	{
		bool hitSomething = false;

		Quaternion playerRotation = transform.rotation;
		Vector3 playerPos = transform.position;
		Vector3 playerDirection = transform.forward;

		Vector3 attackOrgin = playerPos + playerDirection;
		Vector3 attackRadius = new Vector3(_attackRadius.x, .5f, _attackRadius.y) * (1 + (Mathf.Min(3, combo) * .1f));

		float attackDamage = _attackDamage * (1 + (combo * .1f));
		float knockback = _knockback * (1 + (Mathf.Min(8, combo) * .1f));

		if (combo > 0 && (combo + 1) % 3 == 0)
		{
			attackOrgin = playerPos;
			attackRadius = Vector3.one * 1.5f;
			knockback *= 1.5f;
			attackDamage *= .8f;
		}

		GetComponent<AttackMarker>().Enable(combo, attackRadius * 2, attackOrgin, .2f, attackSpeed);

		LayerMask targetLayer = LayerMask.GetMask("Enemy", "Destructable");
		Collider[] hits = Physics.OverlapBox(attackOrgin, attackRadius, playerRotation, targetLayer, QueryTriggerInteraction.Collide);
		foreach (Collider hit in hits)
		{
			if (hit.tag == "TargetPoint")
			{
				hitSomething = true;
				hit.transform.parent.transform.GetComponent<Health>().Damage(attackDamage);
				if (hit.transform.parent.transform.tag == "Enemy")
				{
					hit.transform.parent.transform.GetComponent<Enemy>().Hit(playerPos, knockback);
				}
			}
		}

		return hitSomething;
	}

	IEnumerator FallOff()
	{
		GetHit(1);
		fallTimeout = true;
		cantMove = true;
		yield return new WaitForSeconds(.45f);

		returnPoint.y = 0f;

		float timeout = 10;
		while (timeout > 0)
		{
			transform.position = Vector3.Slerp(transform.position, returnPoint, .02f);
			timeout--;
			yield return new WaitForEndOfFrame();
		}

		transform.position = returnPoint;

		yield return new WaitForSeconds(.25f);

		fallTimeout = false;
		cantMove = false;
	}


	public void GetHit(int damage)
	{
		HP -= damage;
		GameManager.UI.GetComponent<UI>().UpdateHP(HP);
	}

	void FindTargets()
	{
		GameObject mostDirectEnemy = null;
		GameObject closestEnemy = null;
		float closestDifference = 1000;
		float closestDistance = 1000;
		LayerMask enemyLayer = LayerMask.GetMask("Enemy");
		Collider[] enemies = Physics.OverlapSphere(transform.position, targetRadius, enemyLayer, QueryTriggerInteraction.Collide);
		foreach (Collider enemy in enemies)
		{
			if (enemy.tag == "TargetPoint")
			{
				compass.transform.LookAt(enemy.transform);
				float compassRot = compass.transform.rotation.eulerAngles.y;
				float playerRot = 0;

				if (lookVal > .1f)
				{

					if (moveDir != Vector3.zero)
					{
						playerRot = Quaternion.LookRotation(moveDir).eulerAngles.y;
					}

					float difference = Mathf.Abs(compassRot - playerRot);

					if (difference < 60)
					{
						if (difference < closestDifference)
						{
							mostDirectEnemy = enemy.gameObject;
							closestDifference = difference;
						}
					}
				}

				float distance = (enemy.transform.position - transform.position).magnitude;
				if (distance < closestDistance)
				{
					closestEnemy = enemy.gameObject;
					closestDistance = distance;
				}
			}
		}

		if (target != null)
		{
			float targetDistance = (transform.position - target.transform.position).magnitude;
			if (targetDistance > targetRadius)
			{
				if (potentialTarget != null)
				{
					SetTarget(potentialTarget);
				}
				else
				{
					SetTarget(null);
				}
			}
		}

		if (actions.Target.WasPressed)
		{
			if (target == null)
			{
				if (mostDirectEnemy != null)
				{
					SetTarget(mostDirectEnemy);
				}
				else if (closestEnemy != null)
				{
					SetTarget(closestEnemy);
				}
				else
				{
					SetTarget(null);
				}
			}
			else
			{
				if (potentialTarget != null)
					SetTarget(potentialTarget);
			}
		}

		if (mostDirectEnemy != null)
		{
			if (target != mostDirectEnemy)
			{
				SetPotentialTarget(mostDirectEnemy);
			}
		}
		else if (closestEnemy != null)
		{
			if (target != closestEnemy)
			{
				SetPotentialTarget(closestEnemy);
			}
		}
		else
		{
			SetPotentialTarget(null);
		}

		if (target == null)
		{
			if (potentialTarget != null)
			{
				SetTarget(potentialTarget);
			}
			else
			{
				SetTarget(null);
			}
		}
		else
		{
			targetReticle.transform.position = target.transform.position;
		}

		if (potentialTarget == null)
		{
			SetPotentialTarget(null);
		}
		else
		{
			if (potentialTarget == target)
			{
				SetPotentialTarget(null);
			}
			else
			{

				float potentialTargetDistance = (potentialTargetReticle.transform.position - potentialTarget.transform.position).magnitude;
				if (potentialTargetDistance > .3f && potentialTargetDistance < 4f)
				{
					potentialTargetReticle.transform.position = Vector3.Slerp(potentialTargetReticle.transform.position, potentialTarget.transform.position, .2f);
				}
				else
				{
					potentialTargetReticle.transform.position = potentialTarget.transform.position;
				}
			}
		}
	}

	void SetTarget(GameObject _target)
	{
		target = _target;

		if (target != null)
		{
			targetReticle.transform.position = target.transform.position;
			targetReticle.SetActive(true);
			targetDirectionReticle.SetActive(true);
		}
		else
		{
			targetReticle.SetActive(false);
			targetDirectionReticle.SetActive(false);
		}
	}

	void SetPotentialTarget(GameObject _target)
	{
		if (potentialTarget == null && _target != null)
		{
			potentialTargetReticle.transform.position = _target.transform.position;
		}

		potentialTarget = _target;

		if (potentialTarget != null)
		{
			potentialTargetReticle.SetActive(true);
		}
		else
		{
			potentialTargetReticle.SetActive(false);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Winter")
		{
			snow.Play();
		}
		if (collision.transform.tag == "Forest")
		{
			snow.Stop();
		}
		if (collision.transform.tag == "Desert")
		{
			snow.Stop();
		}
	}



}



public class MyCharacterActions : PlayerActionSet
{
	public PlayerAction Attack;
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerAction Up;
	public PlayerAction Down;
	public PlayerTwoAxisAction Move;

	public PlayerAction Target;

	public MyCharacterActions()
	{
		Attack = CreatePlayerAction("Attack");
		Target = CreatePlayerAction("Target");

		Left = CreatePlayerAction("Move Left");
		Right = CreatePlayerAction("Move Right");
		Up = CreatePlayerAction("Move Up");
		Down = CreatePlayerAction("Move Down");

		Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
	}
}


/*
 		float RXInput = actions.Aim.Value.x;
		float RYInput = actions.Aim.Value.y;
		Vector3 lookDirection = new Vector3(RXInput, 0, RYInput);

		if (lookDirection != Vector3.zero)
		{
			disableMouse = true;
		}

		if (!disableMouse)
		{
			Plane playerPlane = new Plane(Vector3.up, transform.position);
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			float hitdist = 0.0f;
			if (playerPlane.Raycast(ray, out hitdist))
			{
				Vector3 targetPoint = ray.GetPoint(hitdist);
				targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
			}
		}

		if (lookDirection != Vector3.zero)
		{
			targetRotation = Quaternion.LookRotation(lookDirection);
		}
*/

