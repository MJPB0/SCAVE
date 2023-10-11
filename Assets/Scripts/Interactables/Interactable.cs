using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected bool isInteractable;
    [SerializeField] protected bool canInteractOnce;
    [SerializeField] protected bool alreadyInteracted;

    public bool IsInteractable { get { return isInteractable; } }
    public bool CanInteractOnce { get { return canInteractOnce; } }
    public bool AlreadyInteracted { get { return alreadyInteracted; } }

    [Space]
    [SerializeField] protected Collider interactCollider;

    protected Player player;

    protected Animator animator;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
    }

    public abstract void Interact();

    public void Setup(bool interactable)
    {
        isInteractable = interactable;
        alreadyInteracted = false;
    }
}
