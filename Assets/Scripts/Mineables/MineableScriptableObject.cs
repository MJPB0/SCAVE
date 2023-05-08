using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mineable object", menuName ="Mineable object", order = 1)]
public class MineableScriptableObject : ScriptableObject
{
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private int pickaxeTierRequired;

    [SerializeField] private Mesh mesh;

    public float Health {get {return maxHealth;}}
    public int PickaxeTierRequired {get {return pickaxeTierRequired;}}
    public Mesh Mesh {get {return mesh;}}
}
