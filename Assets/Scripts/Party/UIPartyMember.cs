using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPartyMember : MonoBehaviour
{
    
    public event System.Action<UIPartyMember> OnItemClick;

    private PartyMember member;

    [SerializeField]
    private TMP_Text quantity;
    [SerializeField]
    private Image image;
    private bool empty = true;
    [SerializeField]
    private Image SelectedImage;

    public void OnPointerClick(BaseEventData data)
    {

        PointerEventData pointerData = (PointerEventData)data;
        if (pointerData.button == PointerEventData.InputButton.Left)
        {

            Debug.Log("here");
            OnItemClick?.Invoke(this);


        }
    }


        public void Deselect()
    {
        SelectedImage.enabled = false;

    }
    public void ResetData()
    {
        this.image.gameObject.SetActive(false);
        empty = true;
    }
    public void Selected()
    {
        SelectedImage.enabled = true;

    }

    public PartyMember GetMember()
    {
        return member;
    }


    public void SetdisplayItem(PartyMember member, int quantity)
    {
        this.image.gameObject.SetActive(true);
        this.image.sprite = member.GetSprite();
        this.quantity.text = quantity.ToString();
        this.member = member;

        empty = false;

    }
}
}
