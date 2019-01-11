using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;

public class GameManager : MonoSingleton<GameManager>
{
	private const float TILE_LENGHT = 7.0f;

	private AppManager app;
	private bool gameActive = true;
	private float gameDuration;
	private int score;
	private int levelUpDelta = 100;
	private int speedUpDelta = 50;
	private float lastScoreTick;
	private float deltaScore = 1.5f;
	private float levelSpeed = 9.0f;
	private float wallSpawn = 0f;
	private float coveredDistance = 0f;
	private float spawnPoint;
	private int ammWall;
	private int wallIndex = -1;
	private bool afterCenter = true;
	private bool isLeft = false;
	private bool isLerping = false;
	private bool isColorLerping = false;
	private bool bonusPoint = false;
	private Vector3 startPoint;
	private Vector3 endPoint;
	private float startTime;
	private float lerpDuration = 2.0f;
	private int currLevel = 0;
	private float textureOffset = 0.0f;
	private float startTimeColor = 0.0f;

	private Color startColor;
	private Color endColor;
	private Color startWallColor;
	private Color endWallColor;

	public PlayerAvatar player;
	public GameObject levelContainer;
	public List<Transform> wallList;
	public Material levelBG;
	public Material wallBG;
	public GameObject sideWall;
	public GameObject menu;
	public Text	currency;
	public Text scoreText;

	public GameObject scorePanel;
	public Text endScoreText;
	public Text highscoreText;

	public override void Init()
	{
		app = AppManager.instance;
		lastScoreTick = Time.time;
		wallList = new List<Transform>();

		foreach(Transform c in levelContainer.transform)
			wallList.Add(c);

		ammWall = wallList.Count;
		coveredDistance = ammWall * TILE_LENGHT;
		spawnPoint = coveredDistance;

		currency.text = "   x" + app.currency.ToString();
		scoreText.text = score.ToString();

		HideMenu();
	}

	private void Update()
	{
		if(!app || !gameActive)
			return;

		TimeScore();
		ScrollLevel();
		InitWall();

		if(isLerping)
		{
			sideWall.transform.position = Vector3.Lerp(startPoint,endPoint,(Time.time-startTime) / lerpDuration);
			if((Time.time-startTime) / lerpDuration > 1)
				isLerping = false;
		}

		if(isColorLerping)
		{
			float t = (Time.time - startTimeColor) / lerpDuration;
			levelBG.color = Color.Lerp(startColor,endColor,t);
			wallBG.color = Color.Lerp(startWallColor,endWallColor,t);
			if(t > 1)
				isColorLerping = false;
		}

		if(score >= levelUpDelta)
			LevelUp();
		if(score >= speedUpDelta)
			SpeedUp();
	}

	public void OnCollectPoint()
	{
		score++;
		AppManager.instance.currency++;
		currency.text = "   x" + app.currency.ToString();
		scoreText.text = score.ToString();
	}

	private void SpeedUp()
	{
		speedUpDelta += 50;
		levelSpeed += 0.10f;
		deltaScore -= 0.10f;

		if(deltaScore < 0.5f)
			deltaScore = 0.5f;

		isColorLerping = true;
		startTimeColor = Time.time;

		startColor = levelBG.color;
		endColor = new Color(Random.Range(0.5f,1f),Random.Range(0.5f,1f),Random.Range(0.5f,1f));
		startWallColor = wallBG.color;
		endWallColor = endColor - new Color(0.25f,0.25f,0.25f);
	}

	private void LevelUp()
	{
		if(currLevel >= 4)
			return;

		levelUpDelta += 100;
		player.OnLevelUp();
		currLevel++;
		SlideSideWalls();
	}

	private void TimeScore()
	{
		if(Time.time - lastScoreTick > deltaScore)
		{
			score++;
			lastScoreTick = Time.time;
			scoreText.text = score.ToString();
		}
	}

	private void ScrollLevel()
	{
		levelContainer.transform.position += (Vector3.down * levelSpeed) * Time.deltaTime;
	}

	private void InitWall()
	{
		float f = levelContainer.transform.position.y;

		if(f < (-wallSpawn) )
		{
			wallIndex = (wallIndex >= ammWall-1)?0:wallIndex+1;
			wallList[wallIndex].position = new Vector3(0,spawnPoint + (f % TILE_LENGHT));
			wallSpawn += TILE_LENGHT;
			foreach(Transform t in wallList[wallIndex])
				t.gameObject.SetActive(false);
			wallList[wallIndex].GetChild(RandomPatern()).gameObject.SetActive(true);
			if(bonusPoint)
			{
				if(Random.Range(0,2) == 1)
					return;
				GameObject go = wallList[wallIndex].GetChild(3).gameObject;
				go.SetActive(true);
				Vector3 bonus = new Vector3((!isLeft)?4.5f:-4.5f,0,0);
				go.transform.localPosition = bonus;
				bonusPoint = false;
			}
		}
	}

	private int RandomPatern()
	{
		if(afterCenter)
		{
			afterCenter = false;
			bonusPoint = true;
			return 0;
		}
		int i = Random.Range(0,2);
		if(i != 0)
		{
			afterCenter = true;
			isLeft = !isLeft;
			return (isLeft)?1:2;
		}
		bonusPoint = true;
		return 0;
	}

	public void SlideSideWalls()
	{
		startPoint = sideWall.transform.position;
		endPoint = startPoint + (Vector3.down * 37.5f);
		startTime = Time.time;
		isLerping = true;
	}

	#region Flow

	public void ShowMenu()
	{
		menu.SetActive(true);
		scorePanel.SetActive(false);
		endScoreText.text = "" + score.ToString();
		highscoreText.text = AppManager.instance.highscore.ToString();
	}

	public void HideMenu()
	{
		menu.SetActive(false);
		scorePanel.SetActive(true);
	}

	public void GameEnded()
	{
		gameDuration = app.GetGameDuration();
		gameActive = false;
		if(app.highscore < score)
			app.highscore = score;
		if(app.maxLevel < currLevel)
			app.maxLevel = currLevel;

		app.SaveData("meta",app.FormatMetaSaveString());
		ShowMenu();

//		if(Advertisement.isInitialized)
//		{
//			if((Time.time - app.lastAd > AppManager.AD_INTERVAL))
//			{
//				app.lastAd = Time.time;
//				Advertisement.Show(null,new ShowOptions{
//					pause = true,
//					resultCallback = result => {
//						
//					}});
//			}
//		}
	}

	public void Reset()
	{
		app.StartGame();
	}

	public void ToMenu()
	{
		app.GameToMenu();
	}

	#endregion
}
