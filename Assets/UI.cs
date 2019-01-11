using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	[SerializeField]
	GameManager gm;

	[SerializeField]
	WorldGenerator wg;

	PlayerBehavior player;

	public TMPro.TextMeshProUGUI Zpos;
	public TMPro.TextMeshProUGUI Score;
	public TMPro.TextMeshProUGUI Gold;

	public GameObject HP;
	List<Image> HpBlocks = new List<Image> { };

	// Start is called before the first frame update
	void Start()
    {
		player = gm._player.GetComponent<PlayerBehavior>();

		foreach (Transform child in HP.transform)
		{
			HpBlocks.Add(child.GetComponent<Image>());
		}
	}

    // Update is called once per frame
    void Update()
    {
		Zpos.text = "Level: " +(gm.level).ToString();
    }



	public void UpdateHP(int HP)
	{
		for (int x = 0; x < 10; x++)
		{
			if (HP > x)
			{
				HpBlocks[x].enabled = true;
			}
			else
			{
				HpBlocks[x].enabled = false;
			}
		}
	}
}
