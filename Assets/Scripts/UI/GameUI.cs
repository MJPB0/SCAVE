using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Sliders")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Slider _swingTimerSlider;
    
    [Header("Pickaxe")]
    [SerializeField] private Text _pickaxeTier;
    [SerializeField] private Text _pickaxeDamage;
    [SerializeField] private Image _pickaxeIcon;

    [Header("Inventory")]
    [SerializeField] private bool isInventoryVisible = false;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject pickaxePanel;

    [Space]
    [SerializeField] private Text _ironWeight;
    [SerializeField] private Text _goldWeight;
    [SerializeField] private Text _coalWeight;
    [SerializeField] private Text _emeraldWeight;

    [Space]
    [SerializeField] private Text _healthValue;
    [SerializeField] private Text _healthRegenerationValue;
    [SerializeField] private Text _staminaValue;
    [SerializeField] private Text _staminaRegenerationValue;
    [SerializeField] private Text _strengthValue;
    [SerializeField] private Text _reachValue;
    [SerializeField] private Text _movementSpeedValue;
    [SerializeField] private Text _jumpForceValue;

    [Header("Console")]
    [SerializeField] private Text _logs;

    [Header("Debug")]
    [SerializeField] private bool showDebug;
    [SerializeField] private Text _selectedObject;
    [SerializeField] private Text _canSwing;
    [SerializeField] private Text _isSwinging;
    [SerializeField] private Text _canStand;
    [SerializeField] private Text _isCrouching;
    [SerializeField] private Text _canSprint;
    [SerializeField] private Text _isSprinting;
    [SerializeField] private Text _canJump;
    [SerializeField] private Text _isGrounded;
    [SerializeField] private Text _canMove;
    [SerializeField] private Text _isMoving;
    [SerializeField] private Text _isChangingPosition;
    [SerializeField] private Text _isRegeneratingHealth;
    [SerializeField] private Text _isRegeneratingStamina;

    private void Start() {
        PlayerController.OnPlayerHealthLost += UpdateHealthSlider;
        PlayerController.OnPlayerHealthRestored += UpdateHealthSlider;

        PlayerController.OnPlayerStaminaLost += UpdateStaminaSlider;
        PlayerController.OnPlayerStaminaRestored += UpdateStaminaSlider;

        PlayerController.OnSwingTimeChanged += UpdateSwingTimeSlider;

        PlayerController.OnPickaxeChange += UpdatePickaxeInfo;

        PlayerController.OnInventoryToggle += ToggleInventory;

        SlidersSetup();
        UpdateSwingTimeSlider();
        UpdateHealthSlider();
        UpdateStaminaSlider();
    }

    private void Update() {
        if(showDebug)
            PlayerDebug();
        if (isInventoryVisible)
            UpdatePlayerInfo();
    }

    private void ToggleInventory(){
        if (isInventoryVisible){
            inventoryPanel.SetActive(false);
            pickaxePanel.SetActive(false);
            isInventoryVisible = false;
        }else{
            inventoryPanel.SetActive(true);
            pickaxePanel.SetActive(true);
            isInventoryVisible = true;
        }
    }

    private void PlayerDebug(){
        _selectedObject.text = $"selected object: {player.SelectedObject}";
        _canSwing.text = $"can swing: {player.CanSwing}";
        _canStand.text = $"can stand: {player.CanStand}";
        _isCrouching.text = $"is crouching: {player.IsCrouching}";
        _canSprint.text = $"can sprint: {player.CanSprint}";
        _isSprinting.text = $"is sprinting: {player.IsSprinting}";
        _canJump.text = $"can jump: {player.CanJump}";
        _isGrounded.text = $"is grounded: {player.IsGrounded}";
        _canMove.text = $"can move: {player.CanMove}";
        _isMoving.text = $"is moving: {player.IsMoving}";
        _isRegeneratingHealth.text = $"is regenerating health: {player.IsRegeneratingHealth}";
        _isRegeneratingStamina.text = $"is regenerating stamina: {player.IsRegeneratingStamina}";
    }

    private void UpdatePlayerInfo(){
        Dictionary<int, float> inv = player.Inventory;

        //todo restructure this when game manager is introduced
        _coalWeight.text = $"{(inv.ContainsKey(0)?inv[0]:0).ToString("F2")}kg";
        _ironWeight.text = $"{(inv.ContainsKey(1)?inv[1]:0).ToString("F2")}kg";
        _goldWeight.text = $"{(inv.ContainsKey(2)?inv[2]:0).ToString("F2")}kg";
        _emeraldWeight.text = $"{(inv.ContainsKey(3)?inv[3]:0).ToString("F2")}kg";

        _healthValue.text = $"{player.BaseHealth} + {player.MaxHealth - player.BaseHealth} - {player.MaxHealth}";
        _healthRegenerationValue.text = $"{player.BaseHealthRegenerationRate} + {player.HealthRegenerationRate - player.BaseHealthRegenerationRate} - {player.HealthRegenerationRate}";
        _staminaValue.text = $"{player.BaseStamina} + {player.MaxStamina - player.BaseStamina} - {player.MaxStamina}";
        _staminaRegenerationValue.text = $"{player.BaseStaminaRegenerationRate} + {player.StaminaRegenerationRate - player.BaseStaminaRegenerationRate} - {player.StaminaRegenerationRate}";
        _strengthValue.text = $"{player.BaseStrength} + {player.Strength - player.BaseStrength} - {player.Strength}";
        _reachValue.text = $"{player.BaseReach} + {player.Reach - player.BaseReach} - {player.Reach}";
        _movementSpeedValue.text = $"{player.BaseMovementSpeed} + {player.MovementSpeed - player.BaseMovementSpeed} - {player.MovementSpeed}";
        _jumpForceValue.text = $"{player.BaseJumpForce} + {player.JumpForce - player.BaseJumpForce} - {player.JumpForce}";
    }

    private void SlidersSetup(){
        _healthSlider.minValue = 0f;
        _healthSlider.wholeNumbers = false;
        _healthSlider.interactable = false;
        
        _staminaSlider.minValue = 0f;
        _staminaSlider.wholeNumbers = false;
        _staminaSlider.interactable = false;
        
        _swingTimerSlider.minValue = 0f;
        _swingTimerSlider.wholeNumbers = false;
        _swingTimerSlider.interactable = false;
    }

    private void UpdateHealthSlider(){
        _healthSlider.maxValue = player.MaxHealth;
        _healthSlider.value = player.Health;
    }

    private void UpdateStaminaSlider(){
        _staminaSlider.maxValue = player.MaxStamina;
        _staminaSlider.value = player.Stamina;
    }

    private void UpdateSwingTimeSlider(){
        _swingTimerSlider.maxValue = player.TimeBetweenSwings;
        _swingTimerSlider.value = player.TimeToNextSwing;
    }

    private void UpdatePickaxeInfo(){
        _pickaxeDamage.text = $"Damage: {player.Pickaxe.Damage}";
        _pickaxeTier.text = $"Tier: {player.Pickaxe.Tier}";
        _pickaxeIcon.sprite = player.Pickaxe.Icon;
    }

    public void AddLog(string log)
    {
        _logs.text += $"\n({DateTime.Now}) {log}";
    }
}
