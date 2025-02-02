using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask playerLayer;
    public Player player;
    private float interactRange = 1.5f;
    private int interactCount = 0;
    public GameObject popUpBox;
    public Text popUpText;
    public Animator animator;

    public void PopUp(string text)
    {
        popUpBox.SetActive(true);
        popUpText.text = text;
        animator.SetTrigger("pop");
    }



    void Start()
    {
        popUpBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.localPosition+(Vector3.left*interactRange)); //Draws a Line Showing the Interact radiuse
        popUpBox.transform.localPosition = transform.position + Vector3.up*50;

        if(Physics2D.OverlapCircle(transform.position, interactRange, playerLayer)){
            popUpBox.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E)||Input.GetKeyDown(KeyCode.Space)){
                interactCount++;
                Debug.Log("YEP, YOU'VE TAPPED ME "+interactCount+" TIMES!!!");
                PopUp("E");
            }
        }else {popUpBox.SetActive(false);}
    }
}
