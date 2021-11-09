using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 3)]
public class ItemScriptableObject : ScriptableObject
{
    //Item id only a single item can have this id
    [SerializeField] private int id;
    //Items value (selling, gamescore)
    [SerializeField] private float value;

    public int ObjectId {get {return id;}}
    public float Value {get {return value;}}
}
