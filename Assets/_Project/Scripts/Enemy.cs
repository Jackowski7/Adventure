using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

	NavMeshAgent agent;
	GameObject target;

	public EnemyType enemyType;
	float knockbackResist;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		if (enemyType == EnemyType.Dummy)
		{
			GetComponent<Health>().HP = 100;
			GetComponent<Health>().maxHP = 100;
			knockbackResist = .5f;

			agent.speed = 0f;
			agent.acceleration = 0;
			agent.angularSpeed = 0;
			agent.stoppingDistance = 10;
		}
		if (enemyType == EnemyType.Walker)
		{
			GetComponent<Health>().HP = 15;
			GetComponent<Health>().maxHP = 15;
			knockbackResist = .5f;

			agent.speed = 3.5f;
			agent.acceleration = 8;
			agent.angularSpeed = 360;
			agent.stoppingDistance = 1;
		}
		if (enemyType == EnemyType.Hopper)
		{
			GetComponent<Health>().HP = 10;
			GetComponent<Health>().maxHP = 10;
			knockbackResist = 0;

			agent.speed = 15;
			agent.acceleration = 500;
			agent.angularSpeed = 10;
			agent.stoppingDistance = 0;
		}
	}

	private void Update()
	{
		if (transform.position.y < 0)
			Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			target = other.gameObject;
			StartCoroutine(FollowPlayer());
		}
	}

	bool chargingHop;
	Vector3 hopTarget;

	public void Hit(Vector3 attackOrgin, float _knockback)
	{
		Vector3 knockback = ((transform.position - attackOrgin).normalized * (0 + Mathf.Max(0, (_knockback - knockbackResist))));
		StartCoroutine(Knockback(knockback));
	}

	IEnumerator Knockback(Vector3 _knockback)
	{
		yield return new WaitForEndOfFrame();

		float timer = 10;
		Vector3 target = transform.position + _knockback;
		while (timer > 0)
		{
			transform.position = Vector3.Slerp(transform.position, target, .3f);
			timer--;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator FollowPlayer()
	{
		while (target != null)
		{
			if (enemyType == EnemyType.Walker)
			{
				if (this.gameObject.activeSelf)
					agent.SetDestination(GameManager.player.transform.position - (GameManager.player.transform.position - transform.position).normalized * 2f);

				yield return new WaitForSeconds(.1f);
			}

			if (enemyType == EnemyType.Hopper)
			{
				hopTarget = GameManager.player.transform.position;
				StartCoroutine(PreHop());
				yield return new WaitForSeconds(.2f);
				chargingHop = false;

				if (this.gameObject.activeSelf)
				{
					agent.SetDestination(hopTarget);
				}

				yield return new WaitForSeconds(2f);
			}

			else
			{
				yield return new WaitForSeconds(.5f);
			}
		}
	}


	IEnumerator PreHop()
	{
		chargingHop = true;

		while (chargingHop)
		{
			Vector3 target = (hopTarget - transform.position).normalized;
			Quaternion targetRotation = Quaternion.LookRotation(target, Vector3.up);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .1f);

			yield return new WaitForEndOfFrame();
		}
	}

}
