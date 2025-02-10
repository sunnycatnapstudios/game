using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private UIItem itemPrefab;

    public Item test;
    //private Sprite image;
    public int quantity;
    public string title, description;


    [SerializeField]
    private RectTransform contentPanel;

    [SerializeField]
    private UIDescription descriptionUI;

    List<UIItem> listOfItems= new List<UIItem>();


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

    private void HandleItemSelection(UIItem item)
    {
        Debug.Log(item.name);
        Item held = item.getItem();
        descriptionUI.SetDescription(held.GetName(),held.GetDesc( ));
    }

    public void DisplayItem() { }

    public void Show()
    {
        gameObject.SetActive(true);
        descriptionUI.ResetDescription();
        listOfItems[0].SetdisplayItem(test,quantity);
        

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
