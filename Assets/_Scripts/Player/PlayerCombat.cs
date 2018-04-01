using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCombat : NetworkBehaviour {

	private PlayerManager playerManager;
    private BombManager bombManager;
    #region Player script links
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
		playerManager = FindObjectOfType<PlayerManager> ();
        playerMovement = GetComponent<PlayerMovement>();

		playerManager.AddPlayer (gameObject);
    }

	void OnDisable()
	{
		playerManager.RemovePlayer (gameObject);
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

        if (Input.GetKey(KeyCode.Space))
        {
            DropBomb();
        }
    }

    private void DropBomb()
    {
        if (Time.time - lastDrop < dropCooldown) return;

        if (bombCount > bombMax) return;

        lastDrop = Time.time;

        CmdDropBomb(playerMovement.GridPosition(), currentBombType); //request the server version of this player drop a bomb
    }

    [Command]
    private void CmdDropBomb(Vector2Int gridPosition, BombType bombType)
    {
        RpcDropBomb(gridPosition, bombType); //relay command to all clients.
    }

    [ClientRpc]
    public void RpcDropBomb(Vector2Int gridPosition, BombType bombType)
    {
        bombManager.DropBomb(gridPosition, bombType); //have combat manager actually drop a bomb.
    }

    public void DetonateDamage()
	{
		//we is dead.
		gameObject.SetActive(false);

	}
}
