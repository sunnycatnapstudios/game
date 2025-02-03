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

    public GameObject popUpPrefab;
    private GameObject currentPopUp;
    private TMP_Text popUpText;
    private Animator animator;

    public GameObject dialogueBox; // Prefab for dialogue box
    private GameObject currentDialogueBox;
    private TMP_Text nameText, dialogueText;
    private Animator dialogueAnimator;

    public bool isDialogueOpen = false;


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

    // void OpenDialogue(string text)
    // {
    //     if (currentDialogueBox == null)
    //     {
    //         // Gets the locations of text items
    //         GameObject npcDialogue = GameObject.FindGameObjectWithTag("Dialogue Text");
    //         GameObject nameCard = GameObject.FindGameObjectWithTag("Name Card");

    //         dialogueText = npcDialogue.GetComponent<TMP_Text>();
    //         nameText = nameCard.GetComponent<TMP_Text>();
    //         nameText.text = this.name;
    //         dialogueText.text = text;

    //         // Instantiate dialogue box and parent to UI Canvas
    //         GameObject canvas = GameObject.FindGameObjectWithTag("Overworld UI");
    //         currentDialogueBox = Instantiate(dialogueBoxPrefab, canvas.transform);

    //         RectTransform rectTransform = currentDialogueBox.GetComponent<RectTransform>();
    //         rectTransform.anchoredPosition = new Vector2(0f, 75f);


            

    //         dialogueAnimator = currentDialogueBox.GetComponent<Animator>();
    //     }
        
    //     dialogueAnimator.SetTrigger("SlideIn");
    //     isDialogueOpen = true;
    // }

    void OpenDialogue(string text)
    {
        dialogueBox.SetActive(true);
        dialogueText.text = text;
        dialogueAnimator.SetTrigger("SlideIn");
        isDialogueOpen = true;
    }

    void CloseDialogue()
    {
        // if (currentDialogueBox != null && dialogueAnimator != null)
        if (isDialogueOpen)
        {
            dialogueAnimator.ResetTrigger("SlideIn");
            dialogueAnimator.Play("Dialogue Disappear");
            dialogueAnimator.SetTrigger("SlideOut"); // Trigger the slide-out animation
            // StartCoroutine(DestroyAfterAnimation(currentDialogueBox, 0.5f)); // Wait for animation to finish before destroying
            StartCoroutine(DeactivateAfterDelay(0.5f));
            isDialogueOpen = false;
        }
    }

    void UpdateDialogue(string text)
    {
        dialogueText.text = text;
    }

    IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueBox.SetActive(false);
    }

    void Start()
    {
        if (CompareTag("NPC")) {

            nameText = GameObject.FindGameObjectWithTag("Name Card").GetComponent<TMP_Text>();
            nameText.text = this.name;
            dialogueText = GameObject.FindGameObjectWithTag("Dialogue Text").GetComponent<TMP_Text>();

            dialogueAnimator = dialogueBox.GetComponent<Animator>();
            
            dialogueBox.SetActive(false);
        }
    }

    void Update()
    {
        bool playerInRange = Physics2D.OverlapCircle(transform.position, interactRange, playerLayer);

        if(playerInRange)
        {
            if (currentPopUp == null)
            {
                currentPopUp = Instantiate(popUpPrefab, Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f), Quaternion.identity, GameObject.FindGameObjectWithTag("Overworld UI").transform);
                popUpText = currentPopUp.GetComponentInChildren<TMP_Text>();
                animator = currentPopUp.GetComponent<Animator>();
            } else
            {
                currentPopUp.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            }

            if (Input.GetKeyDown(KeyCode.E)){
                interactCount++;
                if (CompareTag("Interactable")) {
                    Debug.Log($"YEP, YOU'VE TAPPED {this.name} {interactCount} TIMES!!!");
                    // PopUp("E");
                }
                else if (CompareTag("NPC")) {
                    Debug.Log($"Yep, Dialogue's supposed to appear here {interactCount} times...");
                    if (!isDialogueOpen) {isDialogueOpen = true; OpenDialogue("Yo?");}
                    else {UpdateDialogue("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");}
                    
                }
            }
        } else
        {
            if (currentPopUp) {
                Destroy(currentPopUp);
                currentPopUp = null;
            }
            if (isDialogueOpen) {
                CloseDialogue();
            }
        }
    }
}
