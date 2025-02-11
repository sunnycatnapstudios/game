using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    Player player;
    CircleCollider2D circleCollider;
    Dictionary<string, Slot> inventory;
    List<GameObject> itemsInRange;
    public UIInventory inventoryWindow;
    private Item selected;
    private Item inUse;
    private int timer=0;

    

    Dictionary<string, Survivor> survivors = new Dictionary<string, Survivor>();
    public Survivor test1;
    public Survivor test2;
    



    void Start()
    {
        inventory = new Dictionary<string, Slot>();
        itemsInRange = new List<GameObject>();
        inventoryWindow.InitializeInventory(14);
        if(test1 != null)
        {
            survivors.Add(test1.GetName(), test1);
        }
        if (test2 != null) {
            survivors.Add(test2.GetName(), test2);

        }
        inventoryWindow.InitializeParty(survivors);

        

    }
    private void addItem(Item item)

    {
        if (!inventory.ContainsKey(item.GetName()))//adding if no prev item
        {
            Slot slot = new Slot(item);
            inventory.Add(item.GetName(), slot);
            slot.incCount();
        }
        else//if have inc item
        {
            Slot itemSlot = inventory[item.GetName()];
            itemSlot.incCount();
        }
    }
    public void AddMember(Survivor survivor)
    {
        survivors.Add(survivor.GetName(), survivor);
        inventoryWindow.AddPartyMember();
    }

    public bool hasItemByName(string name) {
        return inventory.ContainsKey(name);
    }

    public void removeItemByName(string name) {
        Slot slot = inventory[name];
        slot.decCount();
        if (slot.getCount() == 0) {
            inventory.Remove(name);
        }
    }

    void OnTriggerEnter2D(Collider2D col)//if colliding with an item add to pickupable
    {

        // Add the GameObject collided with to the list.
        if (col.gameObject.GetComponent<Pickupable>() != null)
        {
            itemsInRange.Add(col.gameObject);

           
            
        }
    }

    void Update()
    {
        // pickup item
        if (Input.GetKey(KeyCode.E)) {
            if (itemsInRange.Count > 0)
            {
                GameObject curObj = itemsInRange[0];
                itemsInRange.RemoveAt(0);
                Item item = curObj.GetComponent<Pickupable>().GetItem();
                addItem(item);
                Debug.Log("did");
                foreach (string slot in inventory.Keys) {
                    Debug.Log(slot + inventory[slot].getCount().ToString());
                    
                
                }
                curObj.GetComponent<Pickupable>().DestroyInWorld();



            }
        }
        else if (Input.GetKey(KeyCode.I)&&timer <= 1)
        {
            //Debug.Log(timer.ToString());
            if (inventoryWindow.isActiveAndEnabled == false)
            {
                inventoryWindow.Show(inventory,survivors);
            }
            else
            {
                inventoryWindow.Hide();
            }
            timer = 30;



        }
        if (timer > 0)
        {
            timer -=1;
        }
    }

    void OnTriggerExit2D(Collider2D col)//when leaving item remove from pickupable
    {

        // Remove the GameObject collided with from the list.
        if (col.gameObject.GetComponent<Pickupable>() != null)
        {
            itemsInRange.Remove(col.gameObject);

            // Print the entire list to the console.
           

        }
    }

    
}


