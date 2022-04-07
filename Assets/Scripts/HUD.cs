using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Health UI refs
    public Text health;
    public Image healthBar;
    // Weapon UI refs
    public List<Text> weapons = new List<Text>(3);
    // Objectives UI refs
    public static List<Text> objectives = new List<Text>(10);

    void Start()
    {
        // Find UI components
        health = transform.Find("HealthBar/Health").gameObject.GetComponent<Text>();   
        healthBar = transform.Find("HealthBar/FG").gameObject.GetComponent<Image>(); 
        foreach (Transform child in transform.Find("Weapons")) {
            weapons.Add(child.GetComponent<Text>());
        }
        objectives.Clear();
        foreach (Transform child in transform.Find("Objectives")) {
            objectives.Add(child.GetComponent<Text>());
        }
    }

    void Update()
    {
        // Update Health UI
        Player currPlayer = GameManager.GetPlayer();
        if (currPlayer != null)
        {
            float currHealth = Mathf.Clamp(currPlayer.Health, 0, currPlayer.MaxHealth);
            float percent = currHealth / currPlayer.MaxHealth;
            healthBar.fillAmount = percent;
            if (percent < 0.25f)
            {
                healthBar.color = Color.red;
            }
            else
            {
                healthBar.color = Color.green;
            }
            health.text = currHealth.ToString("N1");
            // Update Weapons UI
            if (GameManager.playerData != null)
            {
                if (GameManager.playerData.weapon1[0]) weapons[0].text = currPlayer.weapon1.ToString();
                if (GameManager.playerData.weapon2[0]) weapons[1].text = currPlayer.weapon2.ToString();
                if (GameManager.playerData.weapon3[0]) weapons[2].text = currPlayer.weapon3.ToString();
            }
        }
    }
}
