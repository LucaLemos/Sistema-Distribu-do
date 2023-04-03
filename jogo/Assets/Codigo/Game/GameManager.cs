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
    public Transform cards;
    public Transform porta;
    public bool availablePorta;
    public Transform[] cardSlots;
    public bool[] availableCardsSlots;

    [Header("Inventario")]
    [SerializeField]
    public UiInventoryPage inventoryPage;
    
    [Header("Prefab")]
    public GameObject cardPrefab;
    public GameObject explosionPrefab;

    //public void DrawCard() {
    //    if(deck.Count >= 1) {
    //        Card randCard = deck[Random.Range(0, deck.Count)];
    //        for(int i = 0; i < availableCardsSlots.Length; i++) {
    //            if(availableCardsSlots[i] == true) {
    //                randCard.gameObject.SetActive(true);
    //                randCard.handIndex = i;
    //                randCard.transform.position = cardSlots[i].position;
    //                availableCardsSlots[i] = false;
    //                deck.Remove(randCard);
    //                return;
    //            }
    //        }
    //    }
    //}

    public void DrawCard(Carta cartaData) {
        for(int i = 0; i < availableCardsSlots.Length; i++) {
            if(availableCardsSlots[i] == true) {
                Quaternion rotation = Quaternion.identity;
                GameObject prefab = Instantiate(cardPrefab, cards);
                Card card = prefab.GetComponent<Card>();
                card.SpriteName = cartaData.image;
                card.Power = cartaData.power;
                card.Type = cartaData.type;
                card.Effect = cartaData.effect;
                card.porta = false;
                card.atualiza();
                card.handIndex = i;
                card.transform.position = PosiSlot(i);
                availableCardsSlots[i] = false;
                return;
            }
        }
    }

    public void DrawPort(Carta cartaData) {
        if(availablePorta == true) {
            Quaternion rotation = Quaternion.identity;
            GameObject prefab = Instantiate(cardPrefab, porta);
            prefab.name = "CartaPorta";
            ServerIdentity si = prefab.GetComponent<ServerIdentity>();
            si.SetSocketReference(serverIdentity.GetSocket());
            si.SetID(cartaData.id);
            Card card = prefab.GetComponent<Card>();
            card.SpriteName = cartaData.image;
            card.Type = cartaData.type;
            card.Power = cartaData.power;
            card.Effect = cartaData.effect;
            card.Equip = cartaData.equip;
            card.Monster = cartaData.monster;
            card.Treasure = cartaData.treasure;
            card.Level = cartaData.level;
            card.porta = true;
            card.atualiza();
            card.transform.position = porta.position;
            availablePorta = false;
            return;
        }
    }

    public Vector3 PosiSlot(int index) {
        return cardSlots[index].position;
    }

    public void FreeSlot(int index) {
        CardSlot cs = cardSlots[index].GetComponent<CardSlot>();
        cs.ocupado = false;
    }

    public void EffectMonster(int efeito) {
        GameObject childObject = porta.transform.Find("CartaPorta").gameObject;
        Card card = childObject.GetComponent<Card>();
        card.Power = card.Power + efeito;
        card.atualiza();
    }

    public void GameExplosion(Vector3 objectPosition, int power) {
        Quaternion rotation = Quaternion.identity;

        GameObject explosao = Instantiate(explosionPrefab, objectPosition, rotation);

        Explosion ex = explosao.GetComponent<Explosion>();
        ex.Power = power;
        ServerIdentity si = explosao.GetComponent<ServerIdentity>();
        si.SetSocketReference(this.serverIdentity.GetSocket());
        WhoActivate wa = explosao.GetComponent<WhoActivate>();
        wa.SetWho(this.serverIdentity.GetID());
    }

    public void EmitExplosion(Vector3 objectPosition, int power) {
        Position pos = new Position();
        pos.x = objectPosition.x;
        pos.y = objectPosition.y;

        Efeito efeito = new Efeito();
        efeito.power = power;
        efeito.position = pos;

        serverIdentity.GetSocket().Emit("explosion", JsonSerializer.Serialize(efeito));
    }

    private void Update() {

    }
}
