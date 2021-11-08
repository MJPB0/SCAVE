using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float _playerHeight;
    private float _playerRadius;
    private Vector3 _playerScale;
    private Vector3 _playerCrouchScale = new Vector3(1f, .6f, 1f);

    private Rigidbody _rbody;
    private PlayerActions _controls;
    private Vector2 _movementInput;
    private Vector2 _mouseInput;

    private CapsuleCollider _playerCollider;

    [Header("Body")]
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _head;

    [Header("Camera")]
    [SerializeField] private float _sensitivity = 100f;
    [SerializeField] private float _xRotation = 0f;
    [SerializeField] private Camera _camera;

    [Header("Movement")]
    [SerializeField] private bool _isCrouching = false;
    [SerializeField] private bool _isGrounded = false;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canStand = true;
    
    [Space]
    [SerializeField] private float _jumpForce = 2f;

    [Space]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _crouchMovementSpeed = 3f;
    [SerializeField] private float _changePositionSpeed = 5f;
    [SerializeField] private bool _changingPositionInProgress = false;

    [Space]
    [SerializeField] private LayerMask _walkableLayerMask;

    private void Awake() {
        _rbody = GetComponent<Rigidbody>();
        _playerCollider = _body.GetComponent<CapsuleCollider>();

        _controls = new PlayerActions();

        _controls.Gameplay.Enable();

        _controls.Gameplay.Movement.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
        _controls.Gameplay.Movement.canceled += cts => _movementInput = Vector2.zero;

        _controls.Gameplay.Jump.performed += ctx => Jump();

        _controls.Gameplay.Crouch.performed += ctx => ToggleCrouch();

        _controls.Gameplay.MouseMovement.performed += ctx => _mouseInput = ctx.ReadValue<Vector2>();
    }

    private void Start()
    {        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _playerScale = _body.transform.localScale;
        _playerHeight = _playerCollider.height;
        _playerRadius = _playerCollider.radius;
    }

    private void Update() {
        if (_isCrouching)
            CanStandUp();
        CameraFollow();
    }

    private void FixedUpdate(){
        if (_canMove)
            Move();
    }

    private void OnCollisionEnter(Collision other) {
        int layer = other.gameObject.layer;
        if (_walkableLayerMask == (_walkableLayerMask | (1 << layer))) {
            _isGrounded = true;
            _canJump = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        int layer = other.gameObject.layer;
        if (_walkableLayerMask == (_walkableLayerMask | (1 << layer))) 
            _isGrounded = false;
    }

    private void CameraFollow(){
        if (_mouseInput.magnitude < .1f) return;

        float cameraX = _mouseInput.x * Time.deltaTime * _sensitivity;
        float cameraY = _mouseInput.y * Time.deltaTime * _sensitivity;

        _xRotation -= cameraY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _head.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up * cameraX);
    }

    private void Move(){
        if (!_isGrounded) return;

        Vector3 translation = new Vector3(_movementInput.x, 0, _movementInput.y);

        gameObject.transform.Translate(translation * Time.deltaTime * (_isCrouching? _crouchMovementSpeed : _movementSpeed));
    }

    private void Jump(){
        if (!_canJump || !_isGrounded) return;
        
        Vector3 jumpTranslationForce = new Vector3(_movementInput.x * _jumpForce, _jumpForce, _movementInput.y * _jumpForce);

        _rbody.AddForce(jumpTranslationForce);
        _canJump = false;
    }

    private void ToggleCrouch(){
        if (_isCrouching)
            StopCrouching();
        else
            StartCrouching();
    }

    private void StartCrouching(){
        if (!_changingPositionInProgress)
            StartCoroutine(Crouch());
    }

    private IEnumerator Crouch(){
        _canMove = false;
        _changingPositionInProgress = true;
        while(_body.transform.localScale.y - _playerCrouchScale.y >= .05f){
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y - Time.deltaTime * _changePositionSpeed, _body.transform.localScale.z);

            yield return new WaitForFixedUpdate();
        }

        _body.transform.localScale = _playerCrouchScale;
        _isCrouching = true;
        _canMove = true;
        _changingPositionInProgress = false;
    }

    private void StopCrouching(){
        if (!_changingPositionInProgress && _canStand)
            StartCoroutine(StandUp());
    }

    private IEnumerator StandUp(){
        yield return new WaitUntil(() => _canStand);
        _canMove = false;
        _changingPositionInProgress = true;
        while(_playerScale.y - _body.transform.localScale.y >= .05f){
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y + Time.deltaTime * _changePositionSpeed, _body.transform.localScale.z);

            yield return new WaitForFixedUpdate();
        }
        
        _body.transform.localScale = _playerScale;
        _isCrouching = false;
        _canMove = true;
        _changingPositionInProgress = false;
    }

    private void CanStandUp(){
        float raycastLength = _playerHeight/2 + .1f;
        float originRayY = transform.position.y + _playerCollider.height/2 * _playerCrouchScale.y;
        float destinationRayY = transform.position.y + raycastLength;
        
        Vector3 origin = new Vector3(transform.position.x, originRayY, transform.position.z);
        Vector3 destination = new Vector3(transform.position.x, destinationRayY, transform.position.z);

        _canStand = !Physics.Raycast(origin, Vector3.up, out RaycastHit hit2, raycastLength);

        Color rayColor;
        if (!_canStand)
            rayColor = Color.red;
        else
            rayColor = Color.green;
        Debug.DrawRay(origin, Vector3.up * Vector3.Distance(origin, destination), rayColor);
    }
}
