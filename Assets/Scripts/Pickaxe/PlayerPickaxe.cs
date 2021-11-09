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

    //Changes player's current pickaxe to the passed one 
    public void ChangePickaxe(PickaxeScriptableObject newPickaxe){
        if (newPickaxe == pickaxeSO) return;

        //set new pickaxe's values
        pickaxeSO = newPickaxe;
        tier = pickaxeSO.Tier;
        damage = pickaxeSO.Damage;
    }
}
