using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
   // Expose this instance to other functions
   private static BattleHandler instance;
   public static BattleHandler GetInstance()
   {
      return instance;
   }
   
   // Place the player party and enemy encounter under their own empty parent object
   [SerializeField] private Transform playerParty; 
   [SerializeField] private Transform enemyEncounter;
   
   private List<CharacterBattle> playerCharacterBattles = new List<CharacterBattle>();    // List of all player characters (scripts)
   private List<CharacterBattle> enemiesCharacterBattles = new List<CharacterBattle>();   // List of all enemy characters scripts
   private List<CharacterBattle> characterBattlesTurnOrder = new List<CharacterBattle>();
   
   //private int currentTurn;
   private int playerOffset = 0;
   private int enemyOffset = 0;
   
   private CharacterBattle activeCharacterBattle;     // The active character in battle
   private int activeCharacterIndex = 0;
   private int totalCharacterCount;
   
   private State state;    // Current state of combat
   
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
      foreach (Transform child in playerParty)
      {
         playerCharacterBattles.Add(SpawnCharacter(child, true));
      }

      foreach (Transform child in enemyEncounter)
      {
         enemiesCharacterBattles.Add(SpawnCharacter(child, false));
      }

      // Determine the turn order
      createTurnOrder();
      SetActiveCharacterBattle(characterBattlesTurnOrder[activeCharacterIndex++]);  // Fastest Character Goes first
      state = State.WaitingForPlayer;
   }

   private void Update()
   {
      if (state == State.WaitingForPlayer)
      {
         if (Input.GetKeyDown(KeyCode.Space))   // TODO attack when pressing space bar for now
         {
            state = State.Busy;
            
            // TODO need to get selection on who's being attacked
            activeCharacterBattle.Attack(enemiesCharacterBattles[1], onAttackComplete: () =>
            {
               ChooseNextActiveCharacter();     // Next character gets turn
            });
         }
      }
   }

   private CharacterBattle SpawnCharacter(Transform pfCharacterBattle, bool isPlayerTeam)
   {
      Vector3 position;

      
      // Spawn on either left or right hand side
      if (isPlayerTeam)
      {
         position = new Vector3(-3 - playerOffset, 0);
         playerOffset += 1;
      }
      else
      {
         position = new Vector3(3 + enemyOffset, 0);
         enemyOffset += 1;
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
      
      // Get the next character
      SetActiveCharacterBattle(GetNextCharacterBattler());
      
      // If it's an enemy they auto attack
      if (enemiesCharacterBattles.Contains(activeCharacterBattle))
      {
         // TODO Enemy Auto attack (random) player
         activeCharacterBattle.Attack(playerCharacterBattles[0], onAttackComplete: () =>
         {
            ChooseNextActiveCharacter();     // Next character gets turn
         });
      }
      else
      {
         // Back to waiting on player input
         state = State.WaitingForPlayer;
      }
   }

   // Decide the turn order based on speed
   // For now alternates between player and opponent
   private void createTurnOrder()
   {
      for (int i = 0; i < playerCharacterBattles.Count; i++)
      {
         characterBattlesTurnOrder.Add(playerCharacterBattles[i]);
      }

      for (int i = 0; i < enemiesCharacterBattles.Count; i++)
      {
         characterBattlesTurnOrder.Add(enemiesCharacterBattles[i]);
      }
      characterBattlesTurnOrder.Sort();
      totalCharacterCount = characterBattlesTurnOrder.Count;
   }

   // Get the next person in the turn order list
   private CharacterBattle GetNextCharacterBattler()
   {
      int next = activeCharacterIndex;
      activeCharacterIndex = (activeCharacterIndex + 1) % totalCharacterCount;
      return characterBattlesTurnOrder[next];
   }
   
   private bool TestBattleOver()
   {
      bool gameOver = true;
      foreach (CharacterBattle character in playerCharacterBattles)
      {
         if (!character.IsDead())
         {
            gameOver = false;
            break;
         }
      }

      if (gameOver)
      {
         // Players dead, enemy wins
         return true;
      }

      gameOver = true;
      foreach (CharacterBattle character in enemiesCharacterBattles)
      {
         if (!character.IsDead())
         {
            gameOver = false;
            break;
         }
      }

      if (gameOver)
      {
         // Enemies dead, player wins
         return true;
      }

      return false;
   }
}
