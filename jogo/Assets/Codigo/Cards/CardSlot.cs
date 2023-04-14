using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler {
    public bool desativado;
    public bool equipSlot;

    public void OnDrop(PointerEventData eventData) {
        if(!desativado) {
            if(transform.childCount == 0) {
                Card card = eventData.pointerDrag.GetComponent<Card>();
                if(equipSlot) {
                    if(card.Type == "Equip" || card.equipado) {
                        card.parentAfterDrag = transform;
                    }
                }else {
                    if(!card.equipado) {
                        card.parentAfterDrag = transform;
                    }
                }
            }
        }
    }
}
