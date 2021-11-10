using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickaxe : MonoBehaviour
{
    private const string OBJECT_SELECTED = "objectSelected";
    private const string PICKAXE_SWANG = "pickaxeSwang";
    private const string SWING_ANIMATION = "swingAnimation";

    [SerializeField] private int tier;
    [SerializeField] private float damage;
    [SerializeField] private PickaxeScriptableObject pickaxeSO;

    [Space]
    [SerializeField] private AnimationClip[] swingAnimations;
    [SerializeField] private AnimationClip[] swingHitAnimations;

    private Animator controller;

    public int Tier {get {return tier;}}
    public float Damage {get {return damage;}}

    private void Awake() {
        controller = GetComponent<Animator>();
    }

    //Changes player's current pickaxe to the passed one 
    public void ChangePickaxe(PickaxeScriptableObject newPickaxe){
        if (newPickaxe == pickaxeSO) return;

        //set new pickaxe's values
        pickaxeSO = newPickaxe;
        tier = pickaxeSO.Tier;
        damage = pickaxeSO.Damage;
    }

    private void ResetAnimatorParameters(){
        controller.SetBool(PICKAXE_SWANG, false);
        controller.SetBool(OBJECT_SELECTED, false);
        controller.SetInteger(SWING_ANIMATION, 0);
    }

    public void SwingPickaxe(bool hitTarget){
        controller.SetBool(PICKAXE_SWANG, true);
        controller.SetBool(OBJECT_SELECTED, hitTarget);
        if (hitTarget)
            controller.SetInteger(SWING_ANIMATION, Random.Range(0,swingHitAnimations.Length));
        else
            controller.SetInteger(SWING_ANIMATION, Random.Range(0,swingAnimations.Length));
    }

    public void PickaxeSwang(){
        ResetAnimatorParameters();
    }

    public void PickaxeHit(){
        ResetAnimatorParameters();
    }

    public void PickaxeStuck(){
        
    }
}
