using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : NetworkBehaviour {

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
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropBomb();
        }
    }

    private void DropBomb()
    {
        if (Time.time - lastDrop < dropCooldown) return;

        if (bombCount > bombMax) return;

        lastDrop = Time.time;
        dropCooldown = 3.0f;
        //bombCount++;

        CmdDropBomb(playerMovement.GridPosition(), currentBombType); //request the server version of this player drop a bomb
    }

    [Command]
    private void CmdDropBomb(Vector2Int gridPosition, BombType bombType)
    {
        if(isServer && !isClient)bombManager.DropBomb(gridPosition, bombType, netId.Value);//drop server bomb
        RpcDropBomb(gridPosition, bombType); //relay command to all clients.
    }

    [ClientRpc]
    public void RpcDropBomb(Vector2Int gridPosition, BombType bombType)
    {
        bombManager.DropBomb(gridPosition, bombType, netId.Value ); //have combat manager actually drop a bomb. Note, right now this is how the local player sees this. 
    }

    public void DetonateDamage(uint originNetID) 
	{
        //we is dead.

        //Quick reminder as to where this originates. 
        //A Player drops a bomb, that bomb drop is synced by the server
        //The bomb has a default timer built in, it detonates after that duration. (This is exploitable per client, but wouldn't matter anyway since the server is the one who decides who dies)
        //That detonation is handled by GridManager, which determines which grid spots are taking damage
        //Grid manager also passes the block coordinates to player manager, which checks against its player positions
        //If a player's grid position matches any of those block coordinates, that player will have this method called.
        //The actual hit is detected on the server. All clients discard the death information. hit information could be used for visuals.  

        if (isServer)
        {
            playerManager.PlayerScored(originNetID);
            gameObject.SetActive(false);
            RpcDie();
        }


	}

    [ClientRpc]
    public void RpcDie()
    {
        gameObject.SetActive(false);
    }

}
