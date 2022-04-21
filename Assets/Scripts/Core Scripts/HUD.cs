using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Canvas ref
    public Canvas screen;
    // Health UI refs
    public Text health;
    public Image healthBar;
    // Crosshair ref
    public Image crosshair;
    public Vector2 targetPos;
    // Weapon UI refs
    public List<Text> weapons = new List<Text>(3);
    public Image weaponHighlight;
    // Objectives UI refs
    public static List<Text> objectives = new List<Text>(10);

    void Start()
    {
        // Find UI components
        health = transform.Find("HealthBar/Health").gameObject.GetComponent<Text>();   
        healthBar = transform.Find("HealthBar/FG").gameObject.GetComponent<Image>(); 
        crosshair = transform.Find("Crosshair").gameObject.GetComponent<Image>(); 
        weaponHighlight = transform.Find("WeaponHighlight").gameObject.GetComponent<Image>(); 
        foreach (Transform child in transform.Find("Weapons")) {
            weapons.Add(child.GetComponent<Text>());
        }
        objectives.Clear();
        foreach (Transform child in transform.Find("Objectives")) {
            objectives.Add(child.GetComponent<Text>());
        }
        weaponHighlight.enabled = false;
    }

    void Update()
    {
        // Get player
        Player currPlayer = GameManager.GetPlayer();
        if (currPlayer != null)
        {
            // Update Health UI
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
                if (GameManager.playerData.weapon1[0]) weapons[0].text = "-";
                else weapons[0].text = "X";
                if (GameManager.playerData.weapon2[0]) weapons[1].text = currPlayer.weaponAmmo[1].ToString();
                else weapons[1].text = "X";
                if (GameManager.playerData.weapon3[0]) weapons[2].text = currPlayer.weaponAmmo[2].ToString();
                else weapons[2].text = "X";
            }
            // Display weapon highlight
            if(currPlayer.currWeapon != 0) {
                weaponHighlight.enabled = true;
                if(currPlayer.currWeapon == 1) weaponHighlight.rectTransform.anchoredPosition = new Vector2(300, -170);
                if(currPlayer.currWeapon == 2) weaponHighlight.rectTransform.anchoredPosition = new Vector2(340, -170);
                if(currPlayer.currWeapon == 3) weaponHighlight.rectTransform.anchoredPosition = new Vector2(380, -170);
            }
            // Display crosshair
            if(currPlayer.currentCam == 1) {
                crosshair.enabled = true;
                if(currPlayer.targets.Count != 0) {
                    float distance = Mathf.Infinity;
                    foreach(Vector3 t in currPlayer.targets) {
                        Vector2 screenPos;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                screen.GetComponent<RectTransform>(),
                                t,
                                null,
                                out screenPos);
                        float crosshairDistance = screenPos.magnitude;
                        if(crosshairDistance < distance) {
                            targetPos = screenPos;
                            distance = crosshairDistance;
                        }
                    }
                    if(Mathf.Abs(targetPos.x) <= 50 && Mathf.Abs(targetPos.y) <= 50) {
                        crosshair.rectTransform.anchoredPosition = targetPos;
                        crosshair.color = new Color(1f, 0f, 0f, 1.0f);
                    } else {
                        crosshair.rectTransform.anchoredPosition = Vector2.zero;
                        crosshair.color = new Color(0f, 0f, 0f, 0.6f);
                    }
                } else {
                    crosshair.rectTransform.anchoredPosition = Vector2.zero;
                    crosshair.color = new Color(0f, 0f, 0f, 0.6f);
                }
            } else {
                crosshair.enabled = false;
            }
        } else {
            // Helicopter is dead
            healthBar.fillAmount = 0;
            health.text = "0.0";
        }
    }
}
