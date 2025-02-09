using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharStats
{
    public string Name;
    public int Attack;
    public int Health;
    public bool IsEnemy;

    public CharStats(string name, int attack, int health, bool type)
    {
        Name = name;
        Attack = attack;
        Health = health;
        IsEnemy = type;
    }
}

public class BattleUiHandler : MonoBehaviour
{
    public Animator partyUIAnimator;
    public Animator enemyUIAnimator;
    public bool attackOption = false, itemOption = false;
    public GameObject overworldUI, combatUI;

    public PartyManager partyManager;

    List<CharStats> battleOrder = new List<CharStats>();
    private int currentTurnIndex = 0;
    private bool battleInProgress = false;

    public Dictionary<string, CharStats> L1Enemies = new Dictionary<string, CharStats>
    {
        { "Handy", new CharStats("Handy", 20, 100, true)},
        { "Gregor", new CharStats("Gregor", 15, 120, true)},
        { "Turf", new CharStats("Turf", 17, 150, true)}
    };
    public Dictionary<string, CharStats> L2Enemies = new Dictionary<string, CharStats>();
    public List<CharStats> currentEnemies = new List<CharStats>();

    public GameObject floatingTextPrefab;
    
    void OnEnable()
    {
        partyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        enemyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        currentEnemies = new List<CharStats>(L1Enemies.Values);

        StartBattle();
    }
    public void StartBattle()
    {
        battleOrder.Clear();

        foreach (var member in partyManager.currentPartyMembers)
        {
            if (member.isCombatant)
            {   
                battleOrder.Add(new CharStats(member.Name, member.Damage, member.Health, false));
            }
        }
        battleOrder.Add(currentEnemies[Random.Range(0, currentEnemies.Count)]);
        battleOrder = ShuffleList(battleOrder);

        foreach (var Char in battleOrder)
        {
            Debug.Log($"Combatant: {Char.Name}, Attack: {Char.Attack}, Health: {Char.Health}");
        }
        battleInProgress = true;
        StartCoroutine(TurnLoop());
    }
    public List<CharStats> ShuffleList(List<CharStats> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }

    private IEnumerator TurnLoop()
    {
        while (battleInProgress)
        {
            if (currentTurnIndex >= battleOrder.Count)
            {
                currentTurnIndex = 0; // Loop back to the first combatant
            }

            CharStats currentCombatant = battleOrder[currentTurnIndex];

            Debug.Log($"It is {currentCombatant.Name}'s turn!");

            if (currentCombatant.IsEnemy) // Enemy turn
            {
                yield return EnemyTurn(currentCombatant);
            }
            else if (!currentCombatant.IsEnemy)// Player turn
            {
                yield return PlayerTurn(currentCombatant);
            }

            // Check if the battle is over
            if (CheckForBattleEnd())
            {
                Debug.Log("Battle has ended!");
                battleInProgress = false;
                yield break;
            }

            currentTurnIndex++;
            yield return new WaitForSecondsRealtime(1.0f); // Add a delay for better readability
        }
    }
    private IEnumerator PlayerTurn(CharStats player)
    {
        Debug.Log($"{player.Name}'s turn. Choose an action!");
        yield return new WaitForSecondsRealtime(2.0f); // Simulating time for player input
    }

    private IEnumerator EnemyTurn(CharStats enemy)
    {
        // Select a random target from the player's party
        List<CharStats> playerParty = battleOrder.FindAll(c => !c.IsEnemy); // Exclude Will from selection
        if (playerParty.Count > 0)
        {
            CharStats target = playerParty[Random.Range(0, playerParty.Count)];

            // Simulate attack
            Debug.Log($"{enemy.Name} attacks {target.Name} for {enemy.Attack} damage!");
            partyManager.TakeDamage(target.Name, enemy.Attack);
            target.Health -= enemy.Attack;
            ShowFloatingText("egg", enemy.Attack, Color.red);

            // Check if target is defeated
            if (target.Health <= 0)
            {
                Debug.Log($"{target.Name} has been defeated!");
                battleOrder.Remove(target);
            }
        }

        yield return new WaitForSecondsRealtime(1.0f);
    }

    void ShowFloatingText(string memberName, int damage, Color color)
    {
        // GameObject memberObject = GameObject.Find(memberName); // Find the GameObject representing the character
        // if (memberObject != null && floatingTextPrefab != null)
        {
            // Vector3 spawnPosition = memberObject.transform.position + new Vector3(0, 1.5f, 0);
            Vector3 spawnPosition = floatingTextPrefab.transform.position + new Vector3(0, 1.5f, 0);
            // GameObject floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            GameObject floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            floatingText.GetComponent<DamageIndicator>().SetText(damage.ToString(), color);
        }
    }


    private bool CheckForBattleEnd()
    {
        bool playersAlive = battleOrder.Exists(c => !c.IsEnemy);
        bool enemiesAlive = battleOrder.Exists(c => c.IsEnemy);

        return !playersAlive || !enemiesAlive;
        // return false;
    }

    public void Attack()
    {
        Debug.Log("Attack Button Pressed");
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (attackOption) {
                partyUIAnimator.SetTrigger("Close");
                attackOption = false;
            } else if (itemOption) {
                partyUIAnimator.SetTrigger("Reset");
                itemOption = false;
                attackOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                attackOption = true;
            }
        }
    }
    public void Item()
    {
        Debug.Log("Item Button Pressed");
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption) {
                partyUIAnimator.SetTrigger("Close");
                itemOption = false;
            } else if (attackOption) {
                partyUIAnimator.SetTrigger("Reset");
                attackOption = false;
                itemOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                itemOption = true;
            }
        }
    }
    public void Escape()
    {
        Debug.Log("Escape Button Pressed");
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption||attackOption) {
                partyUIAnimator.SetTrigger("Close");
                attackOption = itemOption = false;
            }
        }
        overworldUI.SetActive(true);
        combatUI.SetActive(false);
        Time.timeScale = 1;
    }
    void Start()
    {
        // partyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        // enemyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        // currentEnemies = new List<CharStats>(L1Enemies.Values);
    }
    void Update()
    {

    }
}
