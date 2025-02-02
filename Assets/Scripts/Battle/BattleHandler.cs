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
   [SerializeField] private float XSpawnOffset;
   [SerializeField] private float XSpaceBetween;
   [SerializeField] private float YSpawnOffset;
   
   // To offset spawning of entities
   private float playerOffset = 0;
   private float enemyOffset = 0;
   
   // TODO compress lists into just one
   private List<CharacterBattle> playerCharacterBattles = new List<CharacterBattle>();    // List of all player characters (scripts)
   private List<CharacterBattle> enemiesCharacterBattles = new List<CharacterBattle>();   // List of all enemy characters scripts
   private List<CharacterBattle> characterBattlesTurnOrder = new List<CharacterBattle>(); // Turn order of enemy and characters (sorted by speed)
   
   private GameObject ResultScreen;    // The GameObject ResultScreen with text and button;
   private GameObject TurnOrderView;    // The GameObject TurnOrderView with 4 (+ 1 hidden) Sprite renderer;
   private List<SpriteRenderer> TurnOrderSprites = new List<SpriteRenderer>();     // References to the sprite renderers in TurnOrderView
   
   private CharacterBattle activeCharacterBattle;     // The active characterBattle object in battle
   private int activeCharacterIndex = 0;              // Active index in characterBattleTurnOrder
   private int totalCharacterCount;                   // Total number of entities in combat
   
   private State state;                      // Current player state in combat
   private int selectedEnemyIndex = 0;       // Index of which enemy the player is targeting
   
   private enum State      // Block input when Busy
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
      
      // Fetch the turn order view and set the sprites
      TurnOrderView = GameObject.Find("TurnOrderView");
      for (int i = 1; i <= 5; i++)
      {
         TurnOrderSprites.Add(TurnOrderView.transform.Find("Portraits").Find("Sprite" + i).gameObject.GetComponent<SpriteRenderer>());
         TurnOrderSprites[i - 1].sprite = characterBattlesTurnOrder[i%totalCharacterCount].GetPortraitSprite();
      }
   }

   // Check for user input
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
   
   // Set the character location. Fetch, setup, return
   private CharacterBattle SpawnCharacter(Transform pfCharacterBattle, bool isPlayerTeam)
   {
      Vector3 position;
      
      // Spawn on either left or right hand side
      if (isPlayerTeam)
      {
         position = new Vector3(playerOffset-XSpawnOffset, YSpawnOffset);
         playerOffset -= XSpaceBetween;
      }
      else
      {
         position = new Vector3(enemyOffset+ XSpawnOffset, YSpawnOffset);
         enemyOffset += XSpaceBetween;
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
      
      // Update the turn list
      RenderNewTurnListView();
      
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

   // TODO Called on ChooseNextActiveCharacter. Update the sprites to match new turn
   private void RenderNewTurnListView()
   {
      for (int i = 1; i <= 4; i++)
      {
         TurnOrderSprites[i-1].sprite = TurnOrderSprites[i].sprite;

      }
      // Update the hidden 5th sprite with the 5th character
      TurnOrderSprites[4].sprite = characterBattlesTurnOrder[(activeCharacterIndex+4)%totalCharacterCount].GetPortraitSprite();
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
