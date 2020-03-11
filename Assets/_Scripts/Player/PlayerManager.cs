using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class uIntEvent : UnityEvent<uint>{}

public class PlayerManager : MonoBehaviour {

    public bool[] inputIDs = new bool[4];

    GridManager gridManager;

    public UnityEvent<uint> OnPlayerJoin = new uIntEvent();
    public UnityEvent<uint> OnPlayerLeave = new uIntEvent();
    public UnityEvent<uint> OnChangeScore = new uIntEvent();

    public List<Player> players = new List<Player>();


    public GameObject playerPrefab;

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        gridManager.OnGridLoaded.AddListener(SpawnPlayers);
    }

    public void SpawnPlayers()
    {
        CreatePlayerObjectsAtPostions(gridManager.GetSpawnPositions());
    }

    public void CreatePlayerObjectsAtPostions(List<Vector3> atPositions )
    {
        for(int i = 0; i < atPositions.Count; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab, atPositions[i], Quaternion.identity);

        }
    }

    //this adds the player the player manager's list. This is called from Player at the moment.
    public void AddPlayer(Player player)
	{
        players.Add (player);
        Debug.Log("Added Player netID: " + player.playerID.ToString());
        player.inputID = GetAvailableInputID();

        if (OnPlayerJoin != null)
        {
            OnPlayerJoin.Invoke(player.playerID); //could do this from the network join instead of ourselves.. 
        }
        else
        {
            Debug.LogWarning("OnPlayerJoin event has no listeners.");
        }

    }

    private int GetAvailableInputID()
    {
        for(int i = 0; i < inputIDs.Length; i++)
        {
            if(!inputIDs[i])
            {
                inputIDs[i] = true; //set to in use
                return i;
            }
        }

        return 0;
    }

    private void ReturnInputID(int inputID)
    {
        inputIDs[inputID] = false;
    }

    public void PlayerScored(uint playerID)
    {
        Debug.Log("Player " + playerID + " scored.");
        GetPlayer(playerID).AddScore();
    }

    public int GetPlayerScore(uint playerID)
    {
        Debug.Log("Returning a score of " + GetPlayer(playerID).Score() + " for " + playerID);
       return GetPlayer(playerID).Score();
    }

    public void ReturnPlayerBomb(uint playerID)
    {
        GetPlayer(playerID).playerCombat.ReturnBomb();
    }

    Player GetPlayer(uint playerID)
    {
        if (players.Count <= 0)
        {
            Debug.LogError("GetPlayer called when there are no players");
            return null;
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerID != playerID) continue;

            return players[i];
        }

        Debug.LogWarning("GetPlayer called with player netID " + playerID + ", but not player was found. Returning first player in list.");
        return players[0];
    }


    public void RemovePlayer(Player player)
	{
		for (int i = 0; i < players.Count; i++) 
		{
			if (players [i] == player) 
			{
                ReturnInputID(player.inputID);
                players.RemoveAt (i);
                if (OnPlayerLeave != null) OnPlayerLeave.Invoke(player.playerID);
                break;
			}
		}
	}

    public static System.Random rand = new System.Random();
    public static uint rnd32()
    {
        return (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
    }

}
