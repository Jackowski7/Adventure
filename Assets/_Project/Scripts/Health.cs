using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public float HP;
	public float maxHP;
	public GameObject HPBar;


	// Start is called before the first frame update
	void Start()
	{
		maxHP = HP;
	}

	// Update is called once per frame
	void Update()
	{
		if (HP > 0)
		{
			float healthPercent = (HP / maxHP);
			Vector3 healthBarScale = new Vector3(healthPercent, 1, 1);
			HPBar.transform.localScale = healthBarScale;
			HPBar.transform.LookAt(GameManager.mainCamera.transform);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Damage(float attackDamage)
	{
		HP -= attackDamage;
		StartCoroutine(DamageAnimation());
	}

	IEnumerator DamageAnimation()
	{
		Renderer rend = GetComponentInChildren<Renderer>();
		Color ogColor = rend.material.color;
		rend.material.color = Color.yellow;

		Renderer _rend = GetComponent<Renderer>();
		if (_rend != null)
		{
			Color _ogColor = _rend.material.color;
			_rend.material.color = Color.yellow;
		}

		yield return new WaitForSeconds(.1f);

		rend.material.color = ogColor;

		if (_rend != null)
			_rend.material.color = ogColor;

	}
}
