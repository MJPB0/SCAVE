using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected bool isInteractable;

    public bool IsInteractable { get { return isInteractable; } }

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

    public void Setup(bool pickable)
    {
        isInteractable = pickable;
    }
}
