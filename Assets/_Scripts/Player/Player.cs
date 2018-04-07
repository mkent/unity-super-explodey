using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {


    private PlayerManager playerManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;



    public string myNetID;

    [SyncVar(hook = "OnChangeScore")]
    private int score;
    //private int lastScore;

    private bool isAdded = false;

    private void Update()
    {
       
        if(!isAdded)
        {
            if(netId.Value != 0)
            {
                playerManager.AddPlayer(this);
                isAdded = true;
            }    
        }

        //if(score != lastScore)
        //{
        //    lastScore = score;

        //   if(playerManager.OnPlayerScored != null) playerManager.OnPlayerScored.Invoke(netId.Value); //so we have this thing, where playerScored on playermangaer is only called on the server. By monitoring the score change on each client, we can work around that. Not sure about this implmentation yet.
        //}

        myNetID = netId.Value.ToString();

    }

    private void OnChangeScore(int _score)
    {
        score = _score;

        if (playerManager.OnChangeScore != null)
        {
            Debug.Log("Call UI Update score to " + _score + " for player " + netId.Value);
            playerManager.OnChangeScore.Invoke(netId.Value); //UI scripts subscribe to these events to trigger updates.
        }

    }


	void OnEnable()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    void OnDestroy()
    {
        playerManager.RemovePlayer(this);
    }

    //this is only ever called on the server
    public void AddScore(int _score = 1)
    {
        if (!isServer) 
        {
            Debug.LogWarning("AddScore invoked on something other than the server.");
            return; //just in case.
        }
        score += _score;
    }

    public int Score()
    {
        return score;
    }


}
