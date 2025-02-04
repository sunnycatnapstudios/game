using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember
{
    public string Name { get; private set; }
    public int Health { get; set; }
    public int Damage { get; set; }

    public PartyMember(string name, int health, int damage)
    {
        Name = name;
        Health = health;
        Damage = damage;
    }
}

public class PartyManager : MonoBehaviour
{
    public int partyCount;
    public GameObject partyMemberTemplate;
    public List<GameObject> partyMembers = new List<GameObject>();
    GameObject partySpawn;

    public Dictionary<string, PartyMember> allPartyMembers = new Dictionary<string, PartyMember>();
    public List<PartyMember> currentPartyMembers = new List<PartyMember>();
    public  List<GameObject> spawnedPartyMembers = new List<GameObject>();

    void UpdatePartyCount()
    {
        // Clear existing spawned objects
        foreach (var obj in spawnedPartyMembers)
        {
            Destroy(obj);
        }
        spawnedPartyMembers.Clear();

        // Spawn objects for each current party member
        for (int i = 0; i < currentPartyMembers.Count; i++)
        {
            GameObject newPartyObject = Instantiate(partyMemberTemplate);

            newPartyObject.name = currentPartyMembers[i].Name;
            newPartyObject.GetComponent<Follower>().order = i+1;
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
        allPartyMembers.Add("MemberA", new PartyMember("MemberA", 100, 20));
        allPartyMembers.Add("MemberB", new PartyMember("MemberB", 120, 15));
        allPartyMembers.Add("MemberC", new PartyMember("MemberC", 150, 17));
        allPartyMembers.Add("MemberD", new PartyMember("MemberD", 80, 25));
        allPartyMembers.Add("MemberE", new PartyMember("MemberE", 200, 42));

        // AddToParty("Alice");
        // AddToParty("Bob");
        
        // Party Members Spawning
        // for (int x = 0; x<partyCount; x++) {
        //     partySpawn = GameObject.Instantiate(partyMemberTemplate);
        //     partySpawn.name = $"Follower {x+1}";
        //     partySpawn.GetComponent<Follower>().order = x+1;
        //     partyMembers.Add(partySpawn);
        // }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            // Debug.Log("Added PartyMember");
            AddToParty("MemberD");
            AddToParty("MemberA");

        }
        if (Input.GetKeyDown(KeyCode.O)) {
            // Debug.Log("Removed PartyMember");
            RemoveFromParty("MemberD");
        }
    }
}
