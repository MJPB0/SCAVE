using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected bool isInteractable;

    public bool IsInteractable { get { return isInteractable; } }

    [Space]
    [SerializeField] protected Collider interactCollider;

    protected Player player;

    protected Animator animator;

    protected Vector3 rotation;
    protected Vector3 forward;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        rotation = transform.rotation.eulerAngles;
        forward = transform.forward;
    }

    public abstract void Interact();

    public void Setup(bool pickable)
    {
        isInteractable = pickable;
    }
}
