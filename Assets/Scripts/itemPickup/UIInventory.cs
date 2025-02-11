using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private RectTransform partyPanel;


    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private UIDescription descriptionUI;

    List<UIItem> listOfItems= new List<UIItem>();
    List<UIPartyMember> listOfMembers = new List<UIPartyMember>();

    [SerializeField]
    private Button useButton;

    private UIPartyMember selectedMember;
    private UIItem selectedItem;
    private bool itemInUse;
    private UIItem usingItem;
    

    public void InitializeInventory(int inventorySlotsAmount) {
        for (int i = 0; i < inventorySlotsAmount; i++)
        {
            UIItem item = Instantiate(itemPrefab,Vector3.zero, Quaternion.identity);
            item.transform.SetParent(contentPanel);
            listOfItems.Add(item);
            item.OnItemClick += HandleItemSelection;
        
            useButton.onClick.AddListener(onUseCLick);
        }
    
    }
    private void HideButton()
    {
        useButton.gameObject.SetActive(false);


    }

    private void DisplayButton()
    {
        useButton.gameObject.SetActive(true);
    }
    private void Awake()
    {
        Hide();
        descriptionUI.ResetDescription();
        HideButton();

    }


    void onUseCLick()
    {
        if (itemInUse) {

            if (selectedMember != null)
            {

                selectedMember.GetMember().SetFed();
                selectedMember.SetdisplayItem(selectedMember.GetMember());

                //redraw panel
                itemInUse = false;
                usingItem = null;
                selectedMember = null;
            }

        }
        else
        {
            itemInUse = true;
            usingItem = selectedItem;

        }
    }
    
    public void InitializeParty(Dictionary<string, Survivor> survivors)
    {
        foreach (Survivor member in survivors.Values)
        {
            UIPartyMember person = Instantiate(memberPrefab, Vector3.zero, Quaternion.identity);
            person.transform.SetParent(partyPanel);
            listOfMembers.Add(person);
            person.OnItemClicked += HandlePartySelection;

        }
    }

    public void AddPartyMember()
    {
        UIPartyMember person = Instantiate(memberPrefab, Vector3.zero, Quaternion.identity);
        person.transform.SetParent(partyPanel);
        listOfMembers.Add(person);
        person.OnItemClicked += HandlePartySelection;


    }



    private void HandlePartySelection(UIPartyMember member)
    {
        Debug.Log("here????");
        
           
            Survivor held = member.GetMember();
            if (held != null)
            {
                descriptionUI.SetDescription(held.GetName(), held.GetHealth().ToString());
                ClearSelected();
                member.Selected();
                DisplayButton();
            }
        
    }

    private void HandleItemSelection(UIItem item)
    {
       
           
            Item held = item.getItem();
        if (held != null)
        {
            descriptionUI.SetDescription(held.GetName(), held.GetDesc());
            ClearSelected();
            item.Selected();
            itemInUse = false;
            usingItem = null;
            if (item.getItem().getUsable() == true)
            {
                DisplayButton();
                selectedItem = item.getItem();
            }
        }
        
       }

    public void DisplayItem() { }

    public void Show(Dictionary<string, Slot> inventory, Dictionary<string, Survivor> survivors)
    {
        gameObject.SetActive(true);
        descriptionUI.ResetDescription();
        int counter = 0;
        foreach(Slot Slot in inventory.Values) {
            listOfItems[counter].SetdisplayItem(Slot.GetItem(),Slot.getCount());
            counter++;
            
        }
        counter = 0;
        foreach (Survivor member in survivors.Values)
        {
            Debug.Log(counter);
            listOfMembers[counter].SetdisplayItem(member);
            counter++;
       

        }

    }
    public void ClearSelected()
    {
        foreach(UIItem item in listOfItems)
        {
            item.Deselect();
        }
        foreach(UIPartyMember member in listOfMembers)
        {
            member.Deselect();
        }
        HideButton();
        //itemInUse = false;
        //usingItem = null;
        selectedMember = null;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
