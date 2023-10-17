using System.Collections;
using UnityEngine;

public class Anvil : Interactable {
    [Space]
    [SerializeField] private Transform transformVFX;
    [SerializeField] private GameObject upgradeVFX;
    [SerializeField] private Transform upgradeVFXParent;

    private void Awake() {
        base.Awake();
        PlayerController.OnPickaxeUpgraded += SuccessfulUpgrade;
    }

    public override void Interact() {
        // TODO: add UI
        // TODO: upgrade after UI interaction

        player.TryUpgradePickaxe();
    }

    public void SuccessfulUpgrade() {
        StartCoroutine(UpgradeParticles());
    }

    private IEnumerator UpgradeParticles() {
        GameObject particles = Instantiate(upgradeVFX, transformVFX.position, Quaternion.identity, upgradeVFXParent);
        ParticleSystem system = particles.GetComponent<ParticleSystem>();

        yield return new WaitUntil(() => !system.isPlaying);
        Destroy(particles);
    }
}
