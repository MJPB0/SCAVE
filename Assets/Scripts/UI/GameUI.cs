using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    [SerializeField] private Player player;

    [Header("Sliders")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Slider _swingTimerSlider;

    [Header("Pickaxe")]
    [SerializeField] private Text _pickaxeTier;
    [SerializeField] private Text _pickaxeDamage;
    [SerializeField] private Image _pickaxeIcon;

    [Header("Interaction")]
    [SerializeField] private Text _interactionText;
    [SerializeField] private Text _interactionTargetText;
    [SerializeField] private Slider _interactionHealthSlider;
    [SerializeField] private GameObject interactionPanel;

    [Header("Inventory")]
    [SerializeField] private bool isInventoryVisible = false;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject pickaxePanel;

    [Space]
    [SerializeField] private Text _healthValue;
    [SerializeField] private Text _healthRegenerationValue;
    [SerializeField] private Text _staminaValue;
    [SerializeField] private Text _staminaRegenerationValue;
    [SerializeField] private Text _strengthValue;
    [SerializeField] private Text _reachValue;
    [SerializeField] private Text _movementSpeedValue;
    [SerializeField] private Text _jumpForceValue;

    private const string TAP_INTERACT_TEXT = "Press 'E'";
    private const string HOLD_INTERACT_TEXT = "Hold 'E'";

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
        if (isInventoryVisible)
            UpdatePlayerInfo();

        CanPlayerInteract();
    }

    private void ToggleInventory() {
        if (isInventoryVisible) {
            inventoryPanel.SetActive(false);
            pickaxePanel.SetActive(false);
            isInventoryVisible = false;
        } else {
            inventoryPanel.SetActive(true);
            pickaxePanel.SetActive(true);
            isInventoryVisible = true;
        }
    }

    private void CanPlayerInteract() {
        if (!player.SelectedObject) {
            interactionPanel.SetActive(false);
            return;
        }

        interactionPanel.SetActive(true);
        _interactionHealthSlider.gameObject.SetActive(false);
        _interactionText.gameObject.SetActive(false);
        _interactionTargetText.gameObject.SetActive(true);

        bool requiresHoldInteraction = false;
        bool canInteract = false;

        string interactionAdditionalText = "";
        string interactionTargetText = ShouldDisplaySelectedObjectName(player.SelectedObject) ? player.SelectedObject.name.Split("(")[0] : "";

        if (player.SelectedObject.CompareTag(Tags.PICKABLE_TAG) && player.SelectedObject.TryGetComponent(out Item item)) {
            requiresHoldInteraction = item.RequireHoldInteraction;
            canInteract = item.IsPickable;

            interactionTargetText = item.DisplayName;
            interactionAdditionalText = "to pick up";
        } else if (player.SelectedObject.CompareTag(Tags.INTERACTABLE_TAG) && player.SelectedObject.TryGetComponent(out Interactable interactable)) {
            requiresHoldInteraction = interactable.RequireHoldInteraction;
            canInteract = interactable.CanInteract();

            interactionTargetText = interactable.DisplayName;
            interactionAdditionalText = "to interact";
        } else if (player.SelectedObject.CompareTag(Tags.MINEABLE_TAG) && player.SelectedObject.TryGetComponent(out MineableObject mineable)) {
            _interactionHealthSlider.maxValue = mineable.MaxHealth;
            _interactionHealthSlider.value = mineable.Health;
            _interactionHealthSlider.gameObject.SetActive(true);
        }

        _interactionTargetText.text = interactionTargetText;

        string text = requiresHoldInteraction ? HOLD_INTERACT_TEXT : TAP_INTERACT_TEXT;
        _interactionText.text = $"{text} {interactionAdditionalText}";
        _interactionText.gameObject.SetActive(canInteract);
    }

    private bool ShouldDisplaySelectedObjectName(GameObject selectedObject) => selectedObject.CompareTag(Tags.PICKABLE_TAG) || selectedObject.CompareTag(Tags.INTERACTABLE_TAG) || selectedObject.CompareTag(Tags.MINEABLE_TAG);

    private void UpdatePlayerInfo() {
        _healthValue.text = $"{player.BaseHealth} + {player.MaxHealth - player.BaseHealth} - {player.MaxHealth}";
        _healthRegenerationValue.text = $"{player.BaseHealthRegenerationRate} + {player.HealthRegenerationRate - player.BaseHealthRegenerationRate} - {player.HealthRegenerationRate}";
        _staminaValue.text = $"{player.BaseStamina} + {player.MaxStamina - player.BaseStamina} - {player.MaxStamina}";
        _staminaRegenerationValue.text = $"{player.BaseStaminaRegenerationRate} + {player.StaminaRegenerationRate - player.BaseStaminaRegenerationRate} - {player.StaminaRegenerationRate}";
        _strengthValue.text = $"{player.BaseStrength} + {player.Strength - player.BaseStrength} - {player.Strength}";
        _reachValue.text = $"{player.BaseReach} + {player.Reach - player.BaseReach} - {player.Reach}";
        _movementSpeedValue.text = $"{player.BaseMovementSpeed} + {player.MovementSpeed - player.BaseMovementSpeed} - {player.MovementSpeed}";
        _jumpForceValue.text = $"{player.BaseJumpForce} + {player.JumpForce - player.BaseJumpForce} - {player.JumpForce}";
    }

    private void SlidersSetup() {
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

    private void UpdateHealthSlider() {
        _healthSlider.maxValue = player.MaxHealth;
        _healthSlider.value = player.Health;
    }

    private void UpdateStaminaSlider() {
        _staminaSlider.maxValue = player.MaxStamina;
        _staminaSlider.value = player.Stamina;

        if (_staminaSlider.value == _staminaSlider.maxValue) {
            _staminaSlider.gameObject.SetActive(false);
        } else {
            _staminaSlider.gameObject.SetActive(true);
        }
    }

    private void UpdateSwingTimeSlider() {
        _swingTimerSlider.maxValue = player.TimeBetweenSwings;
        _swingTimerSlider.value = player.TimeToNextSwing;

        if (_swingTimerSlider.value == _swingTimerSlider.maxValue) {
            _swingTimerSlider.gameObject.SetActive(false);
        } else {
            _swingTimerSlider.gameObject.SetActive(true);
        }
    }

    private void UpdatePickaxeInfo() {
        _pickaxeDamage.text = $"Damage: {player.Pickaxe.Damage}";
        _pickaxeTier.text = $"Tier: {player.Pickaxe.Tier}";
        _pickaxeIcon.sprite = player.Pickaxe.Icon;
    }
}
