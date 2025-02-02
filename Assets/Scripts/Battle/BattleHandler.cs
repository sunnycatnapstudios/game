using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

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
   
   private GameObject ResultScreen;    // The GameObject ResultScreen with text and button;

   //private int currentTurn;
   private int playerOffset = 0;
   private int enemyOffset = 0;
   
   private CharacterBattle activeCharacterBattle;     // The active character in battle
   private int activeCharacterIndex = 0;
   private int totalCharacterCount;
   
   private State state;    // Current state of combat
   private int selectedEnemyIndex = 0;    // Index of which enemy the player is targeting
   
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
      // Spawn the entities into the scene
      foreach (Transform child in playerParty)
      {
         playerCharacterBattles.Add(SpawnCharacter(child, true));
      }
      foreach (Transform child in enemyEncounter)
      {
         enemiesCharacterBattles.Add(SpawnCharacter(child, false));
      }
      
      // Default target first enemy
      enemiesCharacterBattles[0].ShowSelectionCircle();

      // Determine the turn order
      CreateTurnOrder();
      SetActiveCharacterBattle(characterBattlesTurnOrder[activeCharacterIndex++]);  // Fastest Character Goes first
      state = State.WaitingForPlayer;
      
      // Fetch and hide the result screen
      ResultScreen = GameObject.Find("ResultScreen");
      ResultScreen.SetActive(false);
   }

   private void Update()
   {
      if (state == State.WaitingForPlayer)
      {
         // Left/Right Arrow keys switches enemy target to next alive enemy. Also hides and show circle
         if (Input.GetKeyDown(KeyCode.LeftArrow))
         {
            do
            {
               enemiesCharacterBattles[selectedEnemyIndex].HideSelectionCircle();
               if (selectedEnemyIndex == 0)     // C# doesnt handle negative module
               {
                  selectedEnemyIndex = enemiesCharacterBattles.Count - 1;
               }
               else
               {
                  selectedEnemyIndex = (selectedEnemyIndex - 1) % enemiesCharacterBattles.Count;
               }
            } while (enemiesCharacterBattles[selectedEnemyIndex].IsDead());
            enemiesCharacterBattles[selectedEnemyIndex].ShowSelectionCircle();
         }
         if (Input.GetKeyDown(KeyCode.RightArrow))
         {
            do
            {
               enemiesCharacterBattles[selectedEnemyIndex].HideSelectionCircle();
               selectedEnemyIndex = (selectedEnemyIndex + 1) % enemiesCharacterBattles.Count;
            } while (enemiesCharacterBattles[selectedEnemyIndex].IsDead());
            enemiesCharacterBattles[selectedEnemyIndex].ShowSelectionCircle();
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
         // TODO End the battle. Show splash screen. Nav back to scene
         Debug.Log("Battle Over");
         return;  
      }

      bool enemyToPlayer = !activeCharacterBattle.IsPlayerTeam;
      
      // Get the next character
      SetActiveCharacterBattle(GetNextCharacterBattler());
      
      // If it's an enemy they auto attack
      if (enemiesCharacterBattles.Contains(activeCharacterBattle))
      {
         if (!enemyToPlayer)
         {
            // We are going from player to enemy. Clear selection circle
            enemiesCharacterBattles[selectedEnemyIndex].HideSelectionCircle();
         }
         // TODO Enemy Auto attack (random, for now just first) player
         activeCharacterBattle.Attack(playerCharacterBattles[0], onAttackComplete: () =>
         {
            ChooseNextActiveCharacter();     // Next character gets turn
         });
      }
      else
      {
         if (enemyToPlayer)
         {
            // We are going from enemy to player. Reset selection
            enemiesCharacterBattles[selectedEnemyIndex].ShowSelectionCircle();
         }
         // Back to waiting on player input
         state = State.WaitingForPlayer;
      }
   }

   // Decide the turn order based on speed
   // TODO alternates between player and opponent. For now all players move first
   private void CreateTurnOrder()
   {

      foreach (CharacterBattle playercb in playerCharacterBattles)
      {
         characterBattlesTurnOrder.Add(playercb);
      }

      foreach (CharacterBattle enemycb in enemiesCharacterBattles)
      {
         characterBattlesTurnOrder.Add(enemycb);
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
         // Set winners text
         ResultScreen.transform.Find("WinnerText").GetComponent<Text>().text = "You've Won!";
         ResultScreen.SetActive(true);
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
         ResultScreen.transform.Find("WinnerText").GetComponent<Text>().text = "You've Lost!";
         ResultScreen.SetActive(true);
         return true;
      }

      return false;
   }
   
   // Public scripts called by player UI
   // Used in attack button to attack currently targeted enemy
   public void AttackOpponent()
   {
      if (state != State.Busy)
      {
         state = State.Busy;
         Debug.Log("Attacking enemy" + selectedEnemyIndex);
         activeCharacterBattle.Attack(enemiesCharacterBattles[selectedEnemyIndex], onAttackComplete: () =>
         {
            ChooseNextActiveCharacter();     // Next character gets turn
         });
      }
   }
   
   // Used in inventory button to open inventory. Blocks all other actions until success
   public void OpenInventory()
   {
      state = State.Busy;
      // TODO implement
      state = State.WaitingForPlayer;
   }

   // Used in escape button. Attempts an escape
   public void EscapeBattle()
   {
      state = State.Busy;
      // TODO implement
      state = State.WaitingForPlayer;
   }

   public void ReturnToSceneTransition()
   {
      // TODO return to original scene
      SceneManager.LoadScene("Testing Scene");
   }
}
