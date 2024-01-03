using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
    }
}
