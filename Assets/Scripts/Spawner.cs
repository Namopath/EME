using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Collider spawnArea;
    public GameObject[] prefabs;
    public int minSpawnDelay = 10; //Letter f indicates that it is a float
    public int maxSpawnDelay = 15;
    public float minAngle = -15f;
    public float maxAngle = 15f;
    public float minForce = 18f;
    public float maxforce = 22f;
    public float lifetime = 5f;

    //Awake is basically initstate
    private void Awake(){
        spawnArea = GetComponent<Collider>();
    }
    private void OnEnable(){
        StartCoroutine(Spawn());
    }
    private void OnDisable(){
        StopAllCoroutines();
    }
    private IEnumerator Spawn(){
        yield return new WaitForSeconds(5);
        while(enabled){
            GameObject fruit = prefabs[UnityEngine.Random.Range(0,prefabs.Length)];

            Vector3 position = new Vector3();

            position.x = UnityEngine.Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
            position.y = UnityEngine.Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
            position.z = UnityEngine.Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);
            
            Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(minAngle, maxAngle));

            GameObject spawnedFruit = Instantiate(fruit, position, rotation);
            Destroy(spawnedFruit, lifetime);

            float force = UnityEngine.Random.Range(minForce, maxforce);
            spawnedFruit.GetComponent<Rigidbody>().AddForce(spawnedFruit.transform.up * force, ForceMode.Impulse);

            // yield return new WaitForSeconds(UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay));
            yield return new WaitForSeconds(5); //Input has to be int for it to work!
        }
    }
}
