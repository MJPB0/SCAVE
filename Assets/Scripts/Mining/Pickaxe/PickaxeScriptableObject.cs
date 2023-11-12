using UnityEngine;

[CreateAssetMenu(fileName = "Pickaxe", menuName = "Player pickaxe", order = 2)]
public class PickaxeScriptableObject : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    public Sprite Icon {get {return icon;}}
    public Mesh Mesh {get {return mesh;}}
    public Material Material {get {return material;}}
}
