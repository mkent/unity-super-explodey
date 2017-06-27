using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	[System.Serializable]
	public class Player
	{
		public PlayerMovement playerMovement;
		public PlayerCombat playerCombat;

		public Player (PlayerMovement _playerMovement, PlayerCombat _playerCombat)
		{
			playerMovement = _playerMovement;
			playerCombat = _playerCombat;
		}
	}

	public List<Player> players = new List<Player>();


	public void AddPlayer(GameObject playerObject)
	{
		Player newPlayer = new Player (playerObject.GetComponent<PlayerMovement> (), playerObject.GetComponent<PlayerCombat> ());
		players.Add (newPlayer);
	}

	public void RemovePlayer(GameObject playerObject)
	{
		for (int i = 0; i < players.Count; i++) 
		{
			if (players [i].playerMovement.gameObject == playerObject) 
			{
				players.RemoveAt (i);
				break;
			}
		}
	}

	public void DetonateDamage(Vector2Int gridPosition)
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (GridManager.IsEqual(players [i].playerMovement.GridPosition (),gridPosition))
			{
				players [i].playerCombat.DetonateDamage ();
			}
		}
	}

}
