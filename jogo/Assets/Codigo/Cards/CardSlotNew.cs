using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlotNew : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData) {
        if(transform.childCount == 0) {
            DragAndDrop dd = eventData.pointerDrag.GetComponent<DragAndDrop>();
            dd.parentAfterDrag = transform;
        }
    }
}
