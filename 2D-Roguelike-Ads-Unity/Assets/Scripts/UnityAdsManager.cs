using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.UI;


public class UnityAdsManager : MonoBehaviour {

	private Button showAdButton;
	private string contextPlacementId;


	private readonly float loadingInterval = 7f;
	private float loadingRemain;

	void Awake()
	{
		showAdButton = GameObject.Find ("RewardButton").GetComponent<Button>();
	}

	// Use this for initialization
	void Start () 
	{
		if (showAdButton != null) {
			showAdButton.gameObject.SetActive (false);
		}
		StartLoading ();

		StartCoroutine(RevealShowAdButton());
	}

	private IEnumerator RevealShowAdButton()
	{
		if(!Advertisement.isInitialized || !Advertisement.IsReady()) {
			yield return new WaitForSeconds(0.5f);
		}
		if (showAdButton != null) {
			showAdButton.gameObject.SetActive (true);
		}
	}

	void Update() {
		ProgressLoading ();
	}

	public void ShowAd(string placementId)
	{
		contextPlacementId = placementId;
		if (Advertisement.IsReady(contextPlacementId))
		{
			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;
			Advertisement.Show (contextPlacementId, options);
		}
	}

	private void HandleShowResult (ShowResult result)
	{
		Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
		switch (result)
		{
		case ShowResult.Finished:
			GameObject playerGo = GameObject.Find ("Player");
			Player player = playerGo.GetComponent<Player> () as Player;
			player.Reward (contextPlacementId);
			break;
		case ShowResult.Skipped:
			buttonText.text = "Video was skipped.";
			Debug.Log ("Video was skipped.");
			break;
		case ShowResult.Failed:
			buttonText.text = "Video failed to show.";
			Debug.Log ("Video failed to show.");
			break;
		}

		StartLoading ();
	}

	// simulate ads loading
	private void StartLoading() {
		loadingRemain = loadingInterval;
		showAdButton.interactable = false;

		Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
		buttonText.text = "广告准备中...";
	}

	private void ProgressLoading() {
		if (loadingRemain > 0) {
			loadingRemain -= Time.deltaTime;

			Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
			buttonText.text = "准备中,剩余" + (int)loadingRemain + "分钟";
		} else if(loadingRemain <= 0 && !IsLoadingCompleted()){
			CompleteLoading ();
		}
	}

	private void CompleteLoading() {
		showAdButton.interactable = true;

		Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
		buttonText.text = "看广告得免费食物";
	}

	private bool IsLoadingCompleted() {
		return showAdButton.IsInteractable();
	}
}
