using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerActions Controls {get; private set;}

    private GameObject objectToHit;
    private Vector3 objectHitPos;

    private float playerHeight;
    private float playerRadius;
    
    private Vector3 playerScale;
    private Vector3 playerCrouchScale = new Vector3(1f, .6f, 1f);

    private Rigidbody rbody;
    private CapsuleCollider playerCollider;
    private Player player;

    private Vector2 movementInput;
    private Vector2 mouseInput;
    
    [Header("Reach")]
    //Position in which player reach ray hit a mineable object's collider
    [SerializeField] private Vector3 reachHitPos;

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

    private void Awake() {
        rbody = GetComponent<Rigidbody>();
        playerCollider = _body.GetComponent<CapsuleCollider>();
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
    }

    private void Start()
    {        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerScale = _body.transform.localScale;
        playerHeight = playerCollider.height;
        playerRadius = playerCollider.radius;
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
        if (!Physics.Raycast(originRay, _camera.transform.forward, out RaycastHit hit, raycastLength))
            player.SelectedObject = null; 
        else if(hit.collider.gameObject.CompareTag(player.MINEABLE_TAG)) {
            player.SelectedObject = hit.collider.gameObject;
            reachHitPos = hit.point;
            player.SelectedObject.GetComponent<MineableObject>().IsMineable(player.Pickaxe.Tier);
            rayColor = Color.green;
        }
        else if (hit.collider.gameObject.CompareTag(player.PICKABLE_TAG)){
            player.SelectedObject = hit.collider.gameObject;
            StartCoroutine(ItemOutline(player.SelectedObject));
            rayColor = Color.green;
        }
        else
            player.SelectedObject = null; 

        Debug.DrawRay(originRay, _camera.transform.forward * raycastLength, rayColor);
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
            Destroy(item.gameObject);
        }
        else
            Debug.Log($"Can't pick up {gameObject.name}!");
    }

    private void SwingPickaxe(){
        if (!player.CanSwing) return;

        player.CanSwing = false;
        player.IsSwinging = true;
        player.ReduceStamina(player.SwingStaminaLoss);

        if (!player.SelectedObject || !player.SelectedObject.CompareTag(player.MINEABLE_TAG)) {
            player.Pickaxe.SwingPickaxe(false);
            return;
        }

        objectToHit = player.SelectedObject;
        objectHitPos = reachHitPos;
        player.Pickaxe.SwingPickaxe(true);
    }

    public void PickaxeHit(){
        if (player.SelectedObject){
            objectHitPos = reachHitPos;
            objectToHit = player.SelectedObject;
        }
            
        objectToHit.GetComponent<MineableObject>().Mine(player.Pickaxe.Damage + player.Strength, player.transform.position, objectHitPos);
    }

    public void PickaxeUnstuck(){
        
    }

    public void SwingEnded(){
        player.IsSwinging = false;
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
        if (movementInput.magnitude < .1f) player.IsMoving = false;
        if (!player.IsGrounded || player.ChangingPositionInProgress || movementInput.magnitude < .1f) return;

        player.IsMoving = true;

        Vector3 newPos = new Vector3(movementInput.x, 0, movementInput.y);
        newPos *= player.IsCrouching? player.CrouchMultiplier : 1;
        newPos *= player.IsSprinting? player.SprintMultiplier : 1;

        rbody.MovePosition(transform.position + transform.rotation * newPos * Time.deltaTime * player.MovementSpeed);
    }

    private void Jump(){
        if (!player.CanJump || !player.IsGrounded) return;
        
        Vector3 jumpTranslationForce = new Vector3(movementInput.x, 0, movementInput.y);
        jumpTranslationForce *= player.IsCrouching? player.CrouchMultiplier : 1;
        jumpTranslationForce *= player.IsSprinting? player.SprintMultiplier : 1;

        jumpTranslationForce.y = 1f;
        rbody.AddForce(transform.rotation * jumpTranslationForce * player.JumpForce);
        player.ReduceStamina(player.JumpStaminaLoss);
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
        if (!player.ChangingPositionInProgress)
            StartCoroutine(Crouch());
    }

    private IEnumerator Crouch(){
        player.ChangingPositionInProgress = true;
        while(_body.transform.localScale.y - playerCrouchScale.y >= .05f){
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y - Time.deltaTime * player.ChangePositionSpeed, _body.transform.localScale.z);

            yield return new WaitForEndOfFrame();
        }

        _body.transform.localScale = playerCrouchScale;
        player.IsCrouching = true;
        player.ChangingPositionInProgress = false;
    }

    private void StopCrouching(){
        if (!player.ChangingPositionInProgress && player.CanStand)
            StartCoroutine(StandUp());
    }

    private IEnumerator StandUp(){
        player.ChangingPositionInProgress = true;
        while(playerScale.y - _body.transform.localScale.y >= .05f){
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y + Time.deltaTime * player.ChangePositionSpeed, _body.transform.localScale.z);

            yield return new WaitForEndOfFrame();
        }
        
        _body.transform.localScale = playerScale;
        player.IsCrouching = false;
        player.ChangingPositionInProgress = false;
    }

    private void CanStandUp(){
        float raycastLength = playerHeight/2 + .1f;
        float originRayY = transform.position.y + playerCollider.height/2 * playerCrouchScale.y;
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
