using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySlot : MonoBehaviour
{
    public string Name;
    public Image profile;
    public Image healthBarBar, healthBarTail;
    public float maxHealth;
    public Vector3 defaultImagePosition;
    public bool _isHighlighted;
    public BattleUiHandler battleUiHandler;

    void OnEnable()
    {
        defaultImagePosition = profile.transform.localPosition;
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
    }

    public void UpdateHealthBar(float currentHealth)
    {
        healthBarBar.fillAmount = currentHealth / maxHealth;
    }

    void Update()
    {
        if (healthBarTail.fillAmount >= healthBarBar.fillAmount){
            healthBarTail.fillAmount = Mathf.Lerp(healthBarTail.fillAmount, healthBarBar.fillAmount, Time.unscaledDeltaTime * 5);
        } else {healthBarTail.fillAmount = healthBarBar.fillAmount;}

        if (_isHighlighted && battleUiHandler.canSelect) {
            HighlightImage();
        } else {
            UnHighlightImage();
        }
    }
}