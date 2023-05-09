using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
    public static UnityAction OnPickaxeSwing;
    public static UnityAction OnSwingTimeChanged;
    public static UnityAction OnPickaxeHit;
    public static UnityAction OnPickaxeChange;
    public static UnityAction OnObjectMined;

    public static UnityAction OnMineableObjectSelected;
    public static UnityAction OnPickableObjectSelected;
    public static UnityAction OnItemPickup;
    public static UnityAction OnObjectInteract;

    public static UnityAction OnPlayerHealthLost;
    public static UnityAction OnPlayerStaminaLost;
    public static UnityAction OnPlayerHealthRestored;
    public static UnityAction OnPlayerStaminaRestored;

    public static UnityAction OnInventoryToggle;

    public PlayerActions Controls {get; private set;}

    [Header("Object to be hit")]
    [SerializeField] private GameObject objectToHit;
    private Vector3 objectHitPos;

    private Player player;
    private PlayerMovement playerMovement;
    
    [Header("Reach")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 reachMineableHitPos;

    [Header("Body")]
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] private GameObject _rightHand;

    public GameObject ObjectToHit { get { return objectToHit; } }

    public Vector3 HitPosition {get {return objectHitPos;}}

    private void Awake() {
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
        Controls = new PlayerActions();

        Controls.Gameplay.Enable();

        Controls.Gameplay.Mine.performed += ctx => SwingPickaxe();
        
        Controls.Gameplay.Pickup.performed += ctx => TryPickingUpItem();

        Controls.Gameplay.Inventory.performed += ctx => OnInventoryToggle?.Invoke();

        Controls.Gameplay.Sprint.performed += ctx => playerMovement.StartSprinting();
        Controls.Gameplay.Sprint.canceled += ctx => playerMovement.StopSprinting();

        Controls.Gameplay.Jump.performed += ctx => playerMovement.Jump();

        Controls.Gameplay.Crouch.performed += ctx => playerMovement.ToggleCrouch();

        Controls.Gameplay.Movement.performed += ctx => playerMovement.MovementInput = ctx.ReadValue<Vector2>();
        Controls.Gameplay.Movement.canceled += cts => playerMovement.MovementInput = Vector2.zero;

        Controls.Gameplay.MouseMovement.performed += ctx => playerMovement.MouseInput = ctx.ReadValue<Vector2>();
    }

    private void Start()
    {        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnObjectMined += ObjectMined;
    }

    private void Update()
    {
        PlayerReach();
    }

    private void PlayerReach(){
        float raycastLength = player.Reach;
        Vector3 originRay = _camera.transform.position;

        Color rayColor = Color.red;
        player.SelectedObject = null; 
        player.SelectedObject = Physics.Raycast(originRay, _camera.transform.forward, out RaycastHit hit, raycastLength)
            ? hit.collider.gameObject
            : null;
        
        if(player.SelectedObject && player.SelectedObject.CompareTag(Constants.MINEABLE_TAG)) {
            MineableSelected(hit);
            rayColor = Color.green;
        }
        else if (player.SelectedObject && player.SelectedObject.CompareTag(Constants.PICKABLE_TAG)){
            PickableSelected();
            rayColor = Color.green;
        }

        Debug.DrawRay(originRay, _camera.transform.forward * raycastLength, rayColor);
    }

    private void MineableSelected(RaycastHit hit){
        reachMineableHitPos = hit.point;
        player.SelectedObject.GetComponent<MineableObject>().IsMineable(player.Pickaxe.Tier);
        OnMineableObjectSelected?.Invoke();
    }

    private void PickableSelected(){
        StartCoroutine(ItemOutline(player.SelectedObject));
        OnPickableObjectSelected?.Invoke();
    }

    private IEnumerator ItemOutline(GameObject item){
        Outline outline = item.GetComponent<Outline>();
        outline.enabled = true;
        yield return new WaitUntil(() => player.SelectedObject != item);
        
        if (item)
            outline.enabled = false;
    }

    private void TryPickingUpItem(){
        if (!player.IsGrounded || !player.SelectedObject) return;

        if (!player.SelectedObject.TryGetComponent<Item>(out Item item)) return;
        
        if (item.IsPickable){
            player.AddItemToInventory(item);
            OnItemPickup?.Invoke();
            Destroy(item.gameObject);
        }
        else
            Debug.Log($"Can't pick up {gameObject.name}!");
    }

    private void SwingPickaxe(){
        if (!player.CanSwing || player.Stamina < player.SwingStaminaLoss) return;

        player.CanSwing = false;
        player.ReduceStamina(player.SwingStaminaLoss);

        OnPickaxeSwing?.Invoke();

        if (!player.SelectedObject || !player.SelectedObject.CompareTag(Constants.MINEABLE_TAG)) {
            player.Pickaxe.SwingPickaxe(false);
            return;
        }

        objectToHit = player.SelectedObject;
        objectHitPos = reachMineableHitPos;
        player.Pickaxe.SwingPickaxe(true);
    }

    public void PickaxeHit(){
        if (player.SelectedObject && player.SelectedObject.CompareTag(Constants.MINEABLE_TAG)){
            objectHitPos = reachMineableHitPos;
            objectToHit = player.SelectedObject;
        }

        OnPickaxeHit?.Invoke();

        var mineable = objectToHit.GetComponent<MineableObject>();

        bool isCritical = Random.Range(0f, 1f) < player.CriticalChance;
        float damage = player.Pickaxe.Damage + player.Strength;
        float damageToBeDealt = isCritical ? damage * player.CriticalMultiplier : damage;

        mineable.Mine(damageToBeDealt, player.transform.position, objectHitPos);

        // TODO new impact system
        // StartCoroutine(mineable.ImpactParticles());
    }

    public void ObjectMined() {
        player.AddMinedObjectToTracker();
    }
}
