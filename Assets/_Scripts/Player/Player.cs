using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;


public class Player : MonoBehaviour {


    private PlayerManager playerManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;

    public string myNetID;

    public uint playerID;
    public int inputID = -1;

  //  [SyncVar(hook = "OnChangeScore")]
    private int score;
    //private int lastScore;

    private bool isAdded = false;

    private void Update()
    {
       
        if(!isAdded)
        {
            if(playerID != 0)
            {
                playerManager.AddPlayer(this);
                isAdded = true;
            }
            else
            {
                playerID = PlayerManager.rnd32();
            }
        }

    }

    private void OnChangeScore(int _score)
    {
        score = _score;

        if (playerManager.OnChangeScore != null)
        {
            Debug.Log("Call UI Update score to " + _score + " for player " + playerID);
            playerManager.OnChangeScore.Invoke(playerID); //UI scripts subscribe to these events to trigger updates.
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

    public void AddScore(int _score = 1)
    {
        score += _score;
    }

    public int Score()
    {
        return score;
    }


}
