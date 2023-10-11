using System.Collections;
using UnityEngine;

public class TeleportationInteractable : Interactable {
    [SerializeField] private Transform teleportPos;

    [Space]
    [SerializeField] private GameObject teleportationVFX;
    [SerializeField] private Transform teleportationVFXParent;

    private void Start() {
        Setup(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;

        isInteractable = true;
    }

    private void OnTriggerStay(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;
         
        player.Controller.TryInteractingWithObject(this);
    }

    private void OnTriggerExit(Collider other) {
        if (!other.gameObject.CompareTag("Player")) return;

        isInteractable = false;
    }

    public override void Interact() {
        player.transform.position = new Vector3(teleportPos.position.x, teleportPos.position.y + player.PlayerBodyHeight / 2, teleportPos.position.z);

        StartCoroutine(TeleportationParticles());

        alreadyInteracted = true;
    }

    private IEnumerator TeleportationParticles() {
        Vector3 vfxPos = new Vector3(player.transform.position.x, player.transform.position.y - player.PlayerBodyHeight / 2, player.transform.position.z);
        GameObject particles = Instantiate(teleportationVFX, vfxPos, Quaternion.identity, teleportationVFXParent);
        ParticleSystem system = particles.GetComponent<ParticleSystem>();

        yield return new WaitUntil(() => !system.isPlaying);
        Destroy(particles);
    }
}
