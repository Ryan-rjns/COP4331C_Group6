using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    // Current level
    public static int currentLevel = 0;
    // Current panel
    private static GameObject currentPanel = null;

    // Debug: You can trigger this function by pressing "Z" while playing the game
    public static void Win()
    {
        // Check if another panel is being displayed
        if(currentPanel == null) {
            // Display win panel
            GameObject winPanel = GameObject.Find("Panels").transform.GetChild(0).gameObject;
            currentPanel = winPanel;
            winPanel.SetActive(true);
        }
    }

    // Debug: You can trigger this function by pressing "X" while playing the game
    public static void Lose()
    {
        // Check if another panel is being displayed
        if(currentPanel == null) {
            // Display lose panel
            GameObject losePanel = GameObject.Find("Panels").transform.GetChild(1).gameObject;
            currentPanel = losePanel;
            losePanel.SetActive(true);
        }
    }

    // Debug: You can trigger this function by pressing "C" while playing the game
    public static void PauseMenu()
    {
        GameObject pausePanel = GameObject.Find("Panels").transform.GetChild(2).gameObject;
        // Check if another panel is being displayed
        if(currentPanel == null) {
            // Display pause panel
            currentPanel = pausePanel;
            pausePanel.SetActive(true);
        } else if(currentPanel == pausePanel) {
            currentPanel = null;
            pausePanel.SetActive(false);
        }
    }
}
