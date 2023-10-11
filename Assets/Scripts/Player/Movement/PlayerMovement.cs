using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

public class PlayerMovement : MonoBehaviour {
    private Rigidbody rbody;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private GameObject playerCrouchBody;

    private Player player;

    public Vector2 MovementInput { get; set; }
    public Vector2 MouseInput { get; set; }

    private float playerHeight;
    private RaycastHit slopeHit;
    private Vector3 moveDirection;

    [Header("Camera")]
    [SerializeField] private float sensitivity = 20f;
    [SerializeField] private float xRotation = 0f;
    [SerializeField] private GameObject _view;
    [SerializeField] private Camera _camera;

    [Header("Gravity")]
    [SerializeField] private float gravityMultiplier = 1.75f;

    [Header("Body")]
    [SerializeField] private GameObject _head;

    private void Awake() {
        player = GetComponent<Player>();
        rbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        UpdatePlayerHeight();
    }


    private void Update() {
        if (player.IsCrouching)
            CanStandUp();
        IsGrounded();
        LimitSpeed();
    }

    private void FixedUpdate() {
        CameraFollow();
        if (player.CanMove)
            Move();
        if (!player.IsGrounded)
            ApplyGravity();
    }

    private void UpdatePlayerHeight() {
        playerHeight = (player.IsCrouching ? playerCrouchBody : playerBody).GetComponent<CapsuleCollider>().height;
    }

    private void IsGrounded() {
        bool isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.2f,
            player.WalkableLayerMask
        );
        player.IsGrounded = isGrounded;

        if (isGrounded) {
            player.IsGrounded = true;
            player.CanJump = true;
            rbody.drag = player.GroundDrag;
        } else {
            player.IsGrounded = false;
            player.CanJump = false;
            rbody.drag = 0f;
        }
    }

    private void CameraFollow() {
        _view.transform.position = _head.transform.position;

        if (MouseInput.magnitude < .1f) return;

        float cameraX = MouseInput.x * Time.deltaTime * sensitivity;
        float cameraY = MouseInput.y * Time.deltaTime * sensitivity;

        xRotation -= cameraY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        _view.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * cameraX);
    }

    private void Move() {
        if (MovementInput.magnitude < .1f) {
            player.IsMoving = false;
            return;
        }
        player.IsMoving = true;

        moveDirection = transform.forward * MovementInput.y + transform.right * MovementInput.x;

        Vector3 moveForce = moveDirection.normalized * player.MovementSpeed * player.MovementSpeedForceMultiplier;

        if (player.IsGrounded) {
            moveForce *= player.IsCrouching ? player.CrouchMultiplier : 1;
            moveForce *= player.IsSprinting ? player.SprintMultiplier : 1;
        }

        rbody.AddForce(moveForce, ForceMode.Force);
    }

    private void LimitSpeed() {
        Vector3 flatVel = new Vector3(rbody.velocity.x, 0f, rbody.velocity.z);

        if (flatVel.magnitude > player.MovementSpeed) {
            Vector3 limitedVel = flatVel.normalized * player.MovementSpeed;
            rbody.velocity = new Vector3(limitedVel.x, rbody.velocity.y, limitedVel.z);
        }
    }

    public void Jump() {
        if (!player.CanJump || player.IsCrouching || !player.IsGrounded || player.Stamina < player.JumpStaminaLoss) return;

        Vector3 jumpTranslationForce = new Vector3(MovementInput.x, 1, MovementInput.y);
        jumpTranslationForce.y *= player.JumpForce * (player.IsSprinting ? player.SprintJumpForce : 1);

        rbody.AddForce(transform.rotation * jumpTranslationForce, ForceMode.Impulse);
        player.ReduceStamina(player.JumpStaminaLoss);
    }

    private void ApplyGravity() {
        if (rbody.velocity.y > 0) return;

        rbody.AddForce(new Vector3(0f, -gravityMultiplier, 0f), ForceMode.Force);
    }


    public void StartSprinting() {
        if (player.CanSprint)
            StartCoroutine(Sprint());
    }

    public void StopSprinting() {
        if (player.IsSprinting)
            player.IsSprinting = false;
    }

    private IEnumerator Sprint() {
        player.IsSprinting = true;
        while (player.IsSprinting && player.CanSprint) {
            if (player.IsGrounded)
                player.ReduceStamina(player.SprintStaminaLoss * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        player.IsSprinting = false;
    }


    public void ToggleCrouch() {
        if (player.IsCrouching)
            StopCrouching();
        else
            StartCrouching();

        UpdatePlayerHeight();
    }

    private void StartCrouching() {
        playerBody.SetActive(false);
        playerCrouchBody.SetActive(true);
        player.IsCrouching = true;

        rbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    private void StopCrouching() {
        if (player.CanStand) {
            playerBody.SetActive(true);
            playerCrouchBody.SetActive(false);
            player.IsCrouching = false;
        }
    }

    private void CanStandUp() {
        float raycastLength = playerBody.GetComponent<CapsuleCollider>().height / 2 + .1f;
        float originRayY = transform.position.y + playerBody.GetComponent<CapsuleCollider>().height / 2;
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
}