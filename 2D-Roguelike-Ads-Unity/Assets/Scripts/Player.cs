using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 0.3f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int food;
	private Vector2 touchOrigin = -Vector2.one;

	[HideInInspector] public readonly string inGameFoodReward = "inGameFoodReward";
	[HideInInspector] public readonly string rebornWithFoodReward = "rebornWithFoodReward";

	int foodAdder = 0;
	int foodAdding = 0;
	int foodInterval = 3;

	// Use this for initialization
	protected override void Start () 
	{
		animator = GetComponent<Animator> ();
		food = GameManager.instance.playerFoodPoints;

		foodText.text = "Food: " + food;

		base.Start ();
	}

	private void OnDisable()
	{
		GameManager.instance.playerFoodPoints = food;
	}

	// Update is called once per frame
	void Update () 
	{
		if (!GameManager.instance.playersTurn) {
			return;
		}

		int horizontal = 0;
		int vertical = 0;

	#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

		horizontal = (int) Input.GetAxisRaw ("Horizontal");
		vertical = (int) Input.GetAxisRaw ("Vertical");

		if (horizontal != 0){
			vertical = 0;
		}
	#else

		if (Input.touchCount > 0)
		{
			Touch myTouch = Input.touches[0];

			if (myTouch.phase == TouchPhase.Began)
			{
				touchOrigin = myTouch.position;
			}
			else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
			{
				Vector2 touchEnd = myTouch.position;
				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;
				touchOrigin.x = -1;
				if (Mathf.Abs(x) > Mathf.Abs(y)) {
					horizontal = x > 0 ? 1 : -1;
				}
				else {
					vertical = y > 0 ? 1 : -1;
				}
			}
		}

	#endif

		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall> (horizontal, vertical);
		}

		AddUpFoodAnimated ();
	}

	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		food-=4;
		foodText.text = "Food: " + food;

		base.AttemptMove<T> (xDir, yDir);

		RaycastHit2D hit;

		if (Move (xDir, yDir, out hit)) {
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

		CheckDeath ();

		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
			other.gameObject.SetActive (false);
		} else if (other.tag == "Soda") {
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
		}
	}

	protected override void OnCantMove <T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}

	private void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	public void loseFood(int loss)
	{
		animator.SetTrigger ("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckDeath ();
	}

	private void CheckDeath()
	{
		if (food <= 0) {
			SoundManager.instance.PlaySingle(gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.RecoverDecision ();
		}
	}

	public void GameOver() 
	{
		GameManager.instance.GameOver ();
	}


	// Unity Ads demo
	public void Reward(string placementId) {
		if (placementId.Equals (inGameFoodReward)) {
			RewardFood ();
		} else if (placementId.Equals (rebornWithFoodReward)) {
			RewardRecover ();
		}
	}

	private void RewardFood() {
//		food += 50;
		foodAdder = 50;
		foodText.text = "Food: " + food;
	}

	private void RewardRecover() {
		GameManager.instance.Recover ();
//		food += 30;
		foodAdder = 30;
		foodText.text = "Food: " + food;
		if (!SoundManager.instance.musicSource.isPlaying) {
			SoundManager.instance.musicSource.Play ();
		}
	}

	private void AddUpFoodAnimated()
	{
		if(foodAdder > 0){
			if (foodAdding >= foodInterval) {
				foodAdder--;
				food++;
				foodText.text = "Food: " + food;
				foodAdding = 0;
			}
			foodAdding++;
		}
	}
}
