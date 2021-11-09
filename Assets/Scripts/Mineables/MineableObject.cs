using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableObject : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private bool isMineable = true;
    [SerializeField] private bool wasMined = false;

    [Space]
    [SerializeField] private float minOnMinedForceMultiplier = 0f;
    [SerializeField] private float maxOnMinedForceMultiplier = 15f;
    
    [Space]
    [SerializeField] private int dropRate = 2;
    [SerializeField] private GameObject itemDrop;
    [SerializeField] private MineableScriptableObject mineableSO;

    private void Start() {
        Setup();
    }

    public void IsMineable(int pickaxeTier){
        isMineable = pickaxeTier >= mineableSO.PickaxeTierRequired;
    }

    private void Setup(){
        health = mineableSO.Health;
        GetComponentInChildren<MeshRenderer>().material = mineableSO.Material;
        transform.localScale *= mineableSO.SizeMultiplier;
    }

    public void Mine(float damage){
        if (!isMineable) {
            //Debug.Log($"Can't mine {gameObject.name}! Required pickaxe tier: {mineableSO.PickaxeTierRequired}.");
            return;
        }

        if (health > damage)
            ReduceHealth(damage);
        else
            WasMined();
    }

    private void ReduceHealth(float value){
        health -= value;
        //Debug.Log($"Player hit {gameObject.name} for {value}dmg!");
    }

    private void WasMined(){
        isMineable = false;
        wasMined = true;
        //Debug.Log($"Player successfuly mined {gameObject.name}!");
        
        DropItems();
        gameObject.SetActive(false);
    }
    
    public void DropItems(){
        int amount = Random.Range(dropRate/2, dropRate);

        float forceX = Random.Range(0f,1f);
        float forceY = Random.Range(0.1f,1f);
        float forceZ = Random.Range(0f,1f);
        Vector3 force = new Vector3(forceX, forceY, forceZ);

        //Debug.Log($"{amount}x{itemDrop.name} came out of {gameObject.name}!");
        GameObject instance = Instantiate(itemDrop, transform.position, Quaternion.identity, transform.parent);
        instance.GetComponent<Rigidbody>().AddForce(force * Random.Range(minOnMinedForceMultiplier, maxOnMinedForceMultiplier));
        instance.GetComponent<Item>().Setup(true, amount);
    }
}
