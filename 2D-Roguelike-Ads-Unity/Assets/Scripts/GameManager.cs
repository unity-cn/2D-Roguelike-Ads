using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public float turnDelay = .1f;
	public static GameManager instance = null;
	public BoardManager boardScript;
	public int playerFoodPoints = 60;
	[HideInInspector] public bool playersTurn = true;

	private Text levelText;
	private GameObject levelImage;
	private int level = 1;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup;

	// Use this for initialization
	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
		enemies = new List<Enemy>();
		boardScript = GetComponent<BoardManager>();
		InitGame ();
	}

	private void OnLevelWasLoaded(int index)
	{
		level++;

		InitGame ();
	}

	void InitGame()
	{
		doingSetup = true;

		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		RecoverOptions (false);
		levelImage.SetActive (true);
		Invoke ("HideLevelImage", levelStartDelay);

		enemies.Clear ();
		boardScript.SetupScene (level);
	}

	private void HideLevelImage()
	{
		levelImage.SetActive (false);
		doingSetup = false;
	}

	private void RecoverOptions(bool appear)
	{
		GameObject recoverGo = GameObject.Find ("Recover");
		GameObject payGo = GameObject.Find ("PayRecover");
		GameObject gameOverGo = GameObject.Find ("GameOver");

		if (recoverGo != null) {
			Text recoverText = recoverGo.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
			recoverText.text = appear ? "Recover = Watch an ad" : "";
		}

		if (payGo != null) {
			Text payText = payGo.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
			payText.text = appear ? "Recover = Diamond x1" : "";
		}

		if (gameOverGo != null) {
			Text gameOverText = gameOverGo.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
			gameOverText.text = appear ? "END" : "";
		}
	}

	public void RecoverDecision()
	{
		levelText.text = "";
		levelImage.SetActive (true);
		RecoverOptions (true);
		enabled = false;
	}

	public void Recover(){
		levelImage.SetActive (false);
		RecoverOptions (false);
		levelText.text = "Day " + level;
		enabled = true;
	}

	public void GameOver()
	{
		levelText.text = "Starved after " + level + " days";
		levelImage.SetActive (true);
		enabled = false;
		RecoverOptions (false);
	}

	// Update is called once per frame
	void Update () {
		if (playersTurn || enemiesMoving) {
			return;
		}

		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add (script);
	}

	IEnumerator MoveEnemies ()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++) 
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		playersTurn = true;
		enemiesMoving = false;
	}
}
