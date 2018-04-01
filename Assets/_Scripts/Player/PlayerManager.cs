using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : MonoBehaviour {


	public List<Player> players = new List<Player>();

    //this adds the player the player manager's list. This is called from PlayerCombat at the moment.
	public void AddPlayer(Player playerObject)
	{
        players.Add (playerObject);
	}

    public void PlayerScored(NetworkInstanceId netID)
    {
        GetPlayer(netID).AddScore();
    }

    Player GetPlayer(NetworkInstanceId netID)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].netId != netID) continue;

            return players[i];
        }

        return null;
    }


    public void RemovePlayer(Player playerObject)
	{
		for (int i = 0; i < players.Count; i++) 
		{
			if (players [i] == playerObject) 
			{
				players.RemoveAt (i);
				break;
			}
		}
	}



}
