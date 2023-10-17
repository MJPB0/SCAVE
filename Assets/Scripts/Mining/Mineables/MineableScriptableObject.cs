using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mineable object", menuName ="Mineable object", order = 1)]
public class MineableScriptableObject : ScriptableObject
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int pickaxeTierRequired;

    [Header("Visual")]
    [SerializeField] private Mesh mesh;

    [Header("Particles")]
    [SerializeField] private GameObject debrisParticles;
    [SerializeField] private GameObject successfulImpactParticles;
    [SerializeField] private GameObject failedImpactParticles;


    public float Health {get {return maxHealth;}}
    public int PickaxeTierRequired {get {return pickaxeTierRequired;}}

    public Mesh Mesh {get {return mesh;}}

    public GameObject DebrisParticles { get {return debrisParticles; } }
    public GameObject SuccessfulImpactParticles { get {return successfulImpactParticles; } }
    public GameObject FailedImpactParticles { get {return failedImpactParticles; } }
}
