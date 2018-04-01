using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIScoreCard : MonoBehaviour {

    private PlayerManager playerManager;

    private NetworkInstanceId netID;
    public Text textName;
    public Text textScore;

	// Use this for initialization
	void Start ()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnPlayerScored.AddListener(PlayerScored); //refresh UI
    }

    public void SetNetID(NetworkInstanceId _netID)
    {
        netID = _netID;
    }
	
    public void PlayerScored(NetworkInstanceId _netID)
    {
        if(netID == _netID)
        {
            textScore.text = playerManager.GetPlayerScore(netID).ToString();
        }
    }

}
