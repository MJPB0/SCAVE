using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 3)]
public class ItemScriptableObject : ScriptableObject
{
    //Item id only a single item can have this id
    [SerializeField] private int id;
    [SerializeField] private float unitWeight;
    [SerializeField] private Mesh[] itemMeshes;
    //Items value (selling, gamescore)
    public float Value;

    public int ObjectId {get {return id;}}
    public float UnitWeight {get {return unitWeight;}}
    public Mesh[] ItemMeshes {get {return itemMeshes;}}
}
