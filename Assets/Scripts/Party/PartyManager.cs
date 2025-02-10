using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember
{
    public string Name { get; private set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    public bool Combatant { get; set; }

    private int curHealth;

    public PartyMember(string name, int health, int damage, bool combatant)
    {
        Name = name;
        Health = health;
        Damage = damage;
        Combatant = combatant;
        curHealth = health;
    }
}

public class PartyManager : MonoBehaviour
{
    public int partyCount;
    public GameObject partyMemberTemplate;
    public List<GameObject> partyMembers = new List<GameObject>();
    GameObject partySpawn;

    public Dictionary<string, PartyMember> allPartyMembers = new Dictionary<string, PartyMember>
    {
        { "MemberA", new PartyMember("MemberA", 100, 20, true)},
        { "MemberB", new PartyMember("MemberB", 120, 15, false)},
        { "MemberC", new PartyMember("MemberC", 150, 17, false)},
        { "MemberD", new PartyMember("MemberD", 80, 25, true)},
        { "MemberE", new PartyMember("MemberE", 200, 42, true)}
    };
    // public Dictionary<string, RuntimeAnimatorController> partyAnimControllers = new Dictionary<string, RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> partyAnimControllers = new List<RuntimeAnimatorController>();

    public List<PartyMember> currentPartyMembers = new List<PartyMember>();
    public List<GameObject> spawnedPartyMembers = new List<GameObject>();
    public List<PartyMember> partyMemberList = new List<PartyMember>();

    void AssignAnimator(GameObject memberObject, string memberName)
    {
        Animator anim = memberObject.GetComponent<Animator>();

        for (int i = 0; i < partyMemberList.Count; i++)
        {
            if (partyMemberList[i].Name == memberName && i < partyAnimControllers.Count)
            {
                anim.runtimeAnimatorController = partyAnimControllers[i];
                break;
            }
        }
    }

    void UpdatePartyCount()
    {
        //Refresh Onscreen Party when there is a change
        foreach (var obj in spawnedPartyMembers) {Destroy(obj);}
        spawnedPartyMembers.Clear();

        // Spawn GameObjects for each current party member
        for (int i = 0; i < currentPartyMembers.Count; i++)
        {
            GameObject newPartyObject = Instantiate(partyMemberTemplate);

            newPartyObject.name = currentPartyMembers[i].Name;
            newPartyObject.GetComponent<Follower>().order = i+1;
            AssignAnimator(newPartyObject, currentPartyMembers[i].Name);

            spawnedPartyMembers.Add(newPartyObject);

            partyCount = currentPartyMembers.Count;
        }

        Debug.Log($"Current Party Count: {currentPartyMembers.Count}");
    }

    public void AddToParty(string memberName)
    {
        if (allPartyMembers.ContainsKey(memberName) && !currentPartyMembers.Contains(allPartyMembers[memberName]))
        {
            currentPartyMembers.Add(allPartyMembers[memberName]);
            Debug.Log($"{memberName} has joined the party!");
            UpdatePartyCount();
        }
        else
        {
            Debug.Log($"{memberName} is already in the party or doesn't exist.");
        }
    }

    public void RemoveFromParty(string memberName)
    {
        var memberToRemove = currentPartyMembers.Find(member => member.Name == memberName);
        if (memberToRemove != null)
        {
            currentPartyMembers.Remove(memberToRemove);
            Debug.Log($"{memberName} has been removed from the party.");
            UpdatePartyCount();
        }
        else
        {
            Debug.Log($"{memberName} is not in the party.");
        }
    }

    void Start()
    {
        partyMemberList = new List<PartyMember>(allPartyMembers.Values);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (currentPartyMembers.Count<allPartyMembers.Count)
            {
                // Debug.Log("Added PartyMember");
                // AddToParty("MemberD");
                // AddToParty("MemberA");

                // AddToParty(partyMemberList[currentPartyMembers.Count].Name);

                foreach (PartyMember member in partyMemberList){
                    if (!currentPartyMembers.Contains(member))
                    {
                        AddToParty(member.Name);
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("Spawned All Party Members");
            }
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            if (currentPartyMembers.Count>0)
            {
                // Debug.Log("Removed PartyMember");
                
                RemoveFromParty(currentPartyMembers[Random.Range(0, currentPartyMembers.Count-1)].Name);
            }
            else
            {
                Debug.Log("Culled All Party Members");
            }
        }
    }
}