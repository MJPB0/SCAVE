using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 3)]
public class ItemScriptableObject : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private float value;

    public int ObjectId {get {return id;}}
    public float Value {get {return value;}}
}
