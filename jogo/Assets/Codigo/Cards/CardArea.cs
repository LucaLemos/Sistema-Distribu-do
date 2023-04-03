using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardArea : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        Card card = collision.gameObject.GetComponent<Card>();
        //Debug.Log("desativado");
        card.isActive = false;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        Card card = collision.gameObject.GetComponent<Card>();
        //Debug.Log("ativado");
        card.isActive = true;
    }
}
