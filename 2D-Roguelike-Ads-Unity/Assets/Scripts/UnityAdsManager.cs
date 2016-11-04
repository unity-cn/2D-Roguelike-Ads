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
		// 1 Check if initialized(Make sure to turn on Ads Service)
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

		// 2 Show an ad
		if (Advertisement.IsReady(contextPlacementId))
		{
			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;
			Advertisement.Show (contextPlacementId, options);
		}
	}

	// 3 Handle callbacks
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
		buttonText.text = "Loading ...";
	}

	private void ProgressLoading() {
		if (loadingRemain > 0) {
			loadingRemain -= Time.deltaTime;

			Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
			buttonText.text = (int)loadingRemain + " min remaining";
		} else if(loadingRemain <= 0 && !IsLoadingCompleted()){
			CompleteLoading ();
		}
	}

	private void CompleteLoading() {
		showAdButton.interactable = true;

		Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
		buttonText.text = "Free food by watching an ad";
	}

	private bool IsLoadingCompleted() {
		return showAdButton.IsInteractable();
	}
}
