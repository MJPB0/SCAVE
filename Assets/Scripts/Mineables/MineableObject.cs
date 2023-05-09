using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MineableObject : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private bool isMineable = true;
    [SerializeField] private bool dropOnMine = true;

    [Space]
    [SerializeField] private float minOnMinedForceMultiplier = 0f;
    [SerializeField] private float maxOnMinedForceMultiplier = 15f;

    [Header("Base drop")]
    [SerializeField][Min(0)] private int minDropOnHit = 1;
    [SerializeField] private int maxDropOnHit = 5;
    [SerializeField] private GameObject[] baseDrops;

    [Header("Special drop")]
    [SerializeField][Range(0, 1)] private float specialDropChance = 0.5f;
    [SerializeField][Min(1)] private int minSpecialDrop = 0;
    [SerializeField] private int maxSpecialDrop = 3;
    [SerializeField] private GameObject[] specialDrops;

    [Space]
    [SerializeField] private MineableScriptableObject mineableSO;

    private Vector3 playerPos;
    private Vector3 hitPos;

    public string Name() {
        return mineableSO.name;
    }

    private void Start() {
        Setup();
    }

    public void IsMineable(int pickaxeTier) {
        isMineable = pickaxeTier >= mineableSO.PickaxeTierRequired;
    }

    private void Setup() {
        health = mineableSO.Health;

        GetComponentInChildren<MeshFilter>().mesh = mineableSO.Mesh;
        GetComponent<MeshCollider>().sharedMesh = mineableSO.Mesh;
    }

    public void Mine(float damage, Vector3 currentPlayerPos, Vector3 currentHitPos) {
        if (!isMineable) {
            Debug.Log($"<color=red>[COMBAT]</color> <color=red>{gameObject.name}</color> is not mineable by the <color=teal>Player</color>");
            return;
        }
        playerPos = currentPlayerPos;
        hitPos = currentHitPos;

        Debug.Log($"<color=red>[COMBAT]</color> <color=red>{gameObject.name}</color> received <color=maroon>{damage}dmg</color>");

        if (health > damage)
            ReduceHealth(damage);
        else
            WasMined();

        if (baseDrops.Length > 0)
            DropItems();
    }

    private void ReduceHealth(float value) {
        health -= value;
    }

    private void WasMined() {
        isMineable = false;

        PlayerController.OnObjectMined?.Invoke();

        if (specialDrops.Length > 0 && Random.Range(0f, 1f) < specialDropChance)
            DropSpecialItems();

        gameObject.SetActive(false);
    }

    public void DropItems() {
        for (int i = 0; i < Random.Range(minDropOnHit, maxDropOnHit + 1); i++)
        {
            GameObject instance = Instantiate(baseDrops[Random.Range(0, baseDrops.Length - 1)], hitPos, Quaternion.identity, transform.parent);

            Vector3 forceDirection = playerPos - instance.transform.position;
            float forceX = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
            float forceY = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
            float forceZ = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
            Vector3 force = new Vector3(forceDirection.x * forceX, forceDirection.y * forceY, forceDirection.z * forceZ);

            instance.GetComponent<Rigidbody>().AddForce(force);
            instance.GetComponent<Item>().Setup(true);

            Debug.Log($"<color=yellow>[DROP]</color> <color=red>{gameObject.name}</color> dropped <color=yellow>{instance.name}</color>");
        }
    }

    public void DropSpecialItems()
    {
        for (int i = 0; i < Random.Range(minSpecialDrop, maxSpecialDrop); i++)
        {
            GameObject instance = Instantiate(specialDrops[Random.Range(0, specialDrops.Length)], hitPos, Quaternion.identity, transform.parent);

            Vector3 forceDirection = playerPos - instance.transform.position;
            float forceX = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
            float forceY = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
            float forceZ = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
            Vector3 force = new Vector3(forceDirection.x * forceX, forceDirection.y * forceY, forceDirection.z * forceZ);

            instance.GetComponent<Rigidbody>().AddForce(force);
            instance.GetComponent<Item>().Setup(true);

            Debug.Log($"<color=yellow>[DROP]</color> <color=red>{gameObject.name}</color> dropped <color=yellow>{instance.name}</color>");
        }
    }
}
