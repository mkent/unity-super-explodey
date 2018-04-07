using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UIScoreBoard : MonoBehaviour {

    public PlayerManager playerManager;
    public GameObject scoreCardPrefab;
    private Dictionary<uint, GameObject> scoreCards = new Dictionary<uint, GameObject>();
 
    void Awake ()
    {
        if(!playerManager)playerManager = FindObjectOfType<PlayerManager>();

        playerManager.OnPlayerJoin.AddListener(PlayerJoined);
        playerManager.OnPlayerLeave.AddListener(PlayerLeft);
    }
	

    private void PlayerJoined(uint netID)
    {
        if (scoreCards.ContainsKey(netID))
        {
            Debug.LogWarning("Played joined from existing netID" + netID);
            return;
        }

        GameObject newScoreCard = Instantiate(scoreCardPrefab, transform);

        newScoreCard.GetComponent<UIScoreCard>().SetNetID(netID);

        RectTransform newScoreCardRectTransform = newScoreCard.GetComponent<RectTransform>();
        newScoreCardRectTransform.anchoredPosition = new Vector3(newScoreCardRectTransform.anchoredPosition.x, -newScoreCardRectTransform.sizeDelta.y * scoreCards.Count);

        scoreCards.Add(netID, newScoreCard);
    }

    private void PlayerLeft(uint netID)
    {
        if (!scoreCards.ContainsKey(netID)) return;

        Destroy(scoreCards[netID]);
        scoreCards.Remove(netID);
    }
    
}
