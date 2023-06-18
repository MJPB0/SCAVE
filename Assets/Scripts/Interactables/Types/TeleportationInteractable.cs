using UnityEngine;

public class TeleportationInteractable : Interactable
{
    [SerializeField] private Transform teleportationPos;

    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public override void Interact()
    {
        // TODO teleport player to teleportationPos
    }
}
