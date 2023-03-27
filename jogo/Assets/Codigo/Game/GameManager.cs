using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Server Identity")]
    [SerializeField]
    public ServerIdentity serverIdentity;

    [Header("Cards")]
    public List<Card> deck = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardsSlots;

    [Header("Prefab")]
    public GameObject explosionPrefab;

    public void DrawCard() {
        if(deck.Count >= 1) {
            Card randCard = deck[Random.Range(0, deck.Count)];
            for(int i = 0; i < availableCardsSlots.Length; i++) {
                if(availableCardsSlots[i] == true) {
                    randCard.gameObject.SetActive(true);
                    randCard.handIndex = i;
                    randCard.transform.position = cardSlots[i].position;
                    availableCardsSlots[i] = false;
                    deck.Remove(randCard);
                    return;
                }
            }
        }
    }

    public void GameExplosion(Vector3 objectPosition) {
        Quaternion rotation = Quaternion.identity;

        GameObject explosao = Instantiate(explosionPrefab, objectPosition, rotation);
    }

    public void EmitExplosion(Vector3 objectPosition){
        Position pos = new Position();
        pos.x = objectPosition.x;
        pos.y = objectPosition.y;

        serverIdentity.GetSocket().Emit("Explosion", JsonSerializer.Serialize(pos));
    }

    private void Update() {

    }
}
