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
    private Vector2 _move;

    [Header("Body")]
    [SerializeField] private CapsuleCollider _playerCollider;

    [Space]
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _head;
    [SerializeField] private Camera _eyes;

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

        _controls = new PlayerActions();

        _controls.Gameplay.Enable();

        _controls.Gameplay.Movement.performed += ctx => _move = ctx.ReadValue<Vector2>();
        _controls.Gameplay.Movement.canceled += cts => _move = Vector2.zero;

        _controls.Gameplay.Jump.performed += ctx => Jump();

        _controls.Gameplay.Crouch.performed += ctx => ToggleCrouch();
    }

    private void Start()
    {        
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        _playerScale = _body.transform.localScale;
        _playerHeight = _playerCollider.height;
        _playerRadius = _playerCollider.radius;
    }

    private void Update() {

    }

    private void FixedUpdate()
    {
        if (_isCrouching)
            CanStandUp();
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

    private void Move(){
        if (!_isGrounded) return;

        Vector3 translation = new Vector3(_move.x, 0, _move.y);

        gameObject.transform.Translate(translation * Time.deltaTime * (_isCrouching? _crouchMovementSpeed : _movementSpeed));
    }

    private void Jump(){
        if (!_canJump || !_isGrounded) return;
        
        Vector3 jumpTranslationForce = new Vector3(_move.x * _jumpForce, _jumpForce, _move.y * _jumpForce);

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
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y - Time.deltaTime*_changePositionSpeed, _body.transform.localScale.z);

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
