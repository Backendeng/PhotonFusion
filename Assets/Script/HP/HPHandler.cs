using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    bool isInitialized = false;

    const byte startingHP = 5;

    public Color uiOnHitColor;
    public Image uiOnHitImage;

    public bool skipSettingStarValues = false;

    public MeshRenderer bodyMeshRenderer;
    Color defaultMeshBodyColor;

    //private void Awake()
    //{
        
    //}

    // Start is called before the first frame update
    void Start()
    {
        if (!skipSettingStarValues)
        {
            HP = startingHP;
            isDead = false;
        }
        
        defaultMeshBodyColor = bodyMeshRenderer.material.color;

        isInitialized = true;
    }

    IEnumerator OnHitCO()
    {
        bodyMeshRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
            uiOnHitImage.color = uiOnHitColor;

        yield return new WaitForSeconds(0.2f);

        bodyMeshRenderer.material.color = defaultMeshBodyColor;

        if (Object.HasInputAuthority && !isDead)
            uiOnHitImage.color = new Color(0, 0, 0, 0);
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

        //byte newHP = changed.Behaviour.HP;

        //// Load the old value
        //changed.LoadOld();

        //byte oldHP = changed.Behaviour.HP;

        //// check if the hp has been decreased
        //if (newHP < oldHP)
        //    changed.Behaviour.OnHPReduced();
    }

    private void OnHPReduced()
    {
        if (!isInitialized) return;

        StartCoroutine(OnHitCO());
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead}");
    }

}
