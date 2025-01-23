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
   
   private CharacterBattle playerCharacterBattle;
   private CharacterBattle enemyCharacterBattle;

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
               state = State.WaitingForPlayer;  // Return to waiting state after attack
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
}
