using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour {
    public int handIndex;
    public bool ocupado;

    private void OnCollisionEnter2D(Collision2D collision) {
        Card card = collision.gameObject.GetComponent<Card>();
        if(!ocupado) {
            //card.gm.FreeSlot(card.handIndex);
            card.handIndex = handIndex;
            //Debug.Log("mudou o slot");
            ocupado = true;
        }
    }
}
