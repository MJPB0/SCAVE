using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineableObject : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private bool isMineable = true;
    [SerializeField] private bool wasMined = false;
    [SerializeField] private float health;
    [SerializeField] private MineableScriptableObject mineableSO;

    public int Id {get {return id;}}

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
            Debug.Log($"Can't mine {gameObject.name}! Required pickaxe tier: {mineableSO.PickaxeTierRequired}.");
            return;
        }

        if (health > damage)
            ReduceHealth(damage);
        else
            WasMined();
    }

    private void ReduceHealth(float value){
        health -= value;
        Debug.Log($"Player hit {gameObject.name} for {value}dmg!");
    }

    private void WasMined(){
        isMineable = false;
        wasMined = true;
        Debug.Log($"Player successfuly mined {gameObject.name}!");
        gameObject.SetActive(false);
    }
}
