using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    private bool slicing;

    private Collider blade;

    private void Awake(){
        blade = GetComponent<Collider>();
        print("Blade component obtained");
        StopSlicing();
    }

    private void Start(){
        StartSlicing();
    }

    private void OnApplicationQuit(){
        StopSlicing();
    }

    private void StartSlicing(){
        slicing = true;
        blade.enabled = true;
    }

    private void StopSlicing(){
        slicing = false;
        blade.enabled = false;
    }

}
