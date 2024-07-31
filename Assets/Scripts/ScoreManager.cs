using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public GameObject scoreObj;
    private TMP_Text scoreText;
    public int intScore = 0;

    public GameObject hrWarn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start(){
        UpdateScore();
        if(scoreObj != null){
            scoreText = scoreObj.GetComponentInChildren<TMP_Text>();
        }
        else{
            print("Score manager is null");
        }
    }

    public void AddScore(){
        if(intScore < 20){
           intScore += 1;
        UpdateScore(); 
        } else{
            hrWarn.SetActive(true);
        }
    }

    private void UpdateScore(){
        if(scoreText != null){
            scoreText.text = "Score:  " + intScore.ToString();
        }
    }
}
