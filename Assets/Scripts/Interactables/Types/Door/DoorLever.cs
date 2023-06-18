using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLever : Interactable
{
    private const string PULL_LEVER = "pullLever";

    [SerializeField] private OpenDoorInteractable door;

    [SerializeField] private bool canInteractOnce = false;

    public override void Interact()
    {
        if (door == null || door.InteractionInProgress) return;

        PullLever();
        door.Interact();
        isInteractable = !canInteractOnce;
    }

    private void PullLever()
    {
        animator.SetBool(PULL_LEVER, !door.IsOpen);
    }
}
