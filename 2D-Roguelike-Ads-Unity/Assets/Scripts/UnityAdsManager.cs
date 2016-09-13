using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.UI;


public class UnityAdsManager : MonoBehaviour {

	public Button showAdButton;

	void Awake () 
	{
		if (showAdButton != null) {
			showAdButton.gameObject.SetActive (false);
		}

	}
	// Use this for initialization
	void Start () 
	{
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

	public void ShowAd()
	{
		if (Advertisement.IsReady())
		{
			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;
			Advertisement.Show ("video", options);
		}
	}

	private void HandleShowResult (ShowResult result)
	{
		int rewardQty = 1;
		Text buttonText = showAdButton.gameObject.GetComponentsInChildren (typeof(UnityEngine.UI.Text)) [0] as Text;
		switch (result)
		{
		case ShowResult.Finished:
			buttonText.text = "Video completed. User rewarded " + rewardQty + " credits.";
			break;
		case ShowResult.Skipped:
			buttonText.text = "Video was skipped.";
			break;
		case ShowResult.Failed:
			buttonText.text = "Video failed to show.";
			break;
		}
	}
}
