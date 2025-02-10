using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private UIItem itemPrefab;

    [SerializeField]
    private UIPartyMember memberPrefab;

    public Item test;
    //private Sprite image;
    public int quantity;
    public string title, description;




    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private UIDescription descriptionUI;

    List<UIItem> listOfItems= new List<UIItem>();
    List<UIPartyMember> listOfMembers = new List<UIPartyMember>();


    public void InitializeInventory(int inventorySlotsAmount) {
        for (int i = 0; i < inventorySlotsAmount; i++)
        {
            UIItem item = Instantiate(itemPrefab,Vector3.zero, Quaternion.identity);
            item.transform.SetParent(contentPanel);
            listOfItems.Add(item);
            item.OnItemClick += HandleItemSelection;
        }
    
    }
    private void Awake()
    {
        Hide();
        descriptionUI.ResetDescription();

    }
    List<UIPartyMember> members;

    public void InitializeParty(List<PartyMember> members)

    {
        foreach (PartyMember member in members)
        {
            UIPartyMember person =Instantiate(memberPrefab,Vector3.zero, Quaternion.identity);
            person.transform.SetParent(contentPanel);
            listOfMembers.Add(person);
            listOfMembers.OnItemClick += HandleItemSelection;

        }
    }

    private void HandleItemSelection(UIItem item)
    {
        if (item != null)
        {
            Debug.Log(item.getItem().GetName());
            Item held = item.getItem();

            descriptionUI.SetDescription(held.GetName(), held.GetDesc());
            item.Selected();
        }
       }

    public void DisplayItem() { }

    public void Show(Dictionary<string, Slot> inventory)
    {
        gameObject.SetActive(true);
        descriptionUI.ResetDescription();
        int counter = 0;
        foreach(Slot Slot in inventory.Values) {
            listOfItems[counter].SetdisplayItem(Slot.GetItem(),Slot.getCount());
            counter++;
            
        }
        //listOfItems[0].SetdisplayItem(test,quantity);
        

    }
    public void ClearSelected()
    {
        foreach(UIItem item in listOfItems)
        {
            item.Deselect();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
