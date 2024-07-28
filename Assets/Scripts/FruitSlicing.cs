using System.Collections;
using System.Collections.Generic;
using TMPro;

// using System.Numerics;
using UnityEngine;

public class FruitSlicing : MonoBehaviour
{
    public GameObject wholeFruit;
    public GameObject slicedFruit;
    private Rigidbody fruitBody;
    private Collider fruitCollider;
    // public GameObject scoreObj;
    // public GameObject scoreObj;
    // private TMP_Text tmpText;
    // public int intScore;
    public Vector3 direction = Vector3.forward;
    private float force = 5f;

    //When a trigger object colldes, this function is called
    
    private void Awake(){
        fruitBody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
        // tmpText = scoreObj.GetComponentInChildren<TMP_Text>();
        print("Components obtained");
    }
    
    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            print("Collision detected");
            ScoreManager.Instance.AddScore();
            Slice();
        }
    }

    private void Slice(){
        wholeFruit.SetActive(false);
        slicedFruit.SetActive(true);
        fruitCollider.enabled = false;
        
        Rigidbody[] slices  = slicedFruit.GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody slice in slices){
            
            slice.AddForce(direction.normalized * force, ForceMode.Impulse);
        }
    }



}
