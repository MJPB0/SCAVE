using System.Collections;
using UnityEngine;

public class TeleportationInteractable : Interactable {
    [SerializeField] private Transform teleportPos;

    [Space]
    [SerializeField] private GameObject teleportationVFX;
    [SerializeField] private Transform teleportationVFXParent;

    private void Start() {
        Setup();
    }

    public override void Interact() {
        alreadyInteracted = true;
        player.transform.position = new(teleportPos.position.x, teleportPos.position.y + player.PlayerBodyHeight / 2, teleportPos.position.z);
        StartCoroutine(TeleportationParticles());
    }

    private IEnumerator TeleportationParticles() {
        Vector3 vfxPos = new(player.transform.position.x, player.transform.position.y - player.PlayerBodyHeight, player.transform.position.z);
        GameObject particles = Instantiate(teleportationVFX, vfxPos, Quaternion.identity, teleportationVFXParent);
        ParticleSystem system = particles.GetComponent<ParticleSystem>();

        yield return new WaitUntil(() => !system.isPlaying);
        Destroy(particles);
    }
}
