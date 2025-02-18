﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartySlot : MonoBehaviour
{
    public string Name;
    public Image profile;
    public GameObject healthbarCasing;
    public Image healthBarBar, healthBarTail;
    public float maxHealth;
    public Vector3 defaultImagePosition, initialBarPosition;
    public bool _isHighlighted;
    public BattleUiHandler battleUiHandler;
    public TextMeshProUGUI playerHealthIndicator;
    private float fadeDuration = .5f, delayBeforeFade = 1.2f;

    void OnEnable()
    {
        defaultImagePosition = profile.transform.localPosition;
        initialBarPosition = healthbarCasing.transform.localPosition;
        UpdateHealthBar(maxHealth);
        playerHealthIndicator.color = new Color(playerHealthIndicator.color.r, playerHealthIndicator.color.g, playerHealthIndicator.color.b, 0f);
    }

    public void SelectTarget()
    {
        if (battleUiHandler != null && Name != "" && battleUiHandler.canSelect)
        {
            battleUiHandler.ReceiveTargetSelection(Name);
        }
    }

    public void isHighlighted() {_isHighlighted = true;}
    public void isNotHighlighted() {_isHighlighted = false;}

    public void HighlightImage()
    {
        if (profile.transform.localPosition.y < defaultImagePosition.y+10) {
            profile.transform.localPosition += Vector3.up*10f*10f*Time.unscaledDeltaTime;
        } else if (profile.transform.localPosition.y > defaultImagePosition.y+10) {
            profile.transform.localPosition = defaultImagePosition+Vector3.up*10f;
        }
    }
    public void UnHighlightImage()
    {
        if (profile.transform.localPosition.y > defaultImagePosition.y) {
            profile.transform.localPosition -= Vector3.up*10f*10f*Time.unscaledDeltaTime;
        } else if (profile.transform.localPosition.y < defaultImagePosition.y) {
            profile.transform.localPosition = defaultImagePosition;
        }
    }

    public void SetHealth(float currentHealth)
    {
        maxHealth = currentHealth;
        healthBarBar.fillAmount = currentHealth/maxHealth;
        UpdateHealthBar(currentHealth);
    }

    public void UpdateHealthBar(float currentHealth)
    {
        if (currentHealth == 0) {healthBarBar.fillAmount = 0;}
        else {healthBarBar.fillAmount = currentHealth / maxHealth;}
        
    }

    public IEnumerator JutterHealthBar(float duration, float strength)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float jutterAmount = Mathf.Sin(elapsedTime * 30f) * strength;  // Small oscillations
            healthbarCasing.transform.localPosition = initialBarPosition + new Vector3(jutterAmount, 0f, 0f); // Jutter left/right

            elapsedTime += Time.unscaledDeltaTime;
            yield return null; // Wait for next frame
        }

        healthbarCasing.transform.localPosition = initialBarPosition; // Return to original position after jutter
    }

    private IEnumerator FadeOutHealthText()
    {
        // if (!playerHealthIndicator.color.a.Equals(1f))
        // {
        //     playerHealthIndicator.text = ((int)(healthBarTail.fillAmount * 100f)).ToString() + "%";
        //     playerHealthIndicator.color = new Color(playerHealthIndicator.color.r, playerHealthIndicator.color.g, playerHealthIndicator.color.b, 1f); // Fully opaque
        // }
        playerHealthIndicator.color = new Color(playerHealthIndicator.color.r, playerHealthIndicator.color.g, playerHealthIndicator.color.b, 1f);
        float timeElapsed = 0f;
        
        yield return new WaitForSecondsRealtime(delayBeforeFade); // Wait for the specified delay before starting fade out

        Color startColor = playerHealthIndicator.color;

        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
            playerHealthIndicator.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            timeElapsed += Time.unscaledDeltaTime; // Increment time

            yield return null; // Wait for the next frame
        }

        // Ensure the text is fully transparent at the end
        playerHealthIndicator.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        // playerHealthIndicator.text = ""; //
    }

    public void ShowHealthChange()
    {
        // Show health text and start fading it out
        // if (playerHealthIndicator.color.a == 0) // If it was previously invisible
        {
            StopAllCoroutines(); // Stop previous coroutines
            StartCoroutine(FadeOutHealthText());
        }
    }

    void Update()
    {
        if (healthBarTail.fillAmount > healthBarBar.fillAmount && healthBarBar.fillAmount != 0){
            healthBarTail.fillAmount = Mathf.Lerp(healthBarTail.fillAmount, healthBarBar.fillAmount, Time.unscaledDeltaTime * 5);
        } else if (healthBarBar.fillAmount == 0) {
            healthBarTail.fillAmount = 0;
        } else {healthBarTail.fillAmount = healthBarBar.fillAmount;}

        if (_isHighlighted && battleUiHandler.canSelect) {
            HighlightImage();
        } else {
            UnHighlightImage();
        }

        // playerHealthIndicator.text = (((int)(healthBarTail.fillAmount*100f)).ToString()+"%");
        if (healthBarBar.fillAmount == 0) {playerHealthIndicator.text = "";}
        else {
            playerHealthIndicator.text = (((int)(healthBarTail.fillAmount*100f)).ToString()+"%");
        }
    }
}
