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


	// Start is called before the first frame update
	void Awake()
	{
		enemyPref = _enemyPref;
		worldGenerator = _worldGenerator;
		player = Instantiate(_player);
		mainCamera = _MainCamera;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void CloseGame()
	{
		Application.Quit();
	}
}
