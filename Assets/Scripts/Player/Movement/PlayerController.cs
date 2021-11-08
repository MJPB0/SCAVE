using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float playerHeight;
    private float playerRadius;
    private Vector3 playerScale;
    private Vector3 playerCrouchScale = new Vector3(1f, .6f, 1f);

    private Rigidbody rbody;
    private PlayerActions controls;
    private Vector2 movementInput;
    private Vector2 mouseInput;

    private CapsuleCollider playerCollider;

    [Header("Body")]
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _head;

    [Header("Camera")]
    [SerializeField] private float sensitivity = 20f;
    [SerializeField] private float xRotation = 0f;
    [SerializeField] private Camera _camera;

    [Header("Movement")]
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool canMove;
    [SerializeField] private bool canJump;
    [SerializeField] private bool canStand;
    
    [Space]
    [SerializeField] private float jumpForce = 450f;

    [Space]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float crouchMovementSpeed = 2f;
    [SerializeField] private float changePositionSpeed = 1f;
    [SerializeField] private bool changingPositionInProgress;

    [Space]
    [SerializeField] private LayerMask walkableLayerMask;

    private void Awake() {
        rbody = GetComponent<Rigidbody>();
        playerCollider = _body.GetComponent<CapsuleCollider>();

        controls = new PlayerActions();

        controls.Gameplay.Enable();

        controls.Gameplay.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Movement.canceled += cts => movementInput = Vector2.zero;

        controls.Gameplay.Jump.performed += ctx => Jump();

        controls.Gameplay.Crouch.performed += ctx => ToggleCrouch();

        controls.Gameplay.MouseMovement.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
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
        if (isCrouching)
            CanStandUp();
        CameraFollow();
    }

    private void FixedUpdate(){
        if (canMove)
            Move();
    }

    private void OnCollisionStay(Collision other) {
        int layer = other.gameObject.layer;
        
        if(walkableLayerMask == (walkableLayerMask | (1 << layer))) {
            isGrounded = true;
            canJump = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        int layer = other.gameObject.layer;
        
        if(walkableLayerMask == (walkableLayerMask | (1 << layer))) {
            isGrounded = false;
            canJump = false;
        }
    }

    private void CameraFollow(){
        if (mouseInput.magnitude < .1f) return;

        float cameraX = mouseInput.x * Time.deltaTime * sensitivity;
        float cameraY = mouseInput.y * Time.deltaTime * sensitivity;

        xRotation -= cameraY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        _head.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * cameraX);
    }

    private void Move(){
        if (!isGrounded || changingPositionInProgress || movementInput.magnitude < .1f) return;

        Vector3 newPos = new Vector3(movementInput.x, 0, movementInput.y);
        rbody.MovePosition(transform.position + transform.rotation * newPos * Time.deltaTime * (isCrouching? crouchMovementSpeed : movementSpeed));
    }

    private void Jump(){
        if (!canJump || !isGrounded) return;
        
        Vector3 jumpTranslationForce = new Vector3(movementInput.x, 1, movementInput.y);
        rbody.AddForce(transform.rotation * jumpTranslationForce * jumpForce);
    }

    #region Crouch
    private void ToggleCrouch(){
        if (isCrouching)
            StopCrouching();
        else
            StartCrouching();
    }

    private void StartCrouching(){
        if (!changingPositionInProgress)
            StartCoroutine(Crouch());
    }

    private IEnumerator Crouch(){
        changingPositionInProgress = true;
        while(_body.transform.localScale.y - playerCrouchScale.y >= .05f){
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y - Time.deltaTime * changePositionSpeed, _body.transform.localScale.z);

            yield return new WaitForFixedUpdate();
        }

        _body.transform.localScale = playerCrouchScale;
        isCrouching = true;
        changingPositionInProgress = false;
    }

    private void StopCrouching(){
        if (!changingPositionInProgress && canStand)
            StartCoroutine(StandUp());
    }

    private IEnumerator StandUp(){
        changingPositionInProgress = true;
        while(playerScale.y - _body.transform.localScale.y >= .05f){
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y + Time.deltaTime * changePositionSpeed, _body.transform.localScale.z);

            yield return new WaitForFixedUpdate();
        }
        
        _body.transform.localScale = playerScale;
        isCrouching = false;
        changingPositionInProgress = false;
    }

    private void CanStandUp(){
        float raycastLength = playerHeight/2 + .1f;
        float originRayY = transform.position.y + playerCollider.height/2 * playerCrouchScale.y;
        float destinationRayY = transform.position.y + raycastLength;
        
        Vector3 origin = new Vector3(transform.position.x, originRayY, transform.position.z);
        Vector3 destination = new Vector3(transform.position.x, destinationRayY, transform.position.z);

        canStand = !Physics.Raycast(origin, Vector3.up, out RaycastHit hit2, raycastLength);

        Color rayColor;
        if (!canStand)
            rayColor = Color.red;
        else
            rayColor = Color.green;
        Debug.DrawRay(origin, Vector3.up * Vector3.Distance(origin, destination), rayColor);
    }
    #endregion
}
