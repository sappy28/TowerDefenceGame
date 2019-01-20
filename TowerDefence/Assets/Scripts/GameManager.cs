using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum gameState
{
	play , next , gameover , win
}
public class GameManager : Singleton<GameManager>
{

	[SerializeField] private int totalWave;
	[SerializeField] private Text totalMoneyLbl ;
	[SerializeField] private Text currentWaveLbl;
	[SerializeField] private Text totalEscapeLbl;
	[SerializeField] private Text playBtnLbl;
	[SerializeField] private Text resultLbl;
	[SerializeField] private Text winlogoLbl;
	[SerializeField] private Image Result;
	[SerializeField] private Image Logo;
	[SerializeField] private Image Pause;
	[SerializeField] private Button playBtn;
	[SerializeField] private GameObject spawnPoint;
	[SerializeField] private Enemy[] enemies;
	[SerializeField] private int totalEnemies = 3;
	[SerializeField] private int enemiesPerSpawn;

	private int waveNumber = 1;
	private int totalMoney = 12;
	private int totalEscape = 0;
	private int roundEscape = 0;
	private int roundKilled = 0;
	private int totalKilled = 0;
	private int gameScore = 0;
	private int whichEnemiesToSpawn = 0;
	const float spawnDelay = 0.5f;
	private int enemiesToSpawn = 0;

	private gameState currerntState = gameState.play;
	private AudioSource audioSource;

	public List<Enemy> EnemyList = new List<Enemy>();

	public int TotalEscape
	{
		get
		{
			return totalEscape;
		}
		set
		{
			totalEscape = value;
		}
	}

	public int RoundEscape
	{
		get
		{
			return roundEscape;
		}
		set
		{
			roundEscape = value;
		}
	}

	public int RoundKilled
	{
		get
		{
			return roundKilled;
		}
		set
		{
			roundKilled = value;
			totalKilled = totalKilled + roundKilled;
		}
	}

	public int TotalMoney
	{
		get
		{
			return totalMoney;
		}
		set
		{
			totalMoney = value;
			totalMoneyLbl.text = totalMoney.ToString();
		}
	}

	public AudioSource AudioSource
	{
		get
		{
			return audioSource;
		}
	}

	// Use this for initialization
	void Start()
	{
		playBtn.gameObject.SetActive(false);
		Result.gameObject.SetActive(false);
		Logo.gameObject.SetActive(false);
		Pause.gameObject.SetActive(false);
		showMenu();
		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if (Time.timeScale == 1)
			{
				Time.timeScale = 0;
				Result.gameObject.SetActive(true);
				Logo.gameObject.SetActive(true);
				Pause.gameObject.SetActive(true);
			}
			else if (Time.timeScale == 0)
			{
				resumeBtnPressed();
			}
		}
	}

	IEnumerator spawn()
	{
		if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies)
		{
			for (int i = 0; i < enemiesPerSpawn; i++)
			{
				if (EnemyList.Count < totalEnemies)
				{
					Enemy newEnemy = Instantiate(enemies[Random.Range(0 , enemiesToSpawn)]) as Enemy;
					newEnemy.transform.position = spawnPoint.transform.position;
				}

			}
			yield return new WaitForSeconds(spawnDelay);
			StartCoroutine(spawn());
		}
	}

	public void RegisterEnemy(Enemy enemy)
	{
		EnemyList.Add(enemy);
	}

	public void UnregisterEnemy(Enemy enemy)
	{
		EnemyList.Remove(enemy);
		Destroy(enemy.gameObject);
	}

	public void DestroyAllEnemies()
	{
		foreach (Enemy enemy in EnemyList)
		{ Destroy(enemy.gameObject); }

		EnemyList.Clear();
	}

	public void addMoney(int money)
	{
		TotalMoney += money;
	}

	public void subMoney(int money)
	{
		TotalMoney -= money;
	}

	public void isWaveOver()
	{
		totalEscapeLbl.text = "Escaped " + totalEscape + "/10";
		if ((RoundEscape + RoundKilled) == totalEnemies)
		{
			if (waveNumber <= enemies.Length)
			{
				enemiesToSpawn = waveNumber;
			}
			
			setCurrentGameState();
			showMenu();
		}

	}

	public void setCurrentGameState()
	{
		if (totalEscape >= 10)
		{
			currerntState = gameState.gameover;
		}

		else if (waveNumber == 0 && (RoundEscape + RoundKilled) == 0)
		{
			currerntState = gameState.play;
		}
		else if (waveNumber > totalWave)
		{
			currerntState = gameState.win;
		}
		else
		{
			currerntState = gameState.next;
		}
	}

	public void showMenu()
	{
		switch (currerntState)
		{
			case gameState.gameover:
				audioSource.PlayOneShot(SoundManager.Instance.GameOver);
				playBtnLbl.text = "Game Over! Play Again!";
				resultLbl.text = "Score " + ((2 * totalKilled) - totalEscape);
				break;

			case gameState.next:
				playBtnLbl.text = "Next Wave";
				resultLbl.text = "Score " + ((2 * totalKilled) - totalEscape);
				break;

			case gameState.play:
				playBtnLbl.text = "Play";
				resultLbl.text = "Score " + ((2 * totalKilled) - totalEscape);
				break;

			case gameState.win:
				playBtnLbl.text = "Play Again ";
				winlogoLbl.text = "WINNER";
				resultLbl.text = "Score " + (((2 * totalKilled) + totalMoney) - totalEscape);
				break;
		}
		playBtn.gameObject.SetActive(true);
		Result.gameObject.SetActive(true);
		Logo.gameObject.SetActive(true);
	}

	public void playBtnPressed()
	{
		switch (currerntState)
		{
			case gameState.next:
				waveNumber += 1;
				totalEnemies = (waveNumber+3);
				break;

			default:
				waveNumber = 1;
				totalEnemies = 3;
				TotalEscape = 0;
				TotalMoney = 12;
				enemiesToSpawn = 0;
				gameScore = 0;
				totalKilled = 0;
				TowerManager.Instance.DestroyAllTower();
				TowerManager.Instance.RenameTagBuildSites();
				totalMoneyLbl.text = TotalMoney.ToString();
				totalEscapeLbl.text = "Escaped " + TotalEscape + "/10";
				audioSource.PlayOneShot(SoundManager.Instance.NewGame);
				break;

		}
		DestroyAllEnemies();
		RoundEscape = 0;
		RoundKilled = 0;
		currentWaveLbl.text = "Wave " + (waveNumber) + "/10";
		StartCoroutine(spawn());
		playBtn.gameObject.SetActive(false);
		Result.gameObject.SetActive(false);
		Logo.gameObject.SetActive(false);
	}

	public void resumeBtnPressed()
	{
		Time.timeScale = 1;
		Result.gameObject.SetActive(false);
		Logo.gameObject.SetActive(false);
		Pause.gameObject.SetActive(false);
	}

	public void exitBtnPressed()
	{
		Application.Quit();
	}
}