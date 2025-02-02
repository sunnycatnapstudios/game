using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [HideInInspector] public SpriteRenderer spriteRenderer;
    public bool isOpen, isLocked;
    public Sprite closed, open;

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
