using System.Collections;
using UnityEngine;

public class MineableObject : MonoBehaviour {
    [SerializeField] private float health;
    [SerializeField] private bool isMineable = true;
    [SerializeField] private bool dropOnMine = true;

    [Space]
    [SerializeField] private float minOnMinedForceMultiplier = 0f;
    [SerializeField] private float maxOnMinedForceMultiplier = 15f;

    [Space]
    [SerializeField] private DropChance[] drops;

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

    public bool Mine(float damage, Vector3 currentPlayerPos, Vector3 currentHitPos) {
        playerPos = currentPlayerPos;
        hitPos = currentHitPos;

        if (!isMineable) {
            Logger.Log(LogType.NOT_MINEABLE_WARNING, gameObject.name);
            return false;
        }

        if (health > damage)
            ReduceHealth(damage);
        else
            WasMined();

        if (drops.Length > 0)
            DropItems();

        return true;
    }

    private void ReduceHealth(float value) {
        Logger.Log(LogType.DAMAGE_DEALT, gameObject.name, value.ToString());
        health -= value;
    }

    private void WasMined() {
        isMineable = false;

        Logger.Log(LogType.OBJECT_MINED, gameObject.name);
        PlayerController.OnObjectMined?.Invoke();

        gameObject.SetActive(false);
    }

    public void DropItems() {
        Transform dropsParent = GameObject.FindGameObjectWithTag(Tags.ORE_PARENT_TAG).transform;

        foreach (DropChance dropChance in drops) {
            for (int i = 0; i < dropChance.amount; i++) {
                if (Random.Range(0f, 1f) < dropChance.chance) continue;

                GameObject instance = Instantiate(dropChance.item, hitPos, Quaternion.identity, dropsParent);

                Vector3 force = LootUtils.CalculateLootForce(playerPos, instance.transform.position, minOnMinedForceMultiplier, maxOnMinedForceMultiplier);

                instance.GetComponent<Rigidbody>().AddForce(force);
                instance.GetComponent<Item>().Setup(true);

                Logger.Log(LogType.LOOT_DROPPED, gameObject.name, instance.name);
            }
        }
    }

    public IEnumerator InstantiateImpactParticles(bool isSuccess) {
        Vector3 direction = playerPos - hitPos;
        Vector3 rotation = Vector3.RotateTowards(Vector3.one, direction, 360f, 0f);

        GameObject impactParticles = isSuccess ? mineableSO.SuccessfulImpactParticles : mineableSO.FailedImpactParticles;
        GameObject particles = Instantiate(impactParticles, hitPos, Quaternion.LookRotation(rotation), transform.parent);
        ParticleSystem system = particles.GetComponent<ParticleSystem>();

        yield return new WaitUntil(() => !system.isPlaying);
        Destroy(particles);
    }

    public IEnumerator InstantiateDebrisParticles() {
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
