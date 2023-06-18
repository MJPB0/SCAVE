using UnityEngine;

public class CompleteLevelInteractable : Interactable
{
    private Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public override void Interact()
    {
        // TODO complete current level
    }
}
