using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class uIntEvent : UnityEvent<uint>{}

public class PlayerManager : MonoBehaviour {

    public UnityEvent<uint> OnPlayerJoin = new uIntEvent();
    public UnityEvent<uint> OnPlayerLeave = new uIntEvent();
    public UnityEvent<uint> OnPlayerScored = new uIntEvent();

    public List<Player> players = new List<Player>();

    //this adds the player the player manager's list. This is called from Player at the moment.
    public void AddPlayer(Player playerObject)
	{
        players.Add (playerObject);
        Debug.Log("Added Player netID: " + playerObject.netId.Value.ToString());

        if (OnPlayerJoin != null)
        {
            OnPlayerJoin.Invoke(playerObject.netId.Value); //could do this from the network join instead of ourselves.. 
        }
        else
        {
            Debug.LogWarning("OnPlayerJoin event has no listeners.");
        }

    }

    public void PlayerScored(uint netID)
    {
        GetPlayer(netID).AddScore();
    }

    public int GetPlayerScore(uint netID)
    {
       return GetPlayer(netID).Score();
    }

    Player GetPlayer(uint netID)
    {
        if (players.Count <= 0)
        {
            Debug.LogError("GetPlayer called when there are no players");
            return null;
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].netId.Value != netID) continue;

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
                if (OnPlayerLeave != null) OnPlayerLeave.Invoke(playerObject.netId.Value);
                break;
			}
		}
	}



}
