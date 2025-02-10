using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public bool actOption = false, itemOption = false;
    public GameObject overworldUI, combatUI, actOptionBList, itemOptionBList, enemySlot;

    // public List<PartySlot> partySlots;
    public List<GameObject> partySlots = new List<GameObject>();
    public PartyManager partyManager;

    public List<CharStats> battleOrder = new List<CharStats>();
    public int currentTurnIndex = 0;
    private bool battleInProgress = false;

    public Dictionary<string, CharStats> L1Enemies = new Dictionary<string, CharStats>
    {
        { "Handy", new CharStats("Handy", 38, 100, true)},
        { "Gregor", new CharStats("Gregor", 21, 120, true)},
        { "Turf", new CharStats("Turf", 31, 150, true)}
    };
    public Dictionary<string, CharStats> L2Enemies = new Dictionary<string, CharStats>();
    public List<CharStats> currentEnemies = new List<CharStats>();
    public List<Sprite> profileImages;
    public TurnIndicator turnIndicator;
    public GameObject floatingTextPrefab;
    public TextMeshProUGUI enemyName;
    private string selectedAction = "";
    CharStats enemyStats;
    
    void OnEnable()
    {
        partyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        enemyUIAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        currentEnemies = new List<CharStats>(L1Enemies.Values);

        StartCoroutine(StartBattle());
    }
    public IEnumerator StartBattle()
    {
        battleOrder.Clear();

        foreach (var member in partyManager.currentPlayer)
        {
            CharStats playerStats = new CharStats(member.Name, member.Damage, member.Health, false);
            battleOrder.Add(playerStats);
            partySlots[0].GetComponent<PartySlot>().Name = member.Name;
            partySlots[0].GetComponent<PartySlot>().SetHealth(member.Health);
        }

        int slotIndex = 1;
        foreach (var member in partyManager.currentPartyMembers)
        {
            if (member.isCombatant)
            {
                CharStats newChar = new CharStats(member.Name, member.Damage, member.Health, false);
                battleOrder.Add(newChar);

                if (slotIndex < partySlots.Count)
                {
                    // Assign correct profile image
                    // partySlots[slotIndex].profile.sprite = member.profileSprite; // Ensure PartyMember has profileSprite
                    partySlots[slotIndex].GetComponent<PartySlot>().Name = member.Name;
                    partySlots[slotIndex].GetComponent<PartySlot>().SetHealth(member.Health);
                }
                slotIndex++;
            }
        }

        enemyStats = currentEnemies[Random.Range(0, currentEnemies.Count)];
        enemyName.text = ":"+enemyStats.Name;
        enemySlot.GetComponent<EnemyHealthbar>().SetHealth(enemyStats.Health);
        battleOrder.Add(enemyStats);

        battleOrder = ShuffleList(battleOrder);
        
        foreach (var profilePic in profileImages)
        {
            foreach (var slot in partySlots)
            {
                if (profilePic.name == slot.GetComponent<PartySlot>().Name) {
                    slot.GetComponent<PartySlot>().profile.sprite = profilePic;
                }
            }
        }

        foreach (var Char in battleOrder)
        {
            Debug.Log($"{Char.Name}: {Char.Attack} Damage, {Char.Health} HP");
        }
        battleInProgress = true;
        turnIndicator.SetupTurnIndicator(battleOrder.Count);

        // Switch to new battle music
        AudioManager.Instance.PlayUISound("Sfx_BattleBell_Short");
        AudioManager.Instance.CrossFadeAmbienceToZero(1f);
        AudioManager.Instance.CrossFadeMusicSound("Music_JustSynth", 2f, 1f, 1f);
            
        yield return new WaitForSecondsRealtime(2.0f);
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
                EndEncounter();
                yield break;
            }

            currentTurnIndex++;
            
        }
    }
    private IEnumerator PlayerTurn(CharStats player)
    {
        Debug.Log($"{player.Name}'s turn. Choose an action!");

        selectedAction = null;
        while (selectedAction == null)
        {
            yield return null;
        }

        if (selectedAction == "Attack")
        {
            enemyStats.Health -= player.Attack;
            Debug.Log($"{player.Name} attacks {enemyStats.Name} for {player.Attack} damage!");

            ShowFloatingText(player.Attack, Color.red);

            if (enemyStats.Health <= 0)
            {
                Debug.Log($"{enemyStats.Name} has been defeated!");
                battleOrder.Remove(enemyStats);
            }
        }
        if (selectedAction == "Defend")
        {
            
        }
        if (selectedAction == "Heal")
        {
            
        }

        enemySlot.GetComponent<EnemyHealthbar>().UpdateHealthBar(enemyStats.Health);
        foreach (GameObject mem in partySlots) {
            if (mem.GetComponent<PartySlot>().Name == player.Name) {mem.GetComponent<PartySlot>().UpdateHealthBar(player.Health);}
        }

        Debug.Log($"Player chose {selectedAction}");
        yield return new WaitForSecondsRealtime(2.0f);
    }

    private IEnumerator EnemyTurn(CharStats enemy)
    {
        yield return new WaitForSecondsRealtime(1.0f);
        // Select a random target from the player's party
        List<CharStats> playerParty = battleOrder.FindAll(c => !c.IsEnemy); // Exclude Will from selection
        if (playerParty.Count > 0)
        {
            CharStats target = playerParty[Random.Range(0, playerParty.Count)];

            // Simulate attack
            Debug.Log($"{enemy.Name} attacks {target.Name} for {enemy.Attack} damage!");
            partyManager.TakeDamage(target.Name, enemy.Attack);
            // partySlots.UpdateHealthBar(target.Health-enemy.Attack);
            target.Health -= enemy.Attack;
            foreach (GameObject mem in partySlots) {
                if (mem.GetComponent<PartySlot>().Name == target.Name) {mem.GetComponent<PartySlot>().UpdateHealthBar(target.Health);}
            }

            ShowFloatingText(enemy.Attack, Color.red);

            // Check if target is defeated
            if (target.Health <= 0)
            {
                Debug.Log($"{target.Name} has been defeated!");
                battleOrder.Remove(target);
            }
        }

        yield return new WaitForSecondsRealtime(1.0f);
    }

    void ShowFloatingText(int damage, Color color)
    {
        {
            Vector3 spawnPosition = floatingTextPrefab.transform.position + new Vector3(0, 1.5f, 0);
            GameObject floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Combat UI").transform);
            floatingText.SetActive(true);
            floatingText.GetComponent<DamageIndicator>().SetText(damage.ToString(), color);
        }
    }

    // Used to check whether all players or all enemies are dead
    private bool CheckForBattleEnd()
    {
        bool playersAlive = battleOrder.Exists(c => !c.IsEnemy);
        bool enemiesAlive = battleOrder.Exists(c => c.IsEnemy);

        return !playersAlive || !enemiesAlive;
    }

    // Called when the battle should end. Use to transition back to overworld 
    private void EndEncounter()
    {
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption||actOption) {
                partyUIAnimator.SetTrigger("Close");
                actOption = itemOption = false;
            }
        }
        actOptionBList.SetActive(actOption);
        itemOptionBList.SetActive(itemOption);
        overworldUI.SetActive(true);
        combatUI.SetActive(false);
        
        // Switch back to environmental sounds
        AudioManager.Instance.CrossFadeMusicToZero(1f);
        AudioManager.Instance.CrossFadeAmbienceSound("Ambient_Forest", 1f, 1f, 1f);
        
        Time.timeScale = 1;
    }
    
    public void OnActionButtonPressed(string action)
    {
        selectedAction = action;
    }

    public void Act()
    {
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (actOption) {
                partyUIAnimator.SetTrigger("Close");
                actOption = false;
            } else if (itemOption) {
                partyUIAnimator.SetTrigger("Reset");
                itemOption = false;
                actOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                actOption = true;
            }
            actOptionBList.SetActive(actOption);
            itemOptionBList.SetActive(itemOption);
        }
    }
    public void Item()
    {
        if (partyUIAnimator != null)
        {
            partyUIAnimator.ResetTrigger("Open");
            partyUIAnimator.ResetTrigger("Close");
            partyUIAnimator.ResetTrigger("Reset");

            if (itemOption) {
                partyUIAnimator.SetTrigger("Close");
                itemOption = false;
            } else if (actOption) {
                partyUIAnimator.SetTrigger("Reset");
                actOption = false;
                itemOption = true;
            }
            else {
                partyUIAnimator.SetTrigger("Open");
                itemOption = true;
            }
            actOptionBList.SetActive(actOption);
            itemOptionBList.SetActive(itemOption);
        }
    }
    public void Escape()
    {
        EndEncounter();
    }
}
