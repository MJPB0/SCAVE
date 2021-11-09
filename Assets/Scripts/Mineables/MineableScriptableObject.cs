using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mineable object", menuName ="Mineable object", order = 1)]
public class MineableScriptableObject : ScriptableObject
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int pickaxeTierRequired;
    [SerializeField] private float sizeMultiplier = 1f;
    [SerializeField] private Material material;

    public float Health {get {return maxHealth;}}
    public float SizeMultiplier {get {return sizeMultiplier;}}
    public int PickaxeTierRequired {get {return pickaxeTierRequired;}}
    public Material Material {get {return material;}}
}
