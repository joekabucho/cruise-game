using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AppManager : MonoSingleton<AppManager>
{
	public const float AD_INTERVAL = 60.0f;

	public float lastAd = -30.0f;
	private float startTime = 0.0f;
	public int currency = 0;
	public int highscore = 0;
	public int ammGamePlayed = 0;
	public int prefAvatar = 0;
	public int maxLevel = 0;
	public int unlockFlag = 9;

	public Material avatarMaterial;

	public override void Init ()
	{
		LoadPrefs();
	//	if(Advertisement.isSupported)
	//	{
	//		Advertisement.Initialize("62145",false);
	//	}
		DontDestroyOnLoad(gameObject);
		Application.LoadLevel("Menu");
	}

	// AmmGamePlayed + Highscore + Currency + PrefAvatar + MaxLevel + UnlockFlag
	private void LoadPrefs()
	{
		string s = PlayerPrefs.GetString("meta");
		if(s == "")
		{
			unlockFlag = 9;
			SaveData("meta",FormatMetaSaveString());
			return;
		}

		string[] vData = s.Split('|');
		int.TryParse(vData[0],out ammGamePlayed);
		int.TryParse(vData[1],out highscore);
		int.TryParse(vData[2],out currency);
		int.TryParse(vData[3],out prefAvatar);
		int.TryParse(vData[4],out maxLevel);
		int.TryParse(vData[5],out unlockFlag);

		avatarMaterial.SetTextureOffset("_MainTex",new Vector2((prefAvatar%8) * 0.125f,0.875f - ((int)(prefAvatar/8) * 0.125f)));
	}

	public void StartGame()
	{
		ammGamePlayed++;
		SaveData("meta",FormatMetaSaveString());
		Application.LoadLevel("Game");
		startTime = Time.time;
	}

	public string FormatMetaSaveString()
	{
		return	 ammGamePlayed.ToString() +
				'|' + highscore.ToString() +
				'|' + currency.ToString() +
				'|' + prefAvatar.ToString() +
				'|' + maxLevel.ToString() +
				'|' + unlockFlag.ToString();
	}

	public void OpenShop()
	{
		Application.LoadLevel("Shop");
	}

	public void GameToMenu()
	{
		Application.LoadLevel("Menu");
	}

	public void ShopToMenu()
	{
		Application.LoadLevel("Menu");
	}

	public float GetGameDuration()
	{
		return Time.time - startTime;
	}

	public void SaveData(string k,string v)
	{
		PlayerPrefs.SetString(k,v);
	}
}
