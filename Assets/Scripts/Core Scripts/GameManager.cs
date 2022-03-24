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
    // A list of the objectives for this level
    private static List<Objective> _sceneObjectives = null;
    public static List<Objective> SceneObjectives 
    {
        get => _sceneObjectives; 
        private set
        {
            // If the old list exists, disable all of its contents and clear it
            if(_sceneObjectives != null)
            {
                foreach (Objective o in _sceneObjectives) o?.Disable();
                _sceneObjectives.Clear();
            }
            // Replace the old list with the new list
            _sceneObjectives = value;
            // Activate the first Objective in the new list (if there is one)
            CurrObjective = 0;
        } 
    }
    // THe current objective in this level that is active
    private static int _currObjective = 0;
    public static int CurrObjective
    {
        get => _currObjective;
        private set
        {
            // If the old objective was valid, disable it.
            if (_currObjective < (SceneObjectives?.Count ?? -1)) SceneObjectives[_currObjective].Disable();
            // Update the current index
            _currObjective = value;
            // If the new objective index is valid, enable it
            if (_currObjective < (SceneObjectives?.Count ?? -1)) SceneObjectives[_currObjective].Enable();
        }
    }

    public static Player GetPlayer()
    {
        Player[] players = Object.FindObjectsOfType<Player>();
        if (players.Length <= 0) return null;
        return players[0];
    }

    public static void Pause()
    {
        GameObject pausePanel = GameObject.Find("Panels").transform.GetChild(2).gameObject;
        // Check if another panel is being displayed
        if (CurrentPanel == null)
        {
            // Display pause panel
            CurrentPanel = pausePanel;
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        else if (CurrentPanel == pausePanel)
        {
            CurrentPanel = null;
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public static void Win()
    {
        // Move on to the next objective
        CurrObjective++;
        // If there is no next objective, then the player won this level
        if(CurrObjective >= SceneObjectives.Count)
        {
            // TODO: Record the player's win and bonus objectives
            // Freeze the game
            Time.timeScale = 0;
            // Display win panel
            if (CurrentPanel != null) CurrentPanel.SetActive(false);
            GameObject winPanel = GameObject.Find("Panels").transform.GetChild(0).gameObject;
            CurrentPanel = winPanel;
            winPanel.SetActive(true);
        }
    }
    public static void Lose()
    {
        // Freeze the game
        Time.timeScale = 0;
        // Display loose panel
        if (CurrentPanel != null) CurrentPanel.SetActive(false);
        GameObject losePanel = GameObject.Find("Panels").transform.GetChild(1).gameObject;
        CurrentPanel = losePanel;
        losePanel.SetActive(true);
    }

    // Scene controls
    public static string Name(this Scene scene)
    {
        if (scene == Scene.MainMenu) return "MainMenu";
        if (scene == Scene.Home) return "Home";
        if (scene == Scene.Upgrades) return "Upgrades";
        if (scene == Scene.DebugScene) return "DebugScene";
        if (scene == Scene.Level1) return "Level1";
        if (scene == Scene.Level2) return "Level2";
        if (scene == Scene.Level3) return "Level3";
        return "NoSceneNameFound";
    }
    public static void Load(this Scene scene)
    {
        // Clean up the old scene
        Time.timeScale = 1.0f;
        CurrentPanel = null;

        // Setup the new scene
        CurrentScene = scene;
        SceneObjectives = scene.CreateObjectives();
        SceneManager.LoadScene(scene.Name());
    }
    public static void RestartScene()
    {
        CurrentScene.Load();
    }
    public static List<Objective> CreateObjectives(this Scene scene)
    {
        if (scene == Scene.MainMenu) return null;
        if (scene == Scene.Home) return null;
        if (scene == Scene.Upgrades) return null;
        if (scene == Scene.DebugScene) return new List<Objective>
        {
            new Objective("DebugObjective1", "Debug trigger zone test")
                .Task(TriggerZone.GetFlag("DebugTrigger1"))
            ,new Objective("DebugObjective2", "Debug kill 2 red enemy helicopters")
        };
        if (scene == Scene.Level1) return new List<Objective>
        {

        };
        if (scene == Scene.Level2) return new List<Objective>
        {

        };
        if (scene == Scene.Level3) return new List<Objective>
        {

        };
        return null;
    }
}

public enum Scene
{
    MainMenu,
    Home,
    Upgrades,
    DebugScene,
    Level1,
    Level2,
    Level3
}
