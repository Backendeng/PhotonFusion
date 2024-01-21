using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    [Header("Prefabs")]
    public GrenadeHandler grenadePrefab;

    [Header("Effects")]
    public ParticleSystem fireParticleSystem;

    [Header("Aim")]
    public Transform aimPoint;

    [Header("Collision")]
    public LayerMask collisionLayers;

    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring {  get; set; }


    float lastTimeFired = 0;

    //Timing
    TickTimer grenadeFireDelay = TickTimer.None;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        // Get then input from the network
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFireButtonPressed)
                Fire(networkInputData.aimForwardVector);

            if (networkInputData.isGrenadeFireButtonPressed)
                FireGrenade(networkInputData.aimForwardVector);
        }
    }

    void Fire(Vector3 aimForwardVector)
    {
        // Limit fire rate
        if (Time.time - lastTimeFired < 0.15f)
            return;

        StartCoroutine(FireEffectCO());

        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100, Object.InputAuthority, out var hitinfo, collisionLayers, HitOptions.IncludePhysX);

        float hitDistance = 100;
        bool isHitOtherPlayer = false;

        if (hitinfo.Distance > 0)
            hitDistance = hitinfo.Distance;

        if (hitinfo.Hitbox != null) 
        {
            Debug.Log($"{Time.time} {transform.name} hit hitbox {hitinfo.Hitbox.transform.root.name}");

            if (Object.HasInputAuthority)
                hitinfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTaskDamage();
             
            isHitOtherPlayer = true;
        }
        else if (hitinfo.Collider != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit PhysX collider {hitinfo.Collider.transform.name}");
        }

        // Debug
        if (isHitOtherPlayer)
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1);
        else Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1);

        lastTimeFired = Time.time;
    }

    void FireGrenade(Vector3 aimForwardVector)
    {
        Debug.Log("fireGrenade");
        // Check that we have not recently fired a grenade.
        if (grenadeFireDelay.ExpiredOrNotRunning(Runner))
        {
            Runner.Spawn(grenadePrefab, aimPoint.position + aimForwardVector * 1.5f, Quaternion.LookRotation(aimForwardVector), Object.InputAuthority, (runner, spawnedGrende) =>
            {
                spawnedGrende.GetComponent<GrenadeHandler>().Throw(aimForwardVector * 15, Object.InputAuthority, "playername nick");
            });
        }
    }

    IEnumerator FireEffectCO()
    {
        isFiring = true;

        fireParticleSystem.Play();
        
        yield return new WaitForSeconds(0.09f);

        isFiring = false;
    }

    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        Debug.Log($"{Time.time} OnFireChanged value {changed.Behaviour.isFiring}");

        bool isFiringCurrent = changed.Behaviour.isFiring;

        // Load the old value
        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld) 
        {
            changed.Behaviour.OnFireRemote();
        }
    }

    void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
            fireParticleSystem.Play();
    }
}
