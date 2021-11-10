using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private int amount;
    [SerializeField] private bool isPickable;
    [SerializeField] private ItemScriptableObject itemSO;

    public int Amount {get {return amount;}}
    public int ItemId {get {return itemSO.ObjectId;}}
    public bool IsPickable {get {return isPickable;}}

    //Set this item's values
    public void Setup(bool pickable, int value, float scale){
        isPickable = pickable;
        amount = value;
        gameObject.transform.localScale *= scale;
    }
}
