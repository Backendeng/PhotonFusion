using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TextMeshProUGUI playerNickNameTM;

    public static NetworkPlayer Local { get; set; }
    public Transform playerModel;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName {  get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            //Sets the layer of the local players model
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            // Disable main camera
            Camera.main.gameObject.SetActive(false);

            Debug.Log("Spawned local player");
        }
        else
        {
            // Disable the camera if we are not the local player
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            // Only 1 audio listner is allowed in the scene do disable rmote players audio listner
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            Debug.Log("Spawned remote player");
        }

        // Make it easier to tell which player is which
        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.nickName}");

        changed.Behaviour.OnNickNameChanged();
    }

    private void OnNickNameChanged()
    {
        Debug.Log($"Nickname changed for player to {nickName} for player {gameObject.name}");

        playerNickNameTM.text = nickName.ToString();
    }

}
