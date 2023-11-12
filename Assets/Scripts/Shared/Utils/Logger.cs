using UnityEngine;

public static class Logger {
    private const string VALUE_NOT_PROVIDED_TEXT = "<color={COMBAT_COLOR}>[VALUE NOT PROVIDED]</color>";

    private static readonly string PLAYER_COLOR = "#2d5efc";

    private static readonly string INTERACTION_COLOR = "#c4a754";
    private static readonly string LOOT_COLOR = "#ffd900";
    private static readonly string COMBAT_COLOR = "#bf3737";
    private static readonly string DAMAGE_COLOR = "#911127";
    private static readonly string TARGET_COLOR = "#edf5ab";
    private static readonly string UPGRADE_COLOR = "#fcb72d";

    private static readonly string FAILURE_COLOR = "#ff0000";
    private static readonly string WARNING_COLOR = "#e3df9a";
    private static readonly string SUCCESS_COLOR = "#00ff22";

    private static readonly string PLAYER = $"<color={PLAYER_COLOR}>Player</color>";

    private static readonly string INTERACTION = $"<color={INTERACTION_COLOR}>[INTERACTION]</color>";
    private static readonly string DROP = $"<color={LOOT_COLOR}>[DROP]</color>";
    private static readonly string SPECIAL_DROP = $"<color={LOOT_COLOR}>[SPECIAL]</color>";
    private static readonly string LOOT = $"<color={LOOT_COLOR}>[LOOT]</color>";
    private static readonly string COMBAT = $"<color={COMBAT_COLOR}>[COMBAT]</color>";
    private static readonly string UPGRADE = $"<color={UPGRADE_COLOR}>[UPGRADE]</color>";

    private static readonly string FAILURE = $"<color={FAILURE_COLOR}>[FAILURE]</color>";
    private static readonly string WARNING = $"<color={WARNING_COLOR}>[WARNING]</color>";
    private static readonly string SUCCESS = $"<color={SUCCESS_COLOR}>[SUCCESS]</color>";

    private static readonly string INTERACTION_SUCCESS = $"{INTERACTION} {SUCCESS}";
    private static readonly string INTERACTION_WARNING = $"{INTERACTION} {WARNING}";
    private static readonly string INTERACTION_FAILURE = $"{INTERACTION} {FAILURE}";

    private static readonly string LOOT_SUCCESS = $"{LOOT} {SUCCESS}";
    private static readonly string LOOT_WARNING = $"{LOOT} {WARNING}";
    private static readonly string LOOT_FAILURE = $"{LOOT} {FAILURE}";

    private static readonly string COMBAT_SUCCESS = $"{COMBAT} {SUCCESS}";
    private static readonly string COMBAT_FAILURE = $"{COMBAT} {FAILURE}";

    private static readonly string DROP_SUCCESS = $"{DROP} {SUCCESS}";
    private static readonly string SPECIAL_DROP_SUCCESS = $"{DROP} {SPECIAL_DROP} {SUCCESS}";

    private static readonly string UPGRADE_SUCCESS = $"{UPGRADE} {SUCCESS}";
    private static readonly string UPGRADE_WARNING = $"{UPGRADE} {WARNING}";
    private static readonly string UPGRADE_FAILURE = $"{UPGRADE} {FAILURE}";

