using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractPrompt : MonoBehaviour
{
    public LayerMask playerLayer;
    public Player player;
    private float interactRange = 1.5f;
    private int interactCount = 0;

    // public GameObject popUpBox;
    // public TMP_Text popUpText;
    // public Animator animator;

    public GameObject popUpPrefab;
    private GameObject currentPopUp;
    private TMP_Text popUpText;
    private Animator animator;


    void OnDrawGizmos() { // Draws a Debug for NPC interact radius
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    public void PopUp(string text)
    {
        if (currentPopUp){
            popUpText.text = text;
            // animator.SetTrigger("pop");
        }
    }



    void Start()
    {

    }

    void Update()
    {
        bool playerInRange = Physics2D.OverlapCircle(transform.position, interactRange, playerLayer);

        if(playerInRange)
        {
            if (currentPopUp == null)
            {
                currentPopUp = Instantiate(popUpPrefab, Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f), Quaternion.identity, GameObject.Find("Overworld UI").transform);
                popUpText = currentPopUp.GetComponentInChildren<TMP_Text>();
                animator = currentPopUp.GetComponent<Animator>();
            } else
            {
                currentPopUp.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            }

            if (Input.GetKeyDown(KeyCode.E)||Input.GetKeyDown(KeyCode.Space)){
                interactCount++;
                Debug.Log("YEP, YOU'VE TAPPED ME "+interactCount+" TIMES!!!");
                PopUp("E");
            }
        } else
        {
            if (currentPopUp) {
                Destroy(currentPopUp);
                currentPopUp = null;
            }
        }
    }
}
