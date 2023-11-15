using UnityEngine;

public class Item : MonoBehaviour {
    [SerializeField] protected string displayName;

    [Space]
    [SerializeField] private bool requireHoldInteraction;

    [SerializeField] private bool isPickable;

    [SerializeField] private ItemScriptableObject itemSO;

    public int ItemId {get {return itemSO.ObjectId;}}
    public bool RequireHoldInteraction { get {return requireHoldInteraction; }}
    public bool IsPickable {get {return isPickable;} }
    public float Weight { get { return itemSO.Weight;} }
    public string DisplayName { get { return displayName; } }

    private void Awake() {
        GetComponentInChildren<MeshRenderer>().material = itemSO.MeshMaterial;
        GetComponent<MeshCollider>().sharedMesh = itemSO.RandomMesh;
        GetComponentInChildren<MeshFilter>().mesh = itemSO.RandomMesh;
    }

    public void Setup(bool pickable = true, bool holdInteraction = false) {
        isPickable = pickable;
        // TODO: implement hold interaction item pick up
        requireHoldInteraction = holdInteraction;
    }
}