    public static void Log(LogType logType, string target = VALUE_NOT_PROVIDED_TEXT, string additionalValue = VALUE_NOT_PROVIDED_TEXT, string secondAdditionalValue = VALUE_NOT_PROVIDED_TEXT) {
        switch (logType) {
            case LogType.OBJECT_INTERACTION:
                Debug.Log($"{INTERACTION_SUCCESS} {PLAYER} interacted with <color={TARGET_COLOR}>{target}</color>");
                break;
            case LogType.ITEM_INTERACTION_INABILITY:
                Debug.Log($"{INTERACTION_FAILURE} {PLAYER} can't interact with <color={TARGET_COLOR}>{target}</color>!");
                break;
            case LogType.ITEM_INTERACTION_HOLD_INTERACTION_WARNING:
                Debug.Log($"{INTERACTION_WARNING} {PLAYER} has to hold interact button to interact with <color={TARGET_COLOR}>{target}</color>!");
                break;
            case LogType.WRONG_INTERACTION_TARGET_WARNING:
                Debug.Log($"{INTERACTION_FAILURE} {PLAYER} tried to interact with <color={TARGET_COLOR}>{target}</color> but it doesn't have neither Interactable nor Item scripts attached!");
                break;
            case LogType.ITEM_PICKUP:
                Debug.Log($"{LOOT_SUCCESS} {PLAYER} picked up <color={TARGET_COLOR}>{target}</color>");
                break;
            case LogType.ITEM_PICKUP_HOLD_INTERACTION_WARNING:
                Debug.Log($"{LOOT_WARNING} {PLAYER} has to hold interact button to pick up <color={TARGET_COLOR}>{target}</color>!");
                break;
            case LogType.ITEM_PICKUP_INABILITY:
                Debug.Log($"{LOOT_FAILURE} {PLAYER} can't pick up <color={TARGET_COLOR}>{target}</color>!");
                break;
            case LogType.OBJECT_MINED:
                Debug.Log($"{COMBAT_SUCCESS} {PLAYER} mined <color={COMBAT_COLOR}>{target}</color>");
                break;
            case LogType.ATTEMPT_TO_DEAL_DAMAGE:
                Debug.Log($"{COMBAT} {PLAYER} tried dealing <color={DAMAGE_COLOR}>{additionalValue} {secondAdditionalValue} damage</color> to <color={COMBAT_COLOR}>{target}</color>");
                break;
            case LogType.NOT_MINEABLE_WARNING:
                Debug.Log($"{COMBAT_FAILURE} <color={COMBAT_COLOR}>{target}</color> is not mineable by the {PLAYER}");
                break;
            case LogType.DAMAGE_DEALT:
                Debug.Log($"{COMBAT_SUCCESS} <color={COMBAT_COLOR}>{target}</color> received <color={DAMAGE_COLOR}>{additionalValue} damage</color>");
                break;
            case LogType.PICKAXE_SWUNG:
                Debug.Log($"{COMBAT_SUCCESS} {PLAYER} swung his <color={TARGET_COLOR}>{target}</color>");
                break;
            case LogType.PICKAXE_HIT:
                Debug.Log($"{COMBAT_SUCCESS} {PLAYER} hit <color={COMBAT_COLOR}>{target}</color>");
                break;
            case LogType.LOOT_DROPPED:
                Debug.Log($"{DROP_SUCCESS} <color={COMBAT_COLOR}>{target}</color> dropped <color={TARGET_COLOR}>{additionalValue}</color>");
                break;
            case LogType.SPECIAL_LOOT_DOPPED:
                Debug.Log($"{SPECIAL_DROP_SUCCESS} <color={COMBAT_COLOR}>{target}</color> dropped <color={TARGET_COLOR}>{additionalValue}</color>");
                break;
            case LogType.PICKAXE_UPGRADED:
                Debug.Log($"{UPGRADE_SUCCESS} {PLAYER} upgraded his <color={TARGET_COLOR}>{target}</color>");
                break;
            case LogType.PICKAXE_FULLY_UPGRADED_WARNING:
                Debug.Log($"{UPGRADE_WARNING} {PLAYER}'s <color={TARGET_COLOR}>{target}</color> is fully upgraded!");
                break;
            case LogType.PICKAXE_UPGRADE_REQUIRES_HIGHER_ANVIL_TIER_WARNING:
                Debug.Log($"{UPGRADE_FAILURE} {PLAYER}'s <color={TARGET_COLOR}>{target}</color> requires higher anvil tier! Anvil tier: {additionalValue}, required: {secondAdditionalValue}");
                break;
            case LogType.PICKAXE_UPGRADE_REQUIRES_MORE_GOLD_WARNING:
                Debug.Log($"{UPGRADE_FAILURE} {PLAYER} doesn't have enough gold to upgrade his <color={TARGET_COLOR}>{target}</color>");
                break;
            case LogType.PICKAXE_UPGRADE_REQUIRES_MORE_RESOURCES_WARNING:
                Debug.Log($"{UPGRADE_FAILURE} {PLAYER} doesn't have enough resources to upgrade his <color={TARGET_COLOR}>{target}</color>");
                break;
        }
    }

}
