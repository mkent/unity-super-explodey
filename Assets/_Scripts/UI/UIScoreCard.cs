using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIScoreCard : MonoBehaviour {

    private PlayerManager playerManager;

    private uint netID;
    public Text textName;
    public Text textScore;

	// Use this for initialization
	void Start ()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.OnChangeScore.AddListener(OnChangeScore); //refresh UI
    }

    public void SetNetID(uint _netID)
    {
        netID = _netID;
    }
	
    public void OnChangeScore(uint _netID)
    {
        if(netID == _netID)
        {
            Debug.Log("Updating Player Score for " + netID);
            textScore.text = playerManager.GetPlayerScore(netID).ToString();
        }
    }

}
