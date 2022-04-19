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
    private Text money;
    private Text feedback;
    private Text difficulty;
    // Button list
    private List<Button> buttonList = new List<Button>();
    // Scene Navigation
    public void MainMenu() => Scene.MainMenu.LoadScene();
    public void Home() => Scene.Home.LoadScene();
    public void DebugScene() => Scene.DebugScene.LoadScene();
    public void Level1() => Scene.Level1.LoadScene();
    public void Level2() => Scene.Level2.LoadScene();
    public void Level3() => Scene.Level3.LoadScene();
    public void RestartLevel() => GameManager.RestartScene();
    public void Exit() => Application.Quit();
    public void ToggleDifficulty() {
        GameManager.playerData.difficultyHard = !GameManager.playerData.difficultyHard;
        difficulty.text = GameManager.playerData.difficultyHard ? "Hard" : "Easy";
    }
    public void LoadSaveSlot(int saveSlot) {
        GameManager.LoadData(saveSlot);
        Home();
    }

    void Start() {
        if (name.Equals("Money")) {
            if (GameManager.playerData == null) {
                return;
            }
            // Get ref to UI
            money = gameObject.GetComponent<Text>();
            // Compute player's money
            getMoney();
            // Get buttons gameobject
            GameObject buttons = GameObject.Find("Buttons");
            // Add buttons to button list
            foreach(Transform child in buttons.transform) {
                buttonList.Add(child.GetComponent<Button>());
            }
            // Disable bought weapons/upgrades
            for(int i = 0; i < 3; i++) {
                if(GameManager.playerData.weapon1[i]) buttonList[i].interactable = false; 
                if(GameManager.playerData.weapon2[i]) buttonList[i + 3].interactable = false; 
                if(GameManager.playerData.weapon3[i]) buttonList[i + 6].interactable = false; 
            }
        }
        if (name.Equals("Difficulty")) {
            if (GameManager.playerData == null) {
                return;
            }
            // Get ref to UI
            difficulty = gameObject.GetComponentInChildren<Text>();
            // Set UI text
            if(GameManager.playerData.difficultyHard) {
                difficulty.text = "Hard";
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
        // Update player's money
        money.text = "Money: " + GameManager.playerData.money.ToString();
        // Display feedback
        feedback = GameObject.Find("Feedback/Purchase").gameObject.GetComponent<Text>();
        if(success == 1) {
            // Disable button
            b.interactable = false;
            feedback.text = "Purchased " + b.name + "!";
        } else if(success == 2) {
            feedback.text = "You do not have that weapon!";
        } else {
            feedback.text = "You do not have enough money!";
        }
        // Save game
        GameManager.SaveData();
    }
}