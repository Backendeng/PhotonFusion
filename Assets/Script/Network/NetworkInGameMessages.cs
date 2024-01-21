using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkInGameMessages : NetworkBehaviour
{

    InGameMessageUIHandler inGameMessageUIHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SendInGameRPCMessage(string userNickName, string message)
    {
        RPC_InGameMessage($"<b>{userNickName}</b> {message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All )]
    public void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        Debug.Log($"[RPC] InGameMessage {message}");

        if (inGameMessageUIHandler == null)
            inGameMessageUIHandler = NetworkPlayer.Local.localCameraHandler.GetComponent<InGameMessageUIHandler>();

        if (inGameMessageUIHandler != null)
            inGameMessageUIHandler.OnGameMessageReceived(message);
    }

}
