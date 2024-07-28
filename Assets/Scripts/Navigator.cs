using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public void PlayRegular(){
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayFruit(){
        SceneManager.LoadSceneAsync(2);
    }

    public void PlayMath(){
        SceneManager.LoadSceneAsync(3);
    }
}
