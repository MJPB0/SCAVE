using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string MINEABLE_TAG {get;} = "Mineable";
    public string PICKABLE_TAG {get;} = "Pickable";
    
    //ItemId, amount
    private Dictionary<int, float> inventory;

    private Dictionary<string, int> minedTracker;

    public GameObject SelectedObject;

    [Header("Mining")]
    public bool CanSwing = true;
    [SerializeField] private PlayerPickaxe playerPickaxe;
    [SerializeField] private PickaxeScriptableObject startingPickaxe;

    [Space]
    [SerializeField] private float timeBetweenSwings = 3f;
    public float TimeToNextSwing = 3f;
    [SerializeField] private float timeBetweenSwingsModifier = 0f;
    
    [Header("Swing strength")]
    [SerializeField] private float defaultStrength = 15f;
    [SerializeField] private float strength = 15f;
    [SerializeField] private float strengthModifier = 0f;

    [Header("Pickaxe Swing speed")]
    [SerializeField] private float defaultSwingSpeed = 2f;
    [SerializeField] private float swingSpeed = 2f;
    [SerializeField] private float swingSpeedModifier = 0f;

    [Header("Pickaxe Stamina loss")]
    [SerializeField] private float defaultSwingStaminaLoss = 10f;
    [SerializeField] private float swingStaminaLoss = 10f;
    [SerializeField] private float swingStaminaLossModifier = 0f;

    [Header("Score")]
    public float CurrentScore = 0f;

    [Header("Player health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 100f;
    [SerializeField] private float healthModifier = 0f;
    
    [Space]
    [SerializeField] private float healthRegeneration = 5f;
    [SerializeField] private float healthRegenerationModifier = 0f;

    [Header("Player stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float staminaModifier = 0f;

    [Space]
    [SerializeField] private float staminaRegeneration = 15f;
    [SerializeField] private float staminaRegenerationModifier = 0f;

    [Header("Perks")]
    [SerializeField] private int defaultPerksMaxAmount = 5;
    [SerializeField] private int perksMaxAmount = 5;
    [SerializeField] private int perksMaxAmountModifier = 0;
    public List<Perk> PlayerPerks;

    [Header("Regeneration")]
    public bool IsRegeneratingStamina;
    public bool IsRegeneratingHealth;
    
    
    [Header("Reach")]
    [SerializeField] private float defaultReach = 3f;
    [SerializeField] private float reach = 3f;
    [SerializeField] private float reachModifier = 0f;

    [Header("Movement")]
    public bool IsCrouching;
    public bool IsSprinting;
    public bool IsGrounded;
    public bool IsMoving;
    public bool ChangingPositionInProgress;
    
    [Space]
    public bool CanMove = true;
    public bool CanSprint;
    public bool CanJump;
    public bool CanStand;

    [Header("Jump")]
    [SerializeField] private float defaultJumpForce = 450f;
    [SerializeField] private float jumpForce = 450f;
    [SerializeField] private float jumpForceModifier = 0f;

    [Header("Movement speeds")]
    [SerializeField] private float defaultMovementSpeed = 4f;
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float movementSpeedModifier = 0f;

    [Space]
    [SerializeField] private float defaultSprintMultiplier = 1.5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float sprintSpeedModifier = 0f;

    [Space]
    [SerializeField] private float defaultCrouchMultiplier = .5f;
    [SerializeField] private float crouchMultiplier = .5f;
    [SerializeField] private float crouchSpeedModifier = 0f;

    [Header("Change position speed")]
    [SerializeField] private float defaultChangePositionSpeed = 1f;
    [SerializeField] private float changePositionSpeed = 1f;
    [SerializeField] private float changePositionSpeedModifier = 0f;
    
    [Header("Sprint stamina loss")]
    [SerializeField] private float defaultSprintStaminaLoss = 10f;
    [SerializeField] private float sprintStaminaLoss = 10f;
    [SerializeField] private float sprintStaminaLossModifier = 0f;
    
    [Header("Jump stamina loss")]
    [SerializeField] private float defaultJumpStaminaLoss = 15f;
    [SerializeField] private float jumpStaminaLoss = 15f;
    [SerializeField] private float jumpStaminaLossModifier = 0f;

    [Space]
    public LayerMask WalkableLayerMask;

    #region  Getters
    public float Health {get {return health;}}
    public float BaseHealth {get {return maxHealth;}}
    public float MaxHealth {get {return maxHealth + healthModifier;}}

    public float Stamina {get {return stamina;}}
    public float BaseStamina {get {return maxStamina;}}
    public float MaxStamina {get {return maxStamina + staminaModifier;}}

    public float BaseStrength {get {return strength;}}
    public float Strength {get {return strength + strengthModifier;}}

    public float BaseSwingSpeed {get {return swingSpeed;}}
    public float SwingSpeed {get {return swingSpeed + swingSpeedModifier;}}
    
    public float BasePerksAmount {get {return perksMaxAmount;}}
    public int PerksMaxAmount {get {return perksMaxAmount + perksMaxAmountModifier;}}

    public float BaseJumpForce {get {return jumpForce;}}
    public float JumpForce {get {return jumpForce + jumpForceModifier;}}

    public float BaseMovementSpeed {get {return movementSpeed;}}
    public float MovementSpeed {get {return movementSpeed + movementSpeedModifier;}}

    public float SprintMultiplier {get {return sprintMultiplier + sprintSpeedModifier;}}
    public float CrouchMultiplier {get {return crouchMultiplier + crouchSpeedModifier;}}
    public float ChangePositionSpeed {get {return changePositionSpeed + changePositionSpeedModifier;}}

    public float SwingStaminaLoss {get {return swingStaminaLoss + swingStaminaLossModifier;}}
    public float SprintStaminaLoss {get {return sprintStaminaLoss + sprintStaminaLossModifier;}}
    public float JumpStaminaLoss {get {return jumpStaminaLoss + jumpStaminaLossModifier;}}

    public float BaseStaminaRegenerationRate {get {return staminaRegeneration;}}
    public float StaminaRegenerationRate {get {return staminaRegeneration + staminaRegenerationModifier;}}

    public float BaseHealthRegenerationRate {get {return healthRegeneration;}}
    public float HealthRegenerationRate {get {return healthRegeneration + healthRegenerationModifier;}}

    public float BaseReach {get {return reach;}}
    public float Reach {get {return reach + reachModifier;}}

    public float TimeBetweenSwings {get {return timeBetweenSwings + timeBetweenSwingsModifier;}}

    public PlayerPickaxe Pickaxe {get {return playerPickaxe;}}
    public Dictionary<int, float> Inventory {get {return inventory;}}
    #endregion

    private void Awake() {
        PlayerPerks = new List<Perk>();
        inventory = new Dictionary<int, float>();
        minedTracker = new Dictionary<string, int>();
    }

    private void Start() {
        SetDefaultValues();

        TimeToNextSwing = timeBetweenSwings;
        playerPickaxe.ChangePickaxe(startingPickaxe);

        PlayerController.OnItemPickup += CalculateScore;
    }

    private void Update() {
        if (!IsSprinting && IsGrounded){
            RegenerateStamina();
            RegenerateHealth();
        }
        ManageCanSprint();
        ManageSwingTime();
    }

    private void SetDefaultValues(){
        ResetModifiers();
        SetMaxValues();
    }

    private void ResetModifiers(){
        healthModifier = 0f;
        staminaModifier = 0f;
        swingSpeedModifier = 0f;
        swingStaminaLossModifier = 0f;
        perksMaxAmountModifier = 0;
        jumpForceModifier = 0f;
        movementSpeedModifier = 0f;
        sprintSpeedModifier = 0f;
        crouchSpeedModifier = 0f;
        changePositionSpeedModifier = 0f;
        sprintStaminaLossModifier = 0f;
        jumpStaminaLossModifier = 0f;
        staminaRegenerationModifier = 0f;
        healthRegenerationModifier = 0f;
        strengthModifier = 0f;
        reachModifier = 0f;
        timeBetweenSwingsModifier = 0f;
    }

    private void SetMaxValues(){
        health = MaxHealth;
        stamina = MaxStamina;
        swingSpeed = SwingSpeed;
        swingStaminaLoss = SwingStaminaLoss;
        perksMaxAmount = PerksMaxAmount;
        jumpForce = JumpForce;
        movementSpeed = MovementSpeed;
        sprintMultiplier = SprintMultiplier;
        crouchMultiplier = CrouchMultiplier;
        changePositionSpeed = ChangePositionSpeed;
        sprintStaminaLoss = SprintStaminaLoss;
        jumpStaminaLoss = JumpStaminaLoss;
        strength = Strength;
        reach = Reach;
        timeBetweenSwings = TimeBetweenSwings;
    }

    private void RegenerateStamina(){
        if (stamina == MaxStamina) {
            IsRegeneratingStamina = false;
            return;
        }

        if (stamina < MaxStamina){
            stamina += StaminaRegenerationRate * Time.deltaTime;
            IsRegeneratingStamina = true;
        }
        else
            stamina = MaxStamina;
            
        PlayerController.OnPlayerStaminaRestored?.Invoke();
    }

    private void RegenerateHealth(){
        if (health == MaxHealth) {
            IsRegeneratingHealth = false;
            return;
        }

        if (health < MaxHealth){
            health += HealthRegenerationRate * Time.deltaTime;
            IsRegeneratingHealth = true;
        }
        else
            health = MaxHealth;
            
        PlayerController.OnPlayerHealthRestored?.Invoke();
    }

    private void ManageCanSprint(){
        if (stamina <= .01f * MaxStamina)
            CanSprint = false;
        else if (stamina >= .5f * MaxStamina)
            CanSprint = true;
    }

    private void ManageSwingTime(){
        if (CanSwing) return;

        if (TimeToNextSwing > 0f)
            TimeToNextSwing -= Time.deltaTime;
        else{
            TimeToNextSwing = TimeBetweenSwings;
            CanSwing = true;
        }

        PlayerController.OnSwingTimeChanged?.Invoke();
    }

    public void ReduceStamina(float value){
        if (value > stamina)
            stamina = 0f;
        else
            stamina -= value;
            
        PlayerController.OnPlayerStaminaLost?.Invoke();
    }

    public void ReduceHealth(float value){
        if (value > health)
            health = 0f;
        else
            health -= value;
            
        PlayerController.OnPlayerHealthLost?.Invoke();
    }

    public void RestoreStamina(float value){
        if (stamina + value > MaxStamina)
            stamina = MaxStamina;
        else
            stamina += value;
            
        PlayerController.OnPlayerStaminaRestored?.Invoke();
    }

    public void RestoreHealth(float value){
        if (health + value > MaxHealth)
            health = MaxHealth;
        else
            health += value;
            
        PlayerController.OnPlayerHealthRestored?.Invoke();
    }

    public void AddItemToInventory(Item item){
        if (inventory.ContainsKey(item.ItemId))
            inventory[item.ItemId] += item.Weight;
        else
            inventory.Add(item.ItemId, item.Weight);

        //todo delete this code once game manager is implemented
        CurrentScore += item.Weight * item.BaseValue;
    }

    public void CalculateScore(){
        // todo: calculating score
    }

    public void AddMinedObjectToTracker(){
        string objName = SelectedObject.gameObject.GetComponent<MineableObject>().Name();
        if (minedTracker.ContainsKey(objName))
            minedTracker[objName]++;
        else
            minedTracker.Add(objName, 1);
    }
}
