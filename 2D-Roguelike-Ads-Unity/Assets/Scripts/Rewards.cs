using UnityEngine;
using System.Collections;

public class Rewards : MonoBehaviour {

	[HideInInspector] public readonly string rewardItem1 = "inGameFoodReward";
	[HideInInspector] public readonly string rewardItem2 = "rebornWithFoodReward";
	public string context;


	public void Reward() {
		Debug.Log ("Rewarding with item = " + context);
		if(!string.IsNullOrEmpty(context)) {
			if (context.Equals (rewardItem1)) {
				RewardFood ();
			} else if (context.Equals(rewardItem2)) {
				Reborn ();
			}
		}
	}

	private void RewardFood() {
		Player player = GameObject.Find ("Player").GetComponent<Player>() as Player;
		if (player != null) {
			player.RewardFood ();
		}
	}

	private void Reborn() {
		
	}
}
