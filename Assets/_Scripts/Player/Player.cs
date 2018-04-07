using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {


    private PlayerManager playerManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;



    public string myNetID;

    [SyncVar]
    private int score;
    private int lastScore;

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

        if(score != lastScore)
        {
            lastScore = score;

           if(playerManager.OnPlayerScored != null) playerManager.OnPlayerScored.Invoke(netId.Value); //so we have this thing, where playerScored on playermangaer is only called on the server. By monitoring the score change on each client, we can work around that. Not sure about this implmentation yet.
        }

        myNetID = netId.Value.ToString();

    }

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();

        Debug.Log("MY NET ID IS " + netId.Value.ToString());

        //CmdAddPlayer(netId.Value);
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

    public void AddScore(int _score = 1)
    {
        score += _score;
    }

    public int Score()
    {
        return score;
    }


}
