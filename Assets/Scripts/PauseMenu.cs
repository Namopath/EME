using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenu;

    void Start(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        print("Pause menu script is running");
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                Resume();
            }
            if(!isPaused){
                Pause();
            }
        }
    }
   public void Resume(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    void Pause(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void mainMenu(){
        SceneManager.LoadScene(0);
    }

    public void Quit(){
        Debug.Log("Quitting Game");
        Application.Quit();
        SceneManager.LoadScene(0);
    }
}
