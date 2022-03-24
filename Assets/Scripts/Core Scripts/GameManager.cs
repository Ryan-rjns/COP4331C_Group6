using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    // Current scene
    public static Scene CurrentScene { get; private set; } = Scene.MainMenu;
    // Current UI panel
    public static GameObject CurrentPanel { get; private set; } = null;

    // Debug: You can trigger this function by pressing "Z" while playing the game
    public static void Win()
    {
        // Check if another panel is being displayed
        if(CurrentPanel == null) {
            // Display win panel
            GameObject winPanel = GameObject.Find("Panels").transform.GetChild(0).gameObject;
            CurrentPanel = winPanel;
            winPanel.SetActive(true);
        }
    }
    // Debug: You can trigger this function by pressing "X" while playing the game
    public static void Lose()
    {
        // Check if another panel is being displayed
        if(CurrentPanel == null) {
            // Display lose panel
            GameObject losePanel = GameObject.Find("Panels").transform.GetChild(1).gameObject;
            CurrentPanel = losePanel;
            losePanel.SetActive(true);
        }
    }
    // Debug: You can trigger this function by pressing "C" while playing the game
    public static void Pause()
    {
        GameObject pausePanel = GameObject.Find("Panels").transform.GetChild(2).gameObject;
        // Check if another panel is being displayed
        if(CurrentPanel == null) {
            // Display pause panel
            CurrentPanel = pausePanel;
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        } else if(CurrentPanel == pausePanel) {
            CurrentPanel = null;
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    // Scene controls
    public static string SceneName(this Scene scene)
    {
        if (scene == Scene.DebugScene) return "DebugScene";
        if (scene == Scene.MainMenu) return "MainMenu";
        if (scene == Scene.Home) return "Home";
        if (scene == Scene.Upgrades) return "Upgrades";
        if (scene == Scene.Level1) return "Level1";
        if (scene == Scene.Level2) return "Level2";
        if (scene == Scene.Level3) return "Level3";
        return "error";
    }
    public static void Load(this Scene scene)
    {
        CurrentScene = scene;
        SceneManager.LoadScene(scene.SceneName());
    }
    public static void RestartScene()
    {
        CurrentScene.Load();
    }

    public static Player GetPlayer()
    {
        Player[] players = Object.FindObjectsOfType<Player>();
        if (players.Length <= 0) return null;
        return players[0];
    }
}

public enum Scene
{
    DebugScene,
    MainMenu,
    Home,
    Upgrades,
    Level1,
    Level2,
    Level3
}
