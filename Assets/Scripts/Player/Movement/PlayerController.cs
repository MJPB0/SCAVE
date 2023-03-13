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

    private GameObject objectToHit;
    private Vector3 objectHitPos;

    private Rigidbody rbody;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private GameObject playerCrouchBody;
    private Player player;

    private Vector2 movementInput;
    private Vector2 mouseInput;
    
    [Header("Reach")]
    [SerializeField] private Vector3 reachMineableHitPos;

    [Header("Body")]
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _head;

    [Header("Camera")]
    [SerializeField] private float sensitivity = 20f;
    [SerializeField] private float xRotation = 0f;
    [SerializeField] private GameObject _view;
    [SerializeField] private Camera _camera;

    [Header("Gravity")]
    [SerializeField] private float gravityMultiplier = 1.75f;

    public Vector3 HitPosition {get {return objectHitPos;}}

    private void Awake() {
        rbody = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
        Controls = new PlayerActions();

        Controls.Gameplay.Enable();

        Controls.Gameplay.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        Controls.Gameplay.Movement.canceled += cts => movementInput = Vector2.zero;

        Controls.Gameplay.Sprint.performed += ctx => StartSprinting();
        Controls.Gameplay.Sprint.canceled += ctx => StopSprinting();

        Controls.Gameplay.Jump.performed += ctx => Jump();

        Controls.Gameplay.Crouch.performed += ctx => ToggleCrouch();

        Controls.Gameplay.MouseMovement.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();

        Controls.Gameplay.Mine.performed += ctx => SwingPickaxe();
        
        Controls.Gameplay.Pickup.performed += ctx => TryPickingUpItem();

        Controls.Gameplay.Inventory.performed += ctx => OnInventoryToggle?.Invoke();
    }

    private void Start()
    {        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnObjectMined += ObjectMined;
    }

    private void Update() {
        if (player.IsCrouching)
            CanStandUp();
        CameraFollow();
        PlayerReach();
    }

    private void FixedUpdate(){
        if (player.CanMove)
            Move();
        if (!player.IsGrounded)
            ApplyGravity();
    }

    private void OnCollisionStay(Collision other) {
        int layer = other.gameObject.layer;
        
        if(player.WalkableLayerMask == (player.WalkableLayerMask | (1 << layer))) {
            player.IsGrounded = true;
            player.CanJump = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        int layer = other.gameObject.layer;
        
        if(player.WalkableLayerMask == (player.WalkableLayerMask | (1 << layer))) {
            player.IsGrounded = false;
            player.CanJump = false;
        }
    }

    private void PlayerReach(){
        float raycastLength = player.Reach;
        Vector3 originRay = _camera.transform.position;

        Color rayColor = Color.red;
        player.SelectedObject = null; 
        player.SelectedObject = Physics.Raycast(originRay, _camera.transform.forward, out RaycastHit hit, raycastLength)?hit.collider.gameObject:null;
        
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
        mineable.Mine(player.Pickaxe.Damage + player.Strength, player.transform.position, objectHitPos);
        StartCoroutine(mineable.ImpactParticles());
    }

    public void ObjectMined() {
        player.AddMinedObjectToTracker();
    }

    private void CameraFollow(){
        _view.transform.position = _head.transform.position;

        if (mouseInput.magnitude < .1f) return;

        float cameraX = mouseInput.x * Time.deltaTime * sensitivity;
        float cameraY = mouseInput.y * Time.deltaTime * sensitivity;

        xRotation -= cameraY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        _view.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * cameraX);
    }

    private void Move(){
        if (movementInput.magnitude < .1f)
        {
            player.IsMoving = false;
            return;
        }

        player.IsMoving = true;

        Vector3 newPos = new Vector3(movementInput.x, 0, movementInput.y);

        if (player.IsGrounded)
        {
            newPos *= player.IsCrouching ? player.CrouchMultiplier : 1;
            newPos *= player.IsSprinting ? player.SprintMultiplier : 1;
        }

        rbody.MovePosition(transform.position + transform.rotation * newPos * Time.deltaTime * player.MovementSpeed);
    }

    private void Jump(){
        if (!player.CanJump || player.IsCrouching || !player.IsGrounded || player.Stamina < player.JumpStaminaLoss) return;
        
        Vector3 jumpTranslationForce = new Vector3(movementInput.x, 1, movementInput.y);
        jumpTranslationForce.y *= player.JumpForce * (player.IsSprinting ? player.SprintJumpForce : 1);

        rbody.AddForce(transform.rotation * jumpTranslationForce, ForceMode.Impulse);
        player.ReduceStamina(player.JumpStaminaLoss);
    }

    private void ApplyGravity()
    {
        if (rbody.velocity.y > 0) return;

        rbody.AddForce(new Vector3(0f, -gravityMultiplier, 0f), ForceMode.Force);
    }


    #region  Sprint
    private void StartSprinting(){
        if (player.CanSprint) 
            StartCoroutine(Sprint());
    }

    private void StopSprinting(){
        if (player.IsSprinting)
            player.IsSprinting = false;
    }

    private IEnumerator Sprint(){
        player.IsSprinting = true;
        while(player.IsSprinting && player.CanSprint){
            if (player.IsGrounded)
                player.ReduceStamina(player.SprintStaminaLoss * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        player.IsSprinting = false;
    }
    #endregion


    #region Crouch
    private void ToggleCrouch(){
        if (player.IsCrouching)
            StopCrouching();
        else
            StartCrouching();
    }

    private void StartCrouching(){
        playerBody.SetActive(false);
        playerCrouchBody.SetActive(true);
        player.IsCrouching = true;
    }

    private void StopCrouching(){
        if (player.CanStand){
            playerBody.SetActive(true);
            playerCrouchBody.SetActive(false);
            player.IsCrouching = false;
        }
    }

    private void CanStandUp(){
        float raycastLength = playerBody.GetComponent<CapsuleCollider>().height / 2 + .1f;
        float originRayY = transform.position.y + playerBody.GetComponent<CapsuleCollider>().height/2;
        float destinationRayY = transform.position.y + raycastLength;
        
        Vector3 origin = new Vector3(transform.position.x, originRayY, transform.position.z);
        Vector3 destination = new Vector3(transform.position.x, destinationRayY, transform.position.z);

        player.CanStand = !Physics.Raycast(origin, Vector3.up, out RaycastHit hit2, raycastLength);

        Color rayColor;
        if (!player.CanStand)
            rayColor = Color.red;
        else
            rayColor = Color.green;
        Debug.DrawRay(origin, Vector3.up * Vector3.Distance(origin, destination), rayColor);
    }
    #endregion
}
