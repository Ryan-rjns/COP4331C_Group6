using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// Functions for changing scene or exiting game
public class MenuButtons : MonoBehaviour
{
    // Weapon/Upgrade costs
    public const int WEAPON_PRICE = 100;
    public const int UPGRADE_1_PRICE = 200;
    public const int UPGRADE_2_PRICE = 300;
    // UI refs
    private static Text money;
    private Text feedback;
    private Text difficulty;
    // Button list
    private List<Button> buttonList = new List<Button>();
    // Scene Navigation
    public void MainMenu()
    {
        // Drop the save file
        GameManager.ClearData();
        // And return to the main menu
        Scene.MainMenu.LoadScene();
    }
    public void Home() => Scene.Home.LoadScene();
    public void DebugScene() => Scene.DebugScene.LoadScene();
    public void Level1() => Scene.Level1.LoadScene();
    public void Level2() => Scene.Level2.LoadScene();
    public void Level3() => Scene.Level3.LoadScene();
    public void RestartLevel() => GameManager.RestartScene();
    public void Exit() => Application.Quit();
    public void ToggleDifficulty() {
        if(GameManager.playerData == null)
        {
            Debug.LogError("Toggle Difficulty: No save file is loaded!");
            return;
        }
        GameManager.playerData.difficultyHard = !GameManager.playerData.difficultyHard;
        difficulty.text = GameManager.playerData.difficultyHard ? "Hard" : "Easy";
    }
    public void LoadSaveSlot(int saveSlot) {
        GameManager.LoadData(saveSlot);
        Home();
    }
    public void EraseSaveData()
    {
        // Overwrite the save file with a blank one
        GameManager.SaveData(true);
        // Drop the save file
        GameManager.ClearData();
        // And return to the main menu
        Scene.MainMenu.LoadScene();
    }

    void Start() {
        if (GameManager.playerData == null)
        {
            //Debug.Log("Menu Start: No save file was found");
            return;
        }
        //Debug.Log($"Menu Start: A save file is currently loaded: {GameManager.playerData}");
        Button myButton = gameObject.GetComponent<Button>();

        // Level Select screen
        if (name.Equals("Difficulty")) {
            // Get ref to UI
            difficulty = gameObject.GetComponentInChildren<Text>();
            // Set UI text
            difficulty.text = GameManager.playerData.difficultyHard ? "Hard" : "Easy";
        }
        if (myButton != null) 
        {
            bool[] levelData = null;
            bool levelUnlocked = false;
            if (name.Equals("Level 1"))
            {
                levelData = GameManager.playerData.level1;
                levelUnlocked = true;
            }
            if (name.Equals("Level 2"))
            {
                levelData = GameManager.playerData.level2;
                levelUnlocked = GameManager.playerData.level1[0];
            }
            if (name.Equals("Level 3"))
            {
                levelData = GameManager.playerData.level3;
                levelUnlocked = GameManager.playerData.level2[0];
            }
            if (levelData != null)
            {
                myButton.interactable = true;
                if (levelData[1]) myButton.SetColor(Color.green);
                else if (levelData[0]) myButton.SetColor(Color.blue);
                else if (levelUnlocked) myButton.SetColor(Color.yellow);
                else
                {
                    myButton.interactable = false;
                    myButton.SetColor(Color.gray);
                }
            }
        }

        // Upgrades Screen
        if (name.Equals("Money"))
        {
            // Get ref to UI
            money = gameObject.GetComponent<Text>();
            // Compute player's money
            getMoney();
            // Get buttons gameobject
            GameObject buttons = GameObject.Find("Buttons");
            // Add buttons to button list
            foreach (Transform child in buttons.transform)
            {
                if(child == null) continue;
                Button button = child.GetComponent<Button>();
                if(button == null || buttonList.Contains(button)) continue;
                buttonList.Add(button);
            }
            // Disable bought weapons/upgrades
            for (int i = 0; i < 3; i++)
            {
                if (GameManager.playerData.weapon1[i]) buttonList[i].interactable = false;
                if (GameManager.playerData.weapon2[i]) buttonList[i + 3].interactable = false;
                if (GameManager.playerData.weapon3[i]) buttonList[i + 6].interactable = false;
            }
        }
    }


