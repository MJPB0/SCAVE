using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Pickaxe", menuName = "Player pickaxe", order = 2)]
public class PickaxeScriptableObject : ScriptableObject
{
    [SerializeField] private int tier;
    [SerializeField] private float damage;
    [SerializeField] private Sprite icon;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    public Sprite Icon {get {return icon;}}
    public int Tier {get {return tier;}}
    public float Damage {get {return damage;}}
    public Mesh Mesh {get {return mesh;}}
    public Material Material {get {return material;}}
}
