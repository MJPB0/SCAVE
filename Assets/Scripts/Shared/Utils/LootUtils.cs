using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootUtils
{
    public static Vector3 CalculateLootForce(Vector3 playerPos, Vector3 objectPos, float minForce, float maxForce)
    {
        Vector3 forceDirection = playerPos - objectPos;
        float forceX = Random.Range(minForce, maxForce);
        float forceY = Random.Range(minForce, maxForce);
        float forceZ = Random.Range(minForce, maxForce);
        return new Vector3(forceDirection.x * forceX, forceDirection.y * forceY, forceDirection.z * forceZ);
    }
}
