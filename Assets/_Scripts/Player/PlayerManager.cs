using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class NetIDEvent : UnityEvent<NetworkInstanceId>{}

public class PlayerManager : MonoBehaviour {

    public UnityEvent<NetworkInstanceId> OnPlayerJoin = new NetIDEvent();
    public UnityEvent<NetworkInstanceId> OnPlayerLeave = new NetIDEvent();
    public UnityEvent<NetworkInstanceId> OnPlayerScored = new NetIDEvent();

    public List<Player> players = new List<Player>();

    //this adds the player the player manager's list. This is called from Player at the moment.
    public void AddPlayer(Player playerObject)
	{
        players.Add (playerObject);
        Debug.Log("ADded PLayer");

        if (OnPlayerJoin != null)
        {
            OnPlayerJoin.Invoke(playerObject.netId); //could do this from the network join instead of ourselves.. 
        }

    }

    public void PlayerScored(NetworkInstanceId netID)
    {
        GetPlayer(netID).AddScore();
    }

    public int GetPlayerScore(NetworkInstanceId netID)
    {
       return GetPlayer(netID).Score();
    }

    Player GetPlayer(NetworkInstanceId netID)
    {
        if (players.Count <= 0)
        {
            Debug.LogError("GetPlayer called when there are no players");
            return null;
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].netId != netID) continue;

            return players[i];
        }

        Debug.LogWarning("GetPlayer called with player netID " + netID + ", but not player was found. Returning first player in list.");
        return players[0];
    }


    public void RemovePlayer(Player playerObject)
	{
		for (int i = 0; i < players.Count; i++) 
		{
			if (players [i] == playerObject) 
			{
                players.RemoveAt (i);
                if (OnPlayerLeave != null) OnPlayerLeave.Invoke(playerObject.netId);
                break;
			}
		}
	}



}
