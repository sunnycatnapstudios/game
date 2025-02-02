using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public bool isOpen, isLocked;
    public Sprite closed, open;
    bool interactable = false;

    // Start is called before the first frame update
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (isOpen) {
            Open();
        } else {
            Close();
        }
    }

    void Update() {
        if (interactable && Input.GetKeyDown(KeyCode.E)) {
            Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D collison) {
        if (collison.gameObject.tag == "Player") {
            interactable = true;
        }
    }

    void OnTriggerExit2D(Collider2D collison) {
        if (collison.gameObject.tag == "Player") {
            interactable = false;
        }
    }

    void Interact() {
        if (isLocked) {
            if (true/* key */) {
                Unlock();
            }
        } else {
            if (isOpen) {
                Close();
            } else {
                Open();
            }
        }
    }

    void Close() {
	spriteRenderer.sprite = closed;
	isOpen = false;
        this.gameObject.layer = LayerMask.NameToLayer("Can't Traverse");
    }

    void Open() {
	spriteRenderer.sprite = open;
	isOpen = true;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    void Lock() {
        isLocked = true;
    }

    void Unlock() {
	isLocked = false;
    }
}
