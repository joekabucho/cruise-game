using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopManager : MonoSingleton<ShopManager>
{
	private const int AVATAR_COUNT = 25;
	private const int AMM_PER_ROW = 3;
	private int selectedIndex = 0;

	public Material avatarMaterial;
	public Transform avatarContainer;
	public Text currencyText;

	public override void Init ()
	{
		currencyText.text = AppManager.instance.currency.ToString();
		selectedIndex = AppManager.instance.prefAvatar;
		int r = 0,
			c = 0,
			l = AppManager.instance.maxLevel,
			i = 0;

		foreach(Transform t in avatarContainer)
		{
			foreach(Transform b in t)
			{
				i = (r * 3 + c);
				i = 1 << i;

				if(r == 0)
				{
					if((AppManager.instance.unlockFlag & i) != i)
						b.GetChild(0).gameObject.SetActive(true);
				}
				else if(r/2 > l) // Locked
				{
					if(r == 10 && AppManager.instance.highscore >= 500)
					{
						if((AppManager.instance.unlockFlag & i) != i)
							b.GetChild(0).gameObject.SetActive(true); 
					}
					else
					{
						b.GetChild(1).gameObject.SetActive(true);
						b.GetComponent<Button>().interactable = false;
					}
				}
				else if((AppManager.instance.unlockFlag & i) != i) // Not bought
				{
					b.GetChild(0).gameObject.SetActive(true); 
				}
				c++;
			}
			r++;
			c = 0;
		}
	}

	public void OnAvatar(int i)
	{

		if((AppManager.instance.unlockFlag & 1 << i) != 1 << i)
		{
			GameObject go = avatarContainer.GetChild((int)i / 3).GetChild(i % 3).GetChild(0).gameObject;
			int cost;

			if(int.TryParse(go.GetComponentInChildren<Text>().text,out cost))
			{
				if(AppManager.instance.currency >= cost)
				{
					AppManager.instance.currency -= cost;
					go.SetActive(false);
					AppManager.instance.unlockFlag |= 1 << i;
					currencyText.text = AppManager.instance.currency.ToString();
				}
				else
				{
					// Play feedback
					return;
				}
			}
			else
			{
				return;
			}
		}

		avatarMaterial.SetTextureOffset("_MainTex",
			new Vector2((i%8) * 0.125f,0.875f - ((int)(i/8) * 0.125f)));

		selectedIndex = i;

		AppManager.instance.prefAvatar = i;
		AppManager.instance.SaveData("meta",AppManager.instance.FormatMetaSaveString());
	}

	public void ToMenu()
	{
		AppManager.instance.ShopToMenu();
	}
}
