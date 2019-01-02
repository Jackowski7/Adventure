using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMarker : MonoBehaviour
{

	float activeTime;
	float activeDuration;
	bool active = false;

	public GameObject attackMarker;
	public GameObject comboMarker;
	Renderer attackMarkerRend;

	private void Start()
	{
		attackMarkerRend = attackMarker.GetComponent<Renderer>();
	}

	// Update is called once per frame
	void Update()
	{
		if (active)
		{
			if (Time.time - activeTime > activeDuration)
			{
				active = false;
				attackMarkerRend.enabled = false;
			}
		}
	}

	IEnumerator ComboWindow(float activeTime, float attackSpeed)
	{
		while (Time.time - activeTime < 1 / (attackSpeed * 1.8))
		{
			yield return new WaitForEndOfFrame();
		}

		comboMarker.SetActive(true);
		while (Time.time - activeTime > 1 / (attackSpeed * 1.8) && Time.time - activeTime < 1 / attackSpeed)
		{
			yield return new WaitForEndOfFrame();
		}
		comboMarker.SetActive(false);
	}

	public void Enable(int combo, Vector3 size, Vector3 pos, float duration, float attackSpeed)
	{
		activeTime = Time.time;
		activeDuration = duration;
		active = true;

		StartCoroutine(ComboWindow(activeTime, attackSpeed));

		attackMarker.transform.position = pos;
		attackMarkerRend.enabled = true;
		attackMarker.transform.localScale = size;

		if (combo > 0)
		{
			attackMarkerRend.material.color = Color.yellow;
		}
		else
		{
			attackMarkerRend.material.color = Color.white;
		}
	}
}
