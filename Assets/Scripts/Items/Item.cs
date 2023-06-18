using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private bool isPickable;

    [SerializeField] private ItemScriptableObject itemSO;

    public int ItemId {get {return itemSO.ObjectId;}}
    public bool IsPickable {get {return isPickable;}}
    public float Weight { get { return itemSO.Weight;}}

    private void Awake() {
        GetComponent<MeshCollider>().sharedMesh = itemSO.RandomMesh;
        GetComponentInChildren<MeshFilter>().mesh = itemSO.RandomMesh;
    }

    public void Setup(bool pickable){
        isPickable = pickable;
    }
}
