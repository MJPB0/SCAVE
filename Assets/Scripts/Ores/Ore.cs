using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private bool isMineable = true;
    [SerializeField] private bool wasMined = false;
    [SerializeField] private float health;
    [SerializeField] private OreScriptableObject oreSO;

    public int Id {get {return id;}}

    private void Start() {
        health = oreSO.Health;
        GetComponentInChildren<MeshRenderer>().material = oreSO.Material;
        transform.localScale *= oreSO.SizeMultiplier;
    }

    public void IsMineable(int pickaxeTier){
        isMineable = pickaxeTier >= oreSO.PickaxeTierRequired;
    }

    public void Mine(float damage){
        if (!isMineable) {
            return;
        }

        if (health > damage)
            HealthReduced(damage);
        else
            OreMined();
    }

    private void HealthReduced(float value){
        health -= value;
    }

    private void OreMined(){
        isMineable = false;
        wasMined = true;
        gameObject.SetActive(false);
    }
}
