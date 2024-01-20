using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    bool isInitialized = false;

    const byte startingHP = 5;

    // Start is called before the first frame update
    void Start()
    {
        HP = startingHP;
        isDead = false;
    }

    // Function only called on the server
    public void OnTaskDamage()
    {
        // only task damage while alive
        if (isDead) return;

        HP -= 1;

        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");

        // Player died
        if (HP <= 0 )
        {
            Debug.Log($"{Time.time} {transform.name} died");

            isDead = true;
        }
    }

    static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead}");
    }

}
