using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableObject : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private bool isMineable = true;
    [SerializeField] private bool wasMined = false;

    [Space]
    //Minimal force multipler used when item is spawned after object's been mined
    [SerializeField] private float minOnMinedForceMultiplier = 0f;
    //Maximum force multipler used when item is spawned after object's been mined
    [SerializeField] private float maxOnMinedForceMultiplier = 15f;
    
    [Space]
    //Max amount of items that can drop from this object
    [SerializeField] private int dropRate = 2;
    //Item that drops from this object
    [SerializeField] private GameObject itemDrop;
    [SerializeField] private MineableScriptableObject mineableSO;

    private void Start() {
        Setup();
    }

    //Checks if player's pickaxe is good enough for this object to be mined by it
    public void IsMineable(int pickaxeTier){
        isMineable = pickaxeTier >= mineableSO.PickaxeTierRequired;
    }

    //Values set at start
    private void Setup(){
        health = mineableSO.Health;
        GetComponentInChildren<MeshRenderer>().material = mineableSO.Material;
        transform.localScale *= mineableSO.SizeMultiplier;
    }

    //Usable by the player to mine the object
    public void Mine(float damage){
        if (!isMineable) {
            //Debug.Log($"Can't mine {gameObject.name}! Required pickaxe tier: {mineableSO.PickaxeTierRequired}.");
            return;
        }

        //Reduce health or mark this object as mined
        if (health > damage)
            ReduceHealth(damage);
        else
            WasMined();
    }

    private void ReduceHealth(float value){
        health -= value;
        //Debug.Log($"Player hit {gameObject.name} for {value}dmg!");
    }

    //Marks this object as mined
    private void WasMined(){
        isMineable = false;
        wasMined = true;
        //Debug.Log($"Player successfuly mined {gameObject.name}!");
        
        //Drop items from this object
        DropItems();
        //Hide the object
        gameObject.SetActive(false);
    }
  
    public void DropItems(){
        //Amount of items to be dropped
        int amount = Random.Range(dropRate/2, dropRate);

        //Calculate force applied to the spawned item
        float forceX = Random.Range(0f,1f);
        float forceY = Random.Range(0.1f,1f);
        float forceZ = Random.Range(0f,1f);
        Vector3 force = new Vector3(forceX, forceY, forceZ);

        //Debug.Log($"{amount}x{itemDrop.name} came out of {gameObject.name}!");
        //Spawn the item
        GameObject instance = Instantiate(itemDrop, transform.position, Quaternion.identity, transform.parent);
        //Add force to the item
        instance.GetComponent<Rigidbody>().AddForce(force * Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier));
        //Set variable values of the item
        instance.GetComponent<Item>().Setup(true, amount);
    }
}
