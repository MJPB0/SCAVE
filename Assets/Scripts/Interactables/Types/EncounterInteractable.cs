using UnityEngine;

public class EncounterInteractable : Interactable
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private int amount;

    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public override void Interact()
    {
        // TODO spawn enemies at spawn pos
    }
}
