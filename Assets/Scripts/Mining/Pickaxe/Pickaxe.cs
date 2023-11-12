using System;
using UnityEngine;

public class Pickaxe : MonoBehaviour {
    private const string OBJECT_SELECTED = "objectSelected";
    private const string PICKAXE_SWANG = "pickaxeSwang";
    private const string SWING_ANIMATION = "swingAnimation";

    [SerializeField] private PickaxeScriptableObject pickaxeSO;

    [Space]
    [SerializeField] private int currentLevel;
    [SerializeField] private int maxLevel;

    [Space]
    [SerializeField] private UpgradeCost[] upgradeCosts;
    [SerializeField] private PickaxeValues[] values;

    // TODO: effects

    [Space]
    [SerializeField] private AnimationClip[] swingAnimations;
    [SerializeField] private AnimationClip[] swingHitAnimations;

    private PlayerController playerController;
    private Animator animationController;

    public int Tier { get { return Array.Find(values, value => value.level == currentLevel).tier; } }
    public float Damage { get { return Array.Find(values, value => value.level == currentLevel).damage; } }

    public UpgradeCost NextLevelUpgradeCost { get { return Array.Find(upgradeCosts, cost => cost.upgradeToLevel == currentLevel + 1); } }

    public Sprite Icon { get { return pickaxeSO.Icon; } }

    private void Awake() {
        animationController = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void ResetAnimatorParameters() {
        animationController.SetBool(PICKAXE_SWANG, false);
        animationController.SetBool(OBJECT_SELECTED, false);
        animationController.SetInteger(SWING_ANIMATION, 0);
    }

    public void SwingPickaxe(bool hitTarget) {
        animationController.SetBool(PICKAXE_SWANG, true);
        animationController.SetBool(OBJECT_SELECTED, hitTarget);

        if (hitTarget)
            animationController.SetInteger(SWING_ANIMATION, UnityEngine.Random.Range(0, swingHitAnimations.Length));
        else
            animationController.SetInteger(SWING_ANIMATION, UnityEngine.Random.Range(0, swingAnimations.Length));
    }

    public void PickaxeSwang() {
        ResetAnimatorParameters();
    }

    public void PickaxeHit() {
        ResetAnimatorParameters();
        playerController.PickaxeHit();
    }

    public bool IsFullyUpgraded() {
        return currentLevel == maxLevel;
    }

    public void Upgrade() {
        Logger.Log(LogType.PICKAXE_UPGRADED, gameObject.name);
        this.currentLevel++;
    }

    public string GetUpgradeRequirementsList() {
        if (IsFullyUpgraded()) {
            return $"{gameObject.name} \nis fully upgraded";
        }

        bool requiresGold = NextLevelUpgradeCost.goldCost > 0;
        bool requiresResources = NextLevelUpgradeCost.materialsCost.Length > 0;

        if (!requiresGold && !requiresResources) {
            return "Free";
        }

        string text = "";

        if (requiresGold) {
            string pluralGold = NextLevelUpgradeCost.goldCost > 1 ? "s" : "";
            text += $"{NextLevelUpgradeCost.goldCost} Gold nugget{pluralGold}\n";
        }

        foreach (MaterialCost material in NextLevelUpgradeCost.materialsCost) {
            string pluralMaterial = material.weight > 1 ? "s" : "";
            text += $"{material.weight} {material.itemId} nugget{pluralMaterial}\n";
        }

        return text;
    }
}
