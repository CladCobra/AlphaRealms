using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    [Header("Health")]
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image backHealthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField][Range(0, 1000)] private int maxHealth;
    [SerializeField][Range(0, 10)] private float chipSpeed;
    private float health;
    private float lerpTimer;

    [Header("Damage Overlay")]
    [SerializeField] private Image damageOverlay;
    [SerializeField][Range(0, 10)] private float duration;
    [SerializeField][Range(1, 5)] private float fadeSpeed;
    private float durationTimer;

    private void Start() {

        health = maxHealth;

        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 0);

    }

    private void Update() {

        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if (damageOverlay.color.a > 0) {

            if (health < 30) {

                return;

            }

            durationTimer += Time.deltaTime;

            if (durationTimer > duration) {

                float tempAlpha = damageOverlay.color.a;
                tempAlpha -= fadeSpeed * Time.deltaTime;
                damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, tempAlpha);

            }
        }
    }

    public void UpdateHealthUI() {

        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float healthFraction = health / maxHealth;

        healthText.text = health + "/" + maxHealth;

        if (fillFront < healthFraction) {

            backHealthBar.fillAmount = healthFraction;
            backHealthBar.color = Color.green;

            lerpTimer += Time.deltaTime;
            float percentCompletion = lerpTimer / chipSpeed;
            percentCompletion *= percentCompletion;

            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, backHealthBar.fillAmount, percentCompletion);

        }

        if (fillBack > healthFraction) {

            frontHealthBar.fillAmount = healthFraction;
            backHealthBar.color = Color.red;

            lerpTimer += Time.deltaTime;
            float percentCompletion = lerpTimer / chipSpeed;
            percentCompletion *= percentCompletion;

            backHealthBar.fillAmount = Mathf.Lerp(fillBack, healthFraction, percentCompletion);

        }
    }

    public void RestoreHealth(float healAmount) {

        health += healAmount;
        lerpTimer = 0;

    }

    public void TakeDamage(float damage) {

        health -= damage;
        lerpTimer = 0;

        durationTimer = 0;
        damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 1);

    }
}
