using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interactable", menuName = "Interactable", order = 4)]
public class InteractableScriptableObject : ScriptableObject
{
    [SerializeField] private Mesh[] itemMeshes;

    public Mesh[] ItemMeshes { get { return itemMeshes; } }
    public Mesh RandomMesh { get { return itemMeshes[Random.Range(0, itemMeshes.Length - 1)]; } }
}
