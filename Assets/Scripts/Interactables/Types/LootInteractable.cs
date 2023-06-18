using UnityEngine;

public class LootInteractable : Interactable
{
    private const string LOOT_ANIMATION = "lootAnimation";

    [SerializeField] private GameObject[] loot;
    [SerializeField] private int lootAmount;

    [Space]
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private Transform lootPos;

    [Space]
    [SerializeField] private Collider[] lootedStateColliders;
    [SerializeField] private GameObject chestLock;

    public override void Interact()
    {
        ManageColliders();
        LootAnimation();
    }

    private void ManageColliders()
    {
        interactCollider.enabled = false;
        foreach (Collider collider in lootedStateColliders)
        {
            collider.enabled = true;
        }

        if (chestLock != null)
        {
            chestLock.GetComponent<Collider>().isTrigger = false;
            chestLock.GetComponent<Rigidbody>().useGravity = true;

            chestLock.transform.Rotate(30, 0, 0);
        }
    }

    public void DropLoot()
    {
        for (int i = 0; i < lootAmount; i++)
        {
            GameObject objectToDrop = loot[Random.Range(0, loot.Length)];
            GameObject instance = Instantiate(objectToDrop, lootPos.position, Quaternion.identity, transform.parent);

            Vector3 force = LootUtils.CalculateLootForce(player.transform.position, lootPos.position, minForce, maxForce);

            instance.GetComponent<Rigidbody>().AddForce(force);

            if (instance.TryGetComponent(out Item item))
            {
                item.Setup(true);
            }
        }
    }

    private void LootAnimation()
    {
        animator.SetTrigger(LOOT_ANIMATION);
    }

    public void Looted()
    {
        isInteractable = false;
    }
}
