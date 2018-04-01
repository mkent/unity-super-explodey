using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UIScoreBoard : MonoBehaviour {

    private PlayerManager playerManager;
    public GameObject scoreCardPrefab;
    private Dictionary<NetworkInstanceId, GameObject> scoreCards = new Dictionary<NetworkInstanceId, GameObject>();
 
    void Start ()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnPlayerJoin.AddListener(PlayerJoined);
        playerManager.OnPlayerLeave.AddListener(PlayerLeft);

       
    }
	
	void Update () {
		
	}

    private void PlayerJoined(NetworkInstanceId netID)
    {
        if (scoreCards.ContainsKey(netID)) return;


        GameObject newScoreCard = Instantiate(scoreCardPrefab, transform);
        newScoreCard.SendMessage("SetNetID", netID);

        scoreCards.Add(netID, newScoreCard);
    }

    private void PlayerLeft(NetworkInstanceId netID)
    {
        if (!scoreCards.ContainsKey(netID)) return;

        Destroy(scoreCards[netID]);
        scoreCards.Remove(netID);
    }
    
}
