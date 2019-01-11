using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Dummy, Walker, Hopper };

public class GameManager : MonoBehaviour
{

	public WorldGenerator _worldGenerator;
	public static WorldGenerator worldGenerator;

	public GameObject _enemyPref;
	public static GameObject enemyPref;

	public GameObject _player;
	public static GameObject player;

	public GameObject _MainCamera;
	public static GameObject mainCamera;

	public GameObject _UI;
	public static GameObject UI;


	public int level;


	// Start is called before the first frame update
	void Awake()
	{
		enemyPref = _enemyPref;
		worldGenerator = _worldGenerator;
		player = GameObject.Find("Player");
		mainCamera = _MainCamera;
		UI = _UI;
	}

	// Update is called once per frame
	void Update()
	{
		level = worldGenerator.zPos / 90;
	}

	public void CloseGame()
	{
		Application.Quit();
	}
}
