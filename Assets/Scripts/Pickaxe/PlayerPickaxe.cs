using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickaxe : MonoBehaviour
{
    [SerializeField] private int tier;
    [SerializeField] private float damage;
    [SerializeField] private PickaxeScriptableObject pickaxeSO;

    public int Tier {get {return tier;}}
    public float Damage {get {return damage;}}

    public void ChangePickaxe(PickaxeScriptableObject newPickaxe){
        if (newPickaxe == pickaxeSO) return;
        pickaxeSO = newPickaxe;
        tier = pickaxeSO.Tier;
        damage = pickaxeSO.Damage;
    }
}
