using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    private void Awake()
    {
        ClearList();
    }

    public void ClearList()
    {
        //Delete all children of the vertical layout group
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("hide status");
        //Hide the status message
        statusText.gameObject.SetActive(false);
    }

    public void AddtToList(SessionInfo sessionInfo)
    {
        // Add a new item to the list
        SessionInfoListUIItem addedSessionInfoListUIItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListUIItem>();

        addedSessionInfoListUIItem.SetInformation(sessionInfo);

        // Hook up events
        addedSessionInfoListUIItem.onJoinSession += AddedSessionInfoListUIItem_OnJoinSession;
    }

    private void AddedSessionInfoListUIItem_OnJoinSession(SessionInfo obj)
    {

    }

    public void OnNoSessionFound()
    {
        statusText.text = "No game session found";
        statusText.gameObject.SetActive(true);
    }

    public void OnLookingForGameSessions()
    {
        statusText.text = "Looking for game sessions";
        statusText.gameObject.SetActive(true);
    }
}

