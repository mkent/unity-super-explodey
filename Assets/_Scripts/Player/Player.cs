using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {


    private PlayerManager playerManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;

    [SyncVar]
    public int score;

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


}
