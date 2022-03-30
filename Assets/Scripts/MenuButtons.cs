using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// Functions for changing scene or exiting game
public class MenuButtons : MonoBehaviour
{
    // Scene Navigation
    public void MainMenu() => Scene.MainMenu.Load();
    public void Home() => Scene.Home.Load();
    public void Upgrades() => Scene.Upgrades.Load();
    public void DebugScene() => Scene.DebugScene.Load();
    public void Level1() => Scene.Level1.Load();
    public void Level2() => Scene.Level2.Load();
    public void Level3() => Scene.Level3.Load();
    public void RestartLevel() => GameManager.RestartScene();
    public void Exit() => Application.Quit();
}