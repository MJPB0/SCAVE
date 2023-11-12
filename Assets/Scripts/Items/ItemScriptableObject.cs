using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 3)]
public class ItemScriptableObject : ScriptableObject {
    [SerializeField] private ItemId id;

    [SerializeField] private float weight;
    [SerializeField] private float value;

    [SerializeField] private Mesh[] itemMeshes;

    public int ObjectId { get { return (int)id; } }
    public float Weight { get { return weight; } }
    public float Value { get { return value; } }
    public Mesh[] ItemMeshes { get { return itemMeshes; } }
    public Mesh RandomMesh { get { return itemMeshes[Random.Range(0, itemMeshes.Length - 1)]; } }
}
