using UnityEngine;

public class EncounterInteractable : Interactable {
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private int amount;

    public override void Interact() {
        // TODO spawn enemies at spawn pos

        alreadyInteracted = true;
    }
}
