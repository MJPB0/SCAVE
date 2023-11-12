using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    [SerializeField] protected string displayName;

    [Space]
    [SerializeField] protected ObjectRequirement[] objectsRequiredToInteract;

    [Space]
    [SerializeField] protected bool requireHoldInteraction;

    [Space]
    [SerializeField] protected bool isInteractable;
    [SerializeField] protected bool canInteractOnce;
    [SerializeField] protected bool alreadyInteracted;

    public bool RequireHoldInteraction { get { return requireHoldInteraction; } }
    public bool IsInteractable { get { return isInteractable; } }
    public bool CanInteractOnce { get { return canInteractOnce; } }
    public bool AlreadyInteracted { get { return alreadyInteracted; } }
    public string DisplayName { get { return displayName; } }

    [Space]
    [SerializeField] protected Collider interactCollider;

    protected Player player;

    protected Animator animator;

    protected void Awake() {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
    }

    public abstract void Interact();

    public void Setup() {
        alreadyInteracted = false;
    }

    public bool CanInteract() => IsInteractable && (!CanInteractOnce || (CanInteractOnce && !AlreadyInteracted)) && PlayerHasRequiredObjects();

    private bool PlayerHasRequiredObjects() {
        bool hasRequiredObject = true;

        if (objectsRequiredToInteract.Length > 0) {
            Array.ForEach(objectsRequiredToInteract, (requirement) => {
                if (!player.HasInInventory((int)requirement.itemId, requirement.amount)) {
                    hasRequiredObject = false;
                    return;
                }
            });
        }

        return hasRequiredObject;
    }
}
