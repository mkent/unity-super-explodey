using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour {

	private PlayerManager playerManager;
    private BombManager bombManager;
    #region Player script links
    private Player player;
    private PlayerMovement playerMovement;
    #endregion

    private int bombMax = 1;
    private int bombCount = 0;
    private BombType currentBombType = BombType.Normal;

    private float lastDrop;
    private float dropCooldown = 0.1f;

    // Use this for initialization
    void OnEnable ()
    {
        bombManager = FindObjectOfType<BombManager>();
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
        playerManager = FindObjectOfType<PlayerManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (playerMovement.IsMoving()) return;

        PlayerInput();
	}

    private void PlayerInput()
    {
 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropBomb();
        }
    }

    private void DropBomb()
    {
        if (Time.time - lastDrop < dropCooldown) return;

        if (bombCount >= bombMax) return;

        lastDrop = Time.time;
        bombCount++;

        bombManager.DropBomb(playerMovement.GridPosition(), currentBombType,player.playerID);
    }

    public void ReturnBomb()
    {
        bombCount--;
    }



    public void DetonateDamage(uint originPlayerID) 
	{
        //we is dead.

            gameObject.SetActive(false);
  


	}



}
