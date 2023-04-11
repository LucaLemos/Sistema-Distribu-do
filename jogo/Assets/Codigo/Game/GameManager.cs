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
    public Transform porta;
    private bool availablePorta = true;
    public CardSlotNew[] hand;

    
    [Header("Prefab")]
    public GameObject cardPrefab;
    public GameObject explosionPrefab;

    public void DrawCard(Carta cartaData) {
        for(int i = 0; i < hand.Length; i++) {
            CardSlotNew slot = hand[i];
            DragAndDrop itemInSlot = slot.GetComponentInChildren<DragAndDrop>();
            if(itemInSlot == null) {
                Quaternion rotation = Quaternion.identity;

                GameObject prefab = Instantiate(cardPrefab, slot.transform);
                DragAndDrop card = prefab.GetComponent<DragAndDrop>();

                card.Id = cartaData.id;
                card.SpriteName = cartaData.image;
                card.Deck = cartaData.deck;
                card.Type = cartaData.type;
                card.Body = cartaData.body;
                card.Big = cartaData.big;
                card.Treasure = cartaData.treasure;
                card.Level = cartaData.level;
                card.Power = cartaData.power;

                card.porta = false;
                card.atualiza();
                card.handIndex = i;
                
                return;
            }
        }
    }
    public void DrawHand(){
        Debug.Log("apertouMao");
        serverIdentity.GetSocket().Emit("getCardTesouro");
        //serverIdentity.GetSocket().Emit("getCardTesouro");
        //serverIdentity.GetSocket().Emit("getCardTesouro");
        //serverIdentity.GetSocket().Emit("getCardPorta");
        //serverIdentity.GetSocket().Emit("getCardPorta");
        //serverIdentity.GetSocket().Emit("getCardPorta");
    }

    public void DrawPort(Carta cartaData) {
        if(availablePorta == true) {
            Quaternion rotation = Quaternion.identity;

            GameObject prefab = Instantiate(cardPrefab, porta);
            DragAndDrop card = prefab.GetComponent<DragAndDrop>();
            prefab.name = "CartaPorta";
            ServerIdentity si = prefab.GetComponent<ServerIdentity>();
            si.SetSocketReference(serverIdentity.GetSocket());
            si.SetID("1");

            card.Id = cartaData.id;
            card.SpriteName = cartaData.image;
            card.Deck = cartaData.deck;
            card.Type = cartaData.type;
            card.Body = cartaData.body;
            card.Big = cartaData.big;
            card.Treasure = cartaData.treasure;
            card.Level = cartaData.level;
            card.Power = cartaData.power;

            card.porta = true;
            card.atualiza();
            availablePorta = false;
                
            return;
        }
    }
    public void DrawPortEmit(){
        Debug.Log("apertouPorta");
        serverIdentity.GetSocket().Emit("getPorta");
    }

    public void EffectMonster(int efeito) {
        DragAndDrop dd = porta.GetComponentInChildren<DragAndDrop>();
        dd.Power = dd.Power + efeito;
        dd.atualiza();
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
