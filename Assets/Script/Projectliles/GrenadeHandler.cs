using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GrenadeHandler : NetworkBehaviour
{
    // Throw by info
    PlayerRef thrownByPlayerRef;
    string thrownByPlayerName;

    // Timing
    TickTimer explodeTickTimer = TickTimer.None;

    // Other components
    NetworkObject networkObject;
    NetworkRigidbody networkRigidbody;

    public void Throw (Vector3 throwForce, PlayerRef thrownByPlayerRef, string thrownByPlayerName)
    {
        networkObject = GetComponent<NetworkObject>();
        networkRigidbody = GetComponent<NetworkRigidbody>();

        networkRigidbody.Rigidbody.AddForce(throwForce, ForceMode.Impulse);

        this.thrownByPlayerRef = thrownByPlayerRef;
        this.thrownByPlayerName = thrownByPlayerName;

        explodeTickTimer = TickTimer.CreateFromSeconds(Runner, 2);
    }

    //Network update
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (explodeTickTimer.Expired(Runner))
            {
                Runner.Despawn(networkObject);

                // Stop the explode timer from being triggered again
                explodeTickTimer = TickTimer.None;
            }
        }
    }

    // when despawning the object we want to create a visual explosion
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        MeshRenderer grenadeMesh = GetComponentInChildren<MeshRenderer>();

    }
}
