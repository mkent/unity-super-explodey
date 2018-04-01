using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {


    private PlayerManager playerManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;

    [SyncVar]
    private int score;
    private int lastScore;

    private void Update()
    {
        if(score != lastScore)
        {
            lastScore = score;

           if(playerManager.OnPlayerScored != null) playerManager.OnPlayerScored.Invoke(netId); //so we have this thing, where playerScored on playermangaer is only called on the server. By monitoring the score change on each client, we can work around that. Not sure about this implmentation yet.
        }

    }

    void OnEnable()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();

        playerManager.AddPlayer(this);
    }

    void OnDisable()
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
