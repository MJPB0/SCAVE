using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickaxe", menuName = "Player pickaxe", order = 2)]
public class PickaxeScriptableObject : ScriptableObject
{
    [SerializeField] private int tier;
    [SerializeField] private float damage;
    [SerializeField] private GameObject model;

    public int Tier {get {return tier;}}
    public float Damage {get {return damage;}}
    public GameObject Model {get {return model;}}
}
