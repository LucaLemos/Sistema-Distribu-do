using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool hasBeenPlayed;
    public int handIndex;
    private GameManager gm;

    private bool isDragging = false;
    private Vector3 startPosition;

    public bool placed { get; private set; }
    public BoundsInt area;

    private void Start() {
        gm = FindObjectOfType<GameManager>(); 
    }
    /*private void OnMouseDown() {
        if(hasBeenPlayed == false) {
            transform.position += Vector3.up * 5;
            hasBeenPlayed = true;
            gm.availableCardsSlots[handIndex] = true;
        }
    }*/

    void OnMouseDown() {
        isDragging = true;
        startPosition = transform.position;
    }

    void OnMouseDrag() {
        if (isDragging)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objectPosition;
        }
    }

    void OnMouseUp() {
        isDragging = false;
        hasBeenPlayed = true;
        gm.availableCardsSlots[handIndex] = true;
        gm.deck.Add(this);
        gameObject.SetActive(false);

        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        gm.EmitExplosion(objectPosition);
        gm.GameExplosion(objectPosition);
    }
}
