using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySlot : MonoBehaviour
{
    public Image profile;
    public Image healthBarBar, healthBarTail;

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBarBar.fillAmount = currentHealth / maxHealth;
    }

    void Update()
    {
        if (healthBarTail.fillAmount >= healthBarBar.fillAmount){
            healthBarTail.fillAmount = Mathf.Lerp(healthBarTail.fillAmount, healthBarBar.fillAmount, Time.unscaledDeltaTime * 5);
        } else {healthBarTail.fillAmount = healthBarBar.fillAmount;}
    }
}