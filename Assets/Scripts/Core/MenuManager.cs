using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoSingleton<MenuManager>
{
	public Text gamePlayed;
	public Text currency;
	public Text highscore;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
			return;
		}
	}

	public override void Init ()
	{
		gamePlayed.text = "Games played : " + AppManager.instance.ammGamePlayed.ToString();
		currency.text = "Crystals : " + AppManager.instance.currency.ToString();
		highscore.text = AppManager.instance.highscore.ToString();
	}

	public void OnButtonPlay()
	{
		AppManager.instance.StartGame();
	}

	public void OnButtonShop()
	{
		AppManager.instance.OpenShop();
	}
}
