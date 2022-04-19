using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class GameManager
{
    // Current scene
    public static Scene CurrentScene { get; private set; } = Scene.MainMenu;
    // Current UI panel
    public static GameObject CurrentPanel { get; private set; } = null;
    // Current HUD
    public static GameObject HUDPanel
    {
        get
        {
            GameObject panels = GameObject.Find("Panels");
            if (panels == null) return null;
            Transform HUD = panels.transform.GetChild(3);
            if (HUD == null) return null;
            return HUD.gameObject;
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
            SetHUDEnabled(false);
            LockCursor(false);
        }
        else if (CurrentPanel == pausePanel)
        {
            CurrentPanel = null;
            pausePanel.SetActive(false);
            Time.timeScale = 1;
            SetHUDEnabled(true);
            LockCursor(true);
        }
    }
    public static void Win()
    {
        // Manage save data
        if(playerData != null)
        {
            // Record the player's win
            bool firstNormalWin = false;
            bool firstHardWin = false;
            bool[] levelData = null;
            if (CurrentScene == Scene.Level1) levelData = playerData.level1;
            if (CurrentScene == Scene.Level2) levelData = playerData.level2;
            if (CurrentScene == Scene.Level3) levelData = playerData.level3;
            if (levelData != null)
            {
                // Level was won
                if (!levelData[0]) firstNormalWin = true;
                levelData[0] = true;
                // Level was won in hard mode
                if (playerData.difficultyHard)
                {
                    if (!levelData[1]) firstHardWin = true;
                    levelData[1] = true;
                }
            }
            // Give the player money
            if (firstNormalWin) playerData.money += 300;
            if (firstHardWin) playerData.money += 300;
            // Save the game
            SaveData();
        }

        // Freeze the game
        Time.timeScale = 0;
        SetHUDEnabled(false);
        LockCursor(false);
        // Display win panel
        if (CurrentPanel != null) CurrentPanel.SetActive(false);
        GameObject winPanel = GameObject.Find("Panels").transform.GetChild(0).gameObject;
        CurrentPanel = winPanel;
        winPanel.SetActive(true);
    }
    public static void Lose()
    {
        // Freeze the game
        Time.timeScale = 0;
        SetHUDEnabled(false);
        LockCursor(false);
        // Display loose panel
        if (CurrentPanel != null) CurrentPanel.SetActive(false);
        GameObject losePanel = GameObject.Find("Panels").transform.GetChild(1).gameObject;
        CurrentPanel = losePanel;
        losePanel.SetActive(true);
    }
    public static void SetHUDEnabled(bool enabled)
    {
        if (HUDPanel == null) return;
        HUDPanel.SetActive(enabled);
    }
    // Hides the cursor and locks it into the game
    public static void LockCursor(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    // Scene controls
    public static string SceneName(this Scene scene)
    {
        if (scene == Scene.MainMenu) return "MainMenu";
        if (scene == Scene.Home) return "Home";
        if (scene == Scene.DebugScene) return "DebugScene";
        if (scene == Scene.Level1) return "Level1";
        if (scene == Scene.Level2) return "Level2";
        if (scene == Scene.Level3) return "Level3";
        return "NoSceneNameFound";
    }
    public static void LoadScene(this Scene scene)
    {
        // Clean up the old scene
        Time.timeScale = 1.0f;
        CurrentPanel = null;

        // Setup the new scene
        CurrentScene = scene;
        //Debug.Log($"Loading Scene {scene.SceneName()}");
        SceneManager.LoadScene(scene.SceneName());
    }
    public static void RestartScene()
    {
        CurrentScene.LoadScene();
    }
    // Function to update Objectives UI
    public static void DisplayTask(int index, string task) {
        HUD.objectives[index].text = task;
    }
    
    // Save File path and extension (relative to Application.persistentDataPath)
    // Current Save File
    public static PlayerData playerData = null;

    // Saves the current SaveFile
    // If erase is true, this instead erases the current SaveFile and replaces it with a blank one
    public static bool SaveData(bool erase = false)
    {
        if (playerData == null) return false;

        BinaryFormatter formatter = new BinaryFormatter();
        string fullpath = Application.persistentDataPath + playerData.savePath;
        FileStream stream = new FileStream(fullpath, FileMode.Create);
        if(erase) playerData = new PlayerData(playerData.savePath);
        formatter.Serialize(stream, playerData);
        stream.Close();
        return true;
    }
    // Loads the specified save file, or creates a new one if there's no existing file.
    public static void LoadData(int saveSlot)
    {
        string savePath = $"/SaveFile{saveSlot}.savefile";
        playerData = null;
        string fullpath = Application.persistentDataPath + savePath;
        if (File.Exists(fullpath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(fullpath, FileMode.Open);
            playerData = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            // If the file exists and was read properly, return
            if (playerData != null) return;
        }
        // If the file was not found or if the read failed, create a new file
        playerData = new PlayerData(savePath);
        SaveData();
    }
    // Saves the current save file and then removes it from memory.
    public static void ClearData()
    {
        SaveData();
        playerData = null;
    }
}

public enum Scene
{
    MainMenu,
    Home,
    DebugScene,
    Level1,
    Level2,
    Level3
}

// Serializable class that stores all of the info for each save file
[System.Serializable]
public class PlayerData
{
    public string savePath = "/DefaultPath.savefile";
    public int money = 1000;
    // The first index is whether or not the level is cleared in normal mode. The 2nd is for hard mode.
    public bool[] level1 = new bool[2];
    public bool[] level2 = new bool[2];
    public bool[] level3 = new bool[2];
    // Each index corresponds to an upgrade button (top to bottom, for each upgrade)
    public bool[] weapon1 = new bool[3];
    public bool[] weapon2 = new bool[3];
    public bool[] weapon3 = new bool[3];
    // False = Normal, true = hard
    public bool difficultyHard = false;

    public PlayerData(string savePath)
    {
        this.savePath = savePath;
        weapon1[0] = true;
    }

    public override string ToString()
    {
        return $"PlayerData:[path:{savePath},money:{money},difficulty:{difficultyHard},"
            + $"level1:[{level1[0]},{level1[1]}],level2:[{level2[0]},{level2[1]}],level3:[{level3[0]},{level3[1]}],\n"
            + $"weapon1:[{weapon1[0]},{weapon1[1]},{weapon1[2]}],weapon2:[{weapon2[0]},{weapon2[1]},{weapon2[2]}],weapon3:[{weapon3[0]},{weapon3[1]},{weapon3[2]}]]";
    }
}