    // Returns current player's money
    public void getMoney() {
        money.text = "Money: " + GameManager.playerData.money.ToString();
    }
    // Purchase weapon/upgrade and saves game
    public void purchase(Button b) {
        int success = 0;

        if (GameManager.playerData == null) {
            return;
        }
        // Purchase weapon/upgrade
        switch(b.name) {
            case "Weapon 1":
                if (GameManager.playerData.money >= WEAPON_PRICE)
                {
                    GameManager.playerData.money -= WEAPON_PRICE;
                    GameManager.playerData.weapon1[0] = true;
                    success = 1;
                }
                break;
            case "Weapon 2":
                if (GameManager.playerData.money >= WEAPON_PRICE)
                {
                    GameManager.playerData.money -= WEAPON_PRICE;
                    GameManager.playerData.weapon2[0] = true;
                    success = 1;
                }
                break;
            case "Weapon 3":
                if (GameManager.playerData.money >= WEAPON_PRICE)
                {
                    GameManager.playerData.money -= WEAPON_PRICE;
                    GameManager.playerData.weapon3[0] = true;
                    success = 1;
                }
                break;
            case "Weapon 1 Upgrade 1":
                if (GameManager.playerData.money >= UPGRADE_1_PRICE)
                {
                    if(GameManager.playerData.weapon1[0]) {
                        success = 1;
                    } else {
                        success = 2;
                        break;
                    }
                    GameManager.playerData.money -= UPGRADE_1_PRICE;
                    GameManager.playerData.weapon1[1] = true;
                }
                break;
            case "Weapon 1 Upgrade 2":
                if (GameManager.playerData.money >= UPGRADE_2_PRICE)
                {
                    if(GameManager.playerData.weapon1[0]) {
                        success = 1;
                    } else {
                        success = 2;
                        break;
                    }
                    GameManager.playerData.money -= UPGRADE_2_PRICE;
                    GameManager.playerData.weapon1[2] = true;
                }
                break;
            case "Weapon 2 Upgrade 1":
                if (GameManager.playerData.money >= UPGRADE_1_PRICE)
                {
                    if(GameManager.playerData.weapon2[0]) {
                        success = 1;
                    } else {
                        success = 2;
                        break;
                    }
                    GameManager.playerData.money -= UPGRADE_1_PRICE;
                    GameManager.playerData.weapon2[1] = true;
                }
                break;
            case "Weapon 2 Upgrade 2":
                if (GameManager.playerData.money >= UPGRADE_2_PRICE)
                {
                    if(GameManager.playerData.weapon2[0]) {
                        success = 1;
                    } else {
                        success = 2;
                        break;
                    }
                    GameManager.playerData.money -= UPGRADE_2_PRICE;
                    GameManager.playerData.weapon2[2] = true;
                }
                break;
            case "Weapon 3 Upgrade 1":
                if (GameManager.playerData.money >= UPGRADE_1_PRICE)
                {
                    if(GameManager.playerData.weapon3[0]) {
                        success = 1;
                    } else {
                        success = 2;
                        break;
                    }
                    GameManager.playerData.money -= UPGRADE_1_PRICE;
                    GameManager.playerData.weapon3[1] = true;
                }
                break;
            case "Weapon 3 Upgrade 2":
                if (GameManager.playerData.money >= UPGRADE_2_PRICE)
                {
                    if(GameManager.playerData.weapon3[0]) {
                        success = 1;
                    } else {
                        success = 2;
                        break;
                    }
                    GameManager.playerData.money -= UPGRADE_2_PRICE;
                    GameManager.playerData.weapon3[2] = true;
                }
                break;
            // Should never happen
            default:
                break;
        }
        Text buttonText = null;
        Transform buttonTextTransform = b.transform.GetChild(0);
        if(buttonTextTransform != null)  buttonText = buttonTextTransform.gameObject.GetComponent<Text>();

        // Update player's money
        money.text = "Money: " + GameManager.playerData.money.ToString();
        // Display feedback
        feedback = GameObject.Find("Feedback/Purchase").gameObject.GetComponent<Text>();
        
        if(success == 1) {
            // Disable button
            b.interactable = false;
            string purchaseName = b.name;
            if(buttonText != null) purchaseName = buttonText.text;
            feedback.text = "Purchased " + purchaseName + "!";
        } else if(success == 2) {
            feedback.text = "You do not have that weapon!";
        } else {
            feedback.text = "You do not have enough money!";
        }
        // Save game
        GameManager.SaveData();
    }
}