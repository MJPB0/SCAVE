using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private float weight;
    [SerializeField] private bool isPickable;
    [SerializeField] private ItemScriptableObject itemSO;

    public float Weight {get {return weight;}}
    public int ItemId {get {return itemSO.ObjectId;}}
    public bool IsPickable {get {return isPickable;}}
    public float BaseValue {get {return itemSO.Value;}}

    private void Awake() {
        Mesh mesh = itemSO.ItemMeshes[Random.Range(0,itemSO.ItemMeshes.Length)];
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponentInChildren<MeshFilter>().mesh = mesh;
    }

    //Set this item's values
    public void Setup(bool pickable, float sizeValue){
        isPickable = pickable;
        weight = sizeValue * itemSO.UnitWeight;
        gameObject.transform.localScale *= sizeValue;
        GetComponent<SphereCollider>().radius /= sizeValue;
    }
}
