using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering.VirtualTexturing;

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

    public bool Mine(float damage, Vector3 currentPlayerPos, Vector3 currentHitPos)
    {
        playerPos = currentPlayerPos;
        hitPos = currentHitPos;

        if (!isMineable) {
            Debug.Log($"<color=red>[COMBAT]</color> <color=red>{gameObject.name}</color> is not mineable by the <color=teal>Player</color>");
            return false;
        }

        if (health > damage)
            ReduceHealth(damage);
        else
            WasMined();

        Debug.Log($"<color=red>[COMBAT]</color> <color=red>{gameObject.name}</color> received <color=maroon>{damage}dmg</color>");

        if (baseDrops.Length > 0)
            DropItems();

        return true;
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
        Transform dropsParent = GameObject.FindGameObjectWithTag(Constants.ORE_PARENT_TAG).transform;

        for (int i = 0; i < Random.Range(minDropOnHit, maxDropOnHit + 1); i++)
        {
            GameObject instance = Instantiate(baseDrops[Random.Range(0, baseDrops.Length - 1)], hitPos, Quaternion.identity, dropsParent);

            Vector3 force = LootUtils.CalculateLootForce(playerPos, instance.transform.position, minOnMinedForceMultiplier, maxOnMinedForceMultiplier);

            instance.GetComponent<Rigidbody>().AddForce(force);
            instance.GetComponent<Item>().Setup(true);

            Debug.Log($"<color=yellow>[DROP]</color> <color=red>{gameObject.name}</color> dropped <color=yellow>{instance.name}</color>");
        }
    }

    public void DropSpecialItems()
    {
        Transform dropsParent = GameObject.FindGameObjectWithTag(Constants.ORE_PARENT_TAG).transform;

        for (int i = 0; i < Random.Range(minSpecialDrop, maxSpecialDrop); i++)
        {
            GameObject instance = Instantiate(specialDrops[Random.Range(0, specialDrops.Length)], hitPos, Quaternion.identity, dropsParent);

            Vector3 force = LootUtils.CalculateLootForce(playerPos, instance.transform.position, minOnMinedForceMultiplier, maxOnMinedForceMultiplier);

            instance.GetComponent<Rigidbody>().AddForce(force);
            instance.GetComponent<Item>().Setup(true);

            Debug.Log($"<color=yellow>[DROP]</color> <color=red>{gameObject.name}</color> dropped <color=yellow>{instance.name}</color>");
        }
    }

    public IEnumerator InstantiateImpactParticles(bool isSuccess)
    {
        Vector3 direction = playerPos - hitPos;
        Vector3 rotation = Vector3.RotateTowards(Vector3.one, direction, 360f, 0f);

        GameObject impactParticles = isSuccess ? mineableSO.SuccessfulImpactParticles : mineableSO.FailedImpactParticles;
        GameObject particles = Instantiate(impactParticles, hitPos, Quaternion.LookRotation(rotation), transform.parent);
        ParticleSystem system = particles.GetComponent<ParticleSystem>();

        yield return new WaitUntil(() => !system.isPlaying);
        Destroy(particles);
    }

    public IEnumerator InstantiateDebrisParticles()
    {
        Vector3 direction = playerPos - hitPos;
        Vector3 rotation = Vector3.RotateTowards(Vector3.one, direction, 360f, 0f);

        GameObject particles = Instantiate(mineableSO.DebrisParticles, hitPos, Quaternion.LookRotation(rotation), transform.parent);

        ParticleSystem system = particles.GetComponent<ParticleSystem>();
        var mainSystem = system.main;

        Color materialColor = GetComponentInChildren<MeshRenderer>().material.color;
        mainSystem.startColor = new Color(materialColor.r, materialColor.g, materialColor.b, 1);

        yield return new WaitUntil(() => !system.isPlaying);
        Destroy(particles);
    }
}
