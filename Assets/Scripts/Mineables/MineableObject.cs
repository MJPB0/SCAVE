using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableObject : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private bool isMineable = true;
    [SerializeField] private bool wasMined = false;
    [SerializeField] private bool dropOnMine = true;

    [Space]
    //Minimal force multipler used when item is spawned after object's been mined
    [SerializeField] private float minOnMinedForceMultiplier = 0f;
    //Maximum force multipler used when item is spawned after object's been mined
    [SerializeField] private float maxOnMinedForceMultiplier = 15f;
    
    [Space]
    //Max amount of items that can drop from this object
    [SerializeField] private int dropRate = 2;
    [SerializeField] private float sizeMultiplier = 1f;
    //Items that drop from this object
    [SerializeField] private List<GameObject> itemDrops;
    [Range(0,1)]
    [SerializeField] private List<float> itemDropChances;
    [SerializeField] private MineableScriptableObject mineableSO;

    [Space]
    [SerializeField] private GameObject impactParticles;

    private Vector3 playerPos;
    private Vector3 hitPos;

    private void Start() {
        Setup();
    }

    //Checks if player's pickaxe is good enough for this object to be mined by it
    public void IsMineable(int pickaxeTier){
        isMineable = pickaxeTier >= mineableSO.PickaxeTierRequired;
    }

    //Values set at start
    private void Setup(){
        if (itemDropChances.Count != itemDrops.Count)
            Debug.LogError($"{gameObject.name}'s drops are not properly set!");

        health = mineableSO.Health;
        GetComponentInChildren<MeshRenderer>().material = mineableSO.Material;

        if (mineableSO.Meshes.Length < 1) return;
        
        Mesh mesh = mineableSO.Meshes[Random.Range(0, mineableSO.Meshes.Length - 1)];
        GetComponentInChildren<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh; 
    }

    //Usable by the player to mine the object
    public void Mine(float damage, Vector3 currentPlayerPos, Vector3 currentHitPos){
        if (!isMineable) {
            //Debug.Log($"Can't mine {gameObject.name}! Required pickaxe tier: {mineableSO.PickaxeTierRequired}.");
            return;
        }

        playerPos = currentPlayerPos;
        hitPos = currentHitPos;
        //Reduce health or mark this object as mined
        if (health > damage)
            ReduceHealth(damage);
        else
            WasMined();
    }

    private void ReduceHealth(float value){
        health -= value;
        if (dropOnMine && itemDrops.Count > 0)
            DropItems();
        //Debug.Log($"Player hit {gameObject.name} for {value}dmg!");
    }

    //Marks this object as mined
    private void WasMined(){
        isMineable = false;
        wasMined = true;
        //Debug.Log($"Player successfuly mined {gameObject.name}!");
        
        //Drop items from this object
        if (itemDrops.Count > 0)
            DropItems();
        //Hide the object
        gameObject.SetActive(false);
    }
  
    public void DropItems(){
        //Amount of items to be dropped
        int amount = Random.Range(dropRate/2, dropRate+1);

        for (int i = 0; i < amount; i++){
            for (int j = 0; j < itemDrops.Count; j++){
                float itemScale = sizeMultiplier * Random.Range(.5f,1.5f);
                float chance = Random.Range(0f,1f);

                if (chance < 1 - itemDropChances[j]) continue;

                //Try to spawn the item
                GameObject instance = Instantiate(itemDrops[j], hitPos, Quaternion.identity, transform.parent);
                
                //Calculate force applied to the spawned item
                Vector3 forceDirection = playerPos - instance.transform.position;
                float forceX = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
                float forceY = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
                float forceZ = Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier);
                Vector3 force = new Vector3(forceDirection.x * forceX, forceDirection.y * forceY, forceDirection.z * forceZ);

                //Add force to the item
                instance.GetComponent<Rigidbody>().AddForce(force);
                //Set variable values of the item
                instance.GetComponent<Item>().Setup(true, itemScale);
            }
        }
    }
    
    public IEnumerator ImpactParticles(){
        impactParticles.SetActive(true);
        impactParticles.transform.position = hitPos;

        Vector3 direction = playerPos - hitPos;
        Vector3 rotation = Vector3.RotateTowards(impactParticles.transform.forward, direction, 360f, 0f);
        impactParticles.transform.rotation = Quaternion.LookRotation(rotation);

        ParticleSystem system = impactParticles.GetComponent<ParticleSystem>();
        Color col = mineableSO.Material.color;
        var mainSystem = system.main;
        mainSystem.startColor = new Color(col.r, col.g, col.b);
        system.Play();

        yield return new WaitUntil(() => !system.isPlaying);
        impactParticles.SetActive(false);
        impactParticles.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
