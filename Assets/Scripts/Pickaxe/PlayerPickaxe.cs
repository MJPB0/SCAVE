using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPickaxe : MonoBehaviour
{
    private const string OBJECT_SELECTED = "objectSelected";
    private const string PICKAXE_SWANG = "pickaxeSwang";
    private const string SWING_ANIMATION = "swingAnimation";

    [SerializeField] private int tier;
    [SerializeField] private float damage;
    [SerializeField] private Sprite icon;
    [SerializeField] private PickaxeScriptableObject pickaxeSO;

    [Space]
    [SerializeField] private AnimationClip[] swingAnimations;
    [SerializeField] private AnimationClip[] swingHitAnimations;

    private PlayerController playerController;
    private Animator animationController;

    public int Tier {get {return tier;}}
    public float Damage {get {return damage;}}
    public Sprite Icon {get {return icon;}}

    private void Awake() {
        animationController = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
    }

    public void ChangePickaxe(PickaxeScriptableObject newPickaxe){
        if (newPickaxe == pickaxeSO) return;

        pickaxeSO = newPickaxe;
        tier = pickaxeSO.Tier;
        damage = pickaxeSO.Damage;
        icon = pickaxeSO.Icon;

        PlayerController.OnPickaxeChange?.Invoke();
    }

    private void ResetAnimatorParameters(){
        animationController.SetBool(PICKAXE_SWANG, false);
        animationController.SetBool(OBJECT_SELECTED, false);
        animationController.SetInteger(SWING_ANIMATION, 0);
    }

    public void SwingPickaxe(bool hitTarget){
        animationController.SetBool(PICKAXE_SWANG, true);
        animationController.SetBool(OBJECT_SELECTED, hitTarget);
        if (hitTarget)
            animationController.SetInteger(SWING_ANIMATION, Random.Range(0,swingHitAnimations.Length));
        else
            animationController.SetInteger(SWING_ANIMATION, Random.Range(0,swingAnimations.Length));
    }

    public void PickaxeSwang(){
        ResetAnimatorParameters();
    }

    public void PickaxeHit(){
        ResetAnimatorParameters();
        playerController.PickaxeHit();
    }
}
