﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueHandler : MonoBehaviour {
    public List<string> dialogueLines;
    private int currentLineIndex = 0; 
    private Dictionary<string, Action> dialogueChoices;
    [HideInInspector] public Action afterDialogue;

    public string GetNextLine() {
        if (currentLineIndex < dialogueLines.Count) {
            return dialogueLines[currentLineIndex++];
        } else {
            return null;  // No more lines
        }
    }

    public void ResetDialogue() {
        currentLineIndex = 0;
    }

    public void AddDialogueChoice(string id, Action callBack) {
        Debug.Assert(dialogueChoices != null);
        Debug.Assert(!dialogueChoices.ContainsKey(id));
        dialogueChoices.Add(id, callBack);
    }

    public Action GetDialogueChoice(string id) {
        if (!dialogueChoices.ContainsKey(id)) {
            return null;
        }
        return dialogueChoices[id];
    }

    void Awake() {
        dialogueChoices = new Dictionary<string, Action>();
    }
    
    void Start() {
    }

    void Update() {
    }
}
