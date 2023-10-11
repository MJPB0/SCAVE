using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private bool requireHoldInteraction;

    [SerializeField] private bool isPickable;

    [SerializeField] private ItemScriptableObject itemSO;

    public int ItemId {get {return itemSO.ObjectId;}}
    public bool RequireHoldInteraction { get {return requireHoldInteraction; }}
    public bool IsPickable {get {return isPickable;} }
    public float Weight { get { return itemSO.Weight;}}

    private void Awake() {
        GetComponent<MeshCollider>().sharedMesh = itemSO.RandomMesh;
        GetComponentInChildren<MeshFilter>().mesh = itemSO.RandomMesh;
    }

    public void Setup(bool pickable = true, bool holdInteraction = false) {
        isPickable = pickable;
        // TODO: implement hold interaction item pick up
        requireHoldInteraction = holdInteraction;
    }
}
