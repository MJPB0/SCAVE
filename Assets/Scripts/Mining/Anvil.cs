using System.Collections;
using UnityEngine;

public class Anvil : Interactable {
    private const string SWING_HAMMER = "swingHammer";

    [Space]
    [SerializeField] private Transform transformVFX;
    [SerializeField] private GameObject upgradeVFX;

    [Space]
    [SerializeField] private int tier;

    private Pickaxe upgradedPickaxe;

    public int Tier { get { return tier; } }

    public override void Interact() {
        player.TryUpgradePickaxe();
    }

    public void SuccessfulUpgrade(Pickaxe pickaxe) {
        upgradedPickaxe = pickaxe;
        animator.SetTrigger(SWING_HAMMER);
    }

    public void UpgradePickaxe() {
        upgradedPickaxe.Upgrade();
        upgradedPickaxe = null;
    }

    private IEnumerator ImpactParticles() {
        GameObject particles = Instantiate(upgradeVFX, transformVFX.position, new Quaternion(0, 0, 0, 0), transformVFX);
        ParticleSystem system = particles.GetComponent<ParticleSystem>();

        yield return new WaitUntil(() => !system.isPlaying);
        Destroy(particles);
    }
}
