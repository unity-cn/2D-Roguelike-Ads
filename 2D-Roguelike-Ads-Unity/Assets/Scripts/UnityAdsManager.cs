using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.UI;


public class UnityAdsManager : MonoBehaviour {

	public Button showAdButton;
	public GameObject rewardHolder;


	private readonly float loadingInterval = 7f;
	private float loadingRemain;

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

	public void ShowAd()
	{
		Rewards rewardScript = rewardHolder.GetComponent<Rewards> ();
		rewardScript.context = rewardScript.rewardItem1;
		if (Advertisement.IsReady(rewardScript.context))
		{
			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;
			Advertisement.Show (rewardScript.context, options);
		}
	}

	private void HandleShowResult (ShowResult result)
	{
		int rewardQty = 1;
		Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
		switch (result)
		{
		case ShowResult.Finished:
			Rewards rewardScript = rewardHolder.GetComponent<Rewards> ();
			rewardScript.Reward ();
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
		buttonText.text = "Loading Video";
	}

	private void ProgressLoading() {
		if (loadingRemain > 0) {
			loadingRemain -= Time.deltaTime;

			Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
			buttonText.text = "Loading " + (int)loadingRemain + " min";
		} else if(loadingRemain <= 0 && !IsLoadingCompleted()){
			CompleteLoading ();
		}
	}

	private void CompleteLoading() {
		showAdButton.interactable = true;

		Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
		buttonText.text = "Show Ad";
	}

	private bool IsLoadingCompleted() {
		return showAdButton.IsInteractable();
	}
}
