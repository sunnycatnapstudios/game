using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
   // Expose this instance to other functions
   private static BattleHandler instance;
   public static BattleHandler GetInstance()
   {
      return instance;
   }
   
   [SerializeField] private Transform pfCharacterBattle;
   
   // TODO replace with list of char
   private CharacterBattle playerCharacterBattle;
   private CharacterBattle enemyCharacterBattle;
   
   private CharacterBattle activeCharacterBattle;     // The active character in battle

   private State state;
   
   private enum State
   {
      WaitingForPlayer,
      Busy,
   }
   private void Awake()
   {
      instance = this;
   }

   private void Start()
   {
      // TEMP
      playerCharacterBattle = SpawnCharacter(true);
      enemyCharacterBattle = SpawnCharacter(false);

      SetActiveCharacterBattle(playerCharacterBattle);
      state = State.WaitingForPlayer;
   }

   private void Update()
   {
      if (state == State.WaitingForPlayer)
      {
         if (Input.GetKeyDown(KeyCode.Space))   // TODO attack when pressing space bar for now
         {
            state = State.Busy;
            playerCharacterBattle.Attack(enemyCharacterBattle, onAttackComplete: () =>
            {
               ChooseNextActiveCharacter();     // Next character gets turn
            });
         }
      }
   }

   private CharacterBattle SpawnCharacter(bool isPlayerTeam)
   {
      Vector3 position;
      
      // Spawn on either left or right hand side
      if (isPlayerTeam)
      {
         position = new Vector3(-5, 0);
      }
      else
      {
         position = new Vector3(5, 0);
      }
      
      Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity); // Spawn into scene
      CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();  // Retrieve the Character Battle instance
      characterBattle.Setup(isPlayerTeam);
      
      return characterBattle;
   }
   
   // Assign activeCharacterBattle variable. Used in ChooseNextActiveCharacter
   private void SetActiveCharacterBattle(CharacterBattle characterBattle)
   {
      // Hide the selection circle
      if (activeCharacterBattle != null)
      {
         activeCharacterBattle.HideSelectionCircle();
      }
      
      // Set new character, show selection circle
      activeCharacterBattle = characterBattle;
      activeCharacterBattle.ShowSelectionCircle();
   }

   // Give the turn to the next character
   private void ChooseNextActiveCharacter()
   {
      if (TestBattleOver())
      {
         return;
      }
      
      if (activeCharacterBattle == playerCharacterBattle)
      {
         SetActiveCharacterBattle(enemyCharacterBattle);
         
         // TODO Enemy Auto attack player
         enemyCharacterBattle.Attack(playerCharacterBattle, onAttackComplete: () =>
         {
            ChooseNextActiveCharacter();     // Next character gets turn
         });
      }
      else
      {
         SetActiveCharacterBattle(playerCharacterBattle);
         state = State.WaitingForPlayer;
      }
   }

   private bool TestBattleOver()
   {
      if (playerCharacterBattle.IsDead())
      {
         // Player dead, enemy wins
         return true;
      }

      if (enemyCharacterBattle.IsDead())
      {
         // Enemy dead, player wins
         return true;
      }

      return false;
   }
}
