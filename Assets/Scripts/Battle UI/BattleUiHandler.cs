using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUiHandler : MonoBehaviour
{
    public Animator partyUIAnimator;
    public bool attackOption = false, itemOption = false;
    public GameObject overworldUI, combatUI;

    public void Attack()
    {
        Debug.Log("Attack Button Pressed");
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (attackOption) {
                partyUIAnimator.SetTrigger("Close");
                attackOption = false;
            } else if (itemOption) {
                partyUIAnimator.SetTrigger("Reset");
                itemOption = false;
                attackOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                attackOption = true;
            }
        }
    }
    public void Item()
    {
        Debug.Log("Item Button Pressed");
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption) {
                partyUIAnimator.SetTrigger("Close");
                itemOption = false;
            } else if (attackOption) {
                partyUIAnimator.SetTrigger("Reset");
                attackOption = false;
                itemOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                itemOption = true;
            }
        }
    }
    public void Escape()
    {
        Debug.Log("Escape Button Pressed");
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption||attackOption) {
                partyUIAnimator.SetTrigger("Close");
                attackOption = itemOption = false;
            }
        }
        overworldUI.SetActive(true);
        combatUI.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void Trigger()
    {
        // if (Time.timeScale == 1) {Time.timeScale = 0;}
        // else {Time.timeScale = 1;}
    }

    void Start()
    {
        partyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
