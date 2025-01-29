using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    Player player;
    CircleCollider2D circleCollider;
    Dictionary<string, Slot> inventory;
    List<GameObject> itemsInRange;



    void Start()
    {
        inventory = new Dictionary<string, Slot>();
        itemsInRange = new List<GameObject>();


    }
    private void addItem(Item item)

    {
        if (!inventory.ContainsKey(item.GetName()))
        {
            Slot slot = new Slot(item);
            inventory.Add(item.GetName(), slot);
            slot.incCount();
        }
        else
        {
            Slot itemSlot = inventory[item.GetName()];
            itemSlot.incCount();
        }

    }


    void OnTriggerEnter2D(Collider2D col)
    {

        // Add the GameObject collided with to the list.
        if (col.gameObject.GetComponent<Pickupable>() != null)
        {
            itemsInRange.Add(col.gameObject);

           
            
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
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
    }

    void OnTriggerExit2D(Collider2D col)
    {

        // Remove the GameObject collided with from the list.
        if (col.gameObject.GetComponent<Pickupable>() != null)
        {
            itemsInRange.Remove(col.gameObject);

            // Print the entire list to the console.
           

        }
    }

    
}


