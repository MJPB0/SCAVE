using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 _playerScale;
    private Vector3 _playerCrouchScale = new Vector3(1f, .5f, 1f);

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
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _isCrouching = false;
    [SerializeField] private bool _isGrounded = false;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canStand = true;
    
    [Space]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _crouchMovementSpeed = 3f;
    [SerializeField] private float _jumpForce = 2f;
    [SerializeField] private float _crouchSpeed = 5f;
    [SerializeField] private bool _isCrouchingInProccess = false;

    [Space]
    [SerializeField] private LayerMask _walkableLayerMask;

    private void Awake() {
        _rbody = GetComponent<Rigidbody>();

        _controls = new PlayerActions();

        _controls.Gameplay.Enable();

        _controls.Gameplay.Movement.performed += ctx => _move = ctx.ReadValue<Vector2>();
        _controls.Gameplay.Movement.canceled += cts => _move = Vector2.zero;

        _controls.Gameplay.Jump.performed += ctx => Jump();

        _controls.Gameplay.Duck.performed += ctx => ToggleCrouch();
    }

    private void Start()
    {        
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        _playerScale = _body.transform.localScale;
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
        if (!_isCrouchingInProccess)
            StartCoroutine(Crouch());
    }

    private IEnumerator Crouch(){
        _canMove = false;
        _isCrouchingInProccess = true;
        while(_body.transform.localScale.y - _playerCrouchScale.y >= .05f){
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y - Time.deltaTime*_crouchSpeed, _body.transform.localScale.z);

            yield return new WaitForFixedUpdate();
        }

        _body.transform.localScale = _playerCrouchScale;
        _isCrouching = true;
        _canMove = true;
        _isCrouchingInProccess = false;
    }

    private void StopCrouching(){
        if (!_isCrouchingInProccess && _canStand)
            StartCoroutine(StandUp());
    }

    private IEnumerator StandUp(){
        yield return new WaitUntil(() => _canStand);
        _canMove = false;
        _isCrouchingInProccess = true;
        while(_playerScale.y - _body.transform.localScale.y >= .05f){
            _body.transform.localScale = new Vector3(_body.transform.localScale.x, _body.transform.localScale.y + Time.deltaTime * _crouchSpeed, _body.transform.localScale.z);

            yield return new WaitForFixedUpdate();
        }
        
        _body.transform.localScale = _playerScale;
        _isCrouching = false;
        _canMove = true;
        _isCrouchingInProccess = false;
    }

    private void CanStandUp(){
        float raycastLength = _playerCollider.height/2 + .1f;
        Color rayColor;
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, raycastLength)){
            _canStand = false;
            rayColor = Color.red;
        }
        else{
            _canStand = true;
            rayColor = Color.green;
        }
        Debug.DrawRay(transform.position, Vector3.up * raycastLength, rayColor);
    }
}
