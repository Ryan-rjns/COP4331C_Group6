using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// Functions for changing scene or exiting game
public class ChangeScene : MonoBehaviour
{
    public void MainMenu() {  
        SceneManager.LoadScene("Main");  
    }  
    public void Home() {  
        SceneManager.LoadScene("Home");  
    }  
    public void Upgrades() {  
        SceneManager.LoadScene("Upgrades");  
    }  
    public void RestartLevel() { 
        if(GameManager.currentLevel == 0) {
            SceneManager.LoadScene("DebugScene");  
        }
    }  
    public void Exit() {  
        Application.Quit();  
    }  
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
}
