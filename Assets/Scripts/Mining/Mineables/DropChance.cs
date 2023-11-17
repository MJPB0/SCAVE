using UnityEngine;

[System.Serializable]

public class DropChance
{
    public GameObject item;

    [Range(0f, 1f)]
    public float chance;

    public int amount;
}
