using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    private Player characterBase;           // Reference to a [player] object
    private State state;                    // State of current entity
    
    private Vector3 slideTargetPositon;     // position to slide to 
    private Action onSlideComplete;         // Callback on slide complete

    private bool isPlayerTeam;              // true - player team | false - enemy team
    
    private enum State
    {
        Idle,
        Sliding,
        Busy,
    }
    private void Awake()
    {
        characterBase = GetComponent<Player>();
        state = State.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Sliding:
                float slideSpeed = 6f;
                transform.position += (slideTargetPositon - GetPosition()) * (slideSpeed * Time.deltaTime);

                float reachedDistance = 1f;
                if (Vector3.Distance(GetPosition(), slideTargetPositon) < reachedDistance)
                {
                    // Arrived at slide target position
                    transform.position = slideTargetPositon;
                    onSlideComplete();
                }
                break;
            case State.Busy:
                break;
        }
    }
    
    // Called first in BattleHandler to setup this character
    public void Setup(bool isPlayerTeam)
    {
        this.isPlayerTeam = isPlayerTeam;
        if (isPlayerTeam)
        {
            // TODO if player, orientate characters and play their idle animation
            characterBase.anim.Play("Walk Down");
        }
        else
        {
            // TODO if enemy, orientate characters and play their idle animation
            characterBase.anim.Play("Walk Left");
        }
    }
    
    // Return the current position of this character
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    // Attack the targeted character
    public void Attack(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPositon = targetCharacterBattle.GetPosition() +  (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 2f;
        Vector3 startingPosition = GetPosition();
        
        SlideToPosition(slideTargetPositon, onSlideComplete: () =>
        {
            state = State.Busy;
            // After Slide complete, play attack animation
            Vector3 attackDir = targetCharacterBattle.GetPosition() - GetPosition().normalized;     // Direction vector toward enemy
            characterBase.anim.Play("Walk Up");     // TODO Attack animation, wait for anim complete
            characterBase.anim.Play("Walk Left");
            
            // Attack Complete, slide back
            SlideToPosition(startingPosition, onSlideComplete: () =>
            {
                // TODO set back to idle animation
                state = State.Idle;
                onAttackComplete();
            });
        });
        
    }


    private void SlideToPosition(Vector3 slideTargetPositon, Action onSlideComplete)
    {
        this.slideTargetPositon = slideTargetPositon;
        this.onSlideComplete = onSlideComplete;
        state = State.Sliding;
        if (slideTargetPositon.x > 0)
        {
            // TODO play slide right animation
        }
        else
        {
            // TODO play slide left animation
        }
    }
}
