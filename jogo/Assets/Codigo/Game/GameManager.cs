using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Server Identity")]
    [SerializeField]
    public ServerIdentity serverIdentity;
    public JogadorManager jogador;

    [Header("Cards")]
    public Transform porta;
    private bool availablePorta = true;
    public CardSlot[] hand;
    public CardSlot[] inventario;
    public CardSlot[] recompensa;
    public GameObject recompensaMenu;
    public GameObject descricao;
    
    [Header("Prefab")]
    public GameObject cardPrefab;
    public GameObject explosionPrefab;

    [Header("Gameplay")]
    public bool Comecou;
    public bool CartasAtivas;
    public TextMeshProUGUI Dinheiro;
    public int jogando = 0;
    public List<JogadorManager> jogadores;
    public JogadorManager jogadorAtual;

    [Header("Layout")]
    public GameObject menuButtons;
    public GameObject buttons;
    public GameObject esperandoStatus;
    public GameObject jogadorSelecionadoStatus;

    void Start() {
        Comecou = false;
    }
    private void Update() {
        if(!Comecou && jogadores.Count >= 1 && todosProntos()) {
            comecarJogo();
        }
    }

    //manipular itens
    public void venderItem(Card itemInSlot) {
        if(itemInSlot != null) {
            if(itemInSlot.Type == "Equip") {
                jogadores[0].gold += itemInSlot.Treasure;
                if(jogadores[0].gold >= 1000) {
                    jogadores[0].gold -= 1000;
                    Efeito effect = new Efeito();
                    effect.id = serverIdentity.GetID();
                    effect.level = 1;
                    serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));
                }
                Dinheiro.text = "GP " + jogadores[0].gold;
            }
            itemInSlot.destruir();
        }
    }
    public bool equiparItem(Card equipamento) {
        for(int i = 0; i < inventario.Length; i++) {
            CardSlot slot = inventario[i];
            Card itemInSlot = slot.GetComponentInChildren<Card>();
            if(itemInSlot == null) {
                if(equipamento.Type == "Equip") {
                    if(equipamento.corpo.helm <= jogadores[0].corpo.helm &&
                    equipamento.corpo.armor <= jogadores[0].corpo.armor &&
                    equipamento.corpo.hands <= jogadores[0].corpo.hands &&
                    equipamento.corpo.boot <= jogadores[0].corpo.boot &&
                    equipamento.corpo.grande <= jogadores[0].corpo.grande
                    ) 
                    {
                        jogadores[0].corpo.helm -= equipamento.corpo.helm;
                        jogadores[0].corpo.armor -= equipamento.corpo.armor;
                        jogadores[0].corpo.hands -= equipamento.corpo.hands;
                        jogadores[0].corpo.boot -= equipamento.corpo.boot;
                        jogadores[0].corpo.grande -= equipamento.corpo.grande;

                        equipamento.transform.SetParent(slot.transform);

                        Efeito effect = new Efeito();
                        effect.id = serverIdentity.GetID();
                        effect.power = equipamento.Power;
                        serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));
                        return true;
                    }
                }
                if(equipamento.Type == "Classe") {
                    if(jogadores[0].corpo.classe == "") {
                        jogadores[0].corpo.classe = equipamento.corpo.classe;

                        equipamento.transform.SetParent(slot.transform);

                        return true;
                    }
                }
                if(equipamento.Type == "Race") {
                    if(jogadores[0].corpo.race == "") {
                        jogadores[0].corpo.race = equipamento.corpo.race;

                        equipamento.transform.SetParent(slot.transform);

                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void desEquiparItem(Card equipamento) {
        for(int i = 0; i < inventario.Length; i++) {
            CardSlot slot = inventario[i];
            Card itemInSlot = slot.GetComponentInChildren<Card>();
            if(itemInSlot == equipamento) {
                jogadores[0].corpo.helm += equipamento.corpo.helm;
                jogadores[0].corpo.armor += equipamento.corpo.armor;
                jogadores[0].corpo.hands += equipamento.corpo.hands;
                jogadores[0].corpo.boot += equipamento.corpo.boot;
                jogadores[0].corpo.grande += equipamento.corpo.grande;
                try {
                    // code that might throw an exception
                    jogadores[0].corpo.classe = jogadores[0].corpo.classe.Remove(jogadores[0].corpo.classe.IndexOf(equipamento.corpo.classe), equipamento.corpo.classe.Length);
                    jogadores[0].corpo.race = jogadores[0].corpo.race.Remove(jogadores[0].corpo.race.IndexOf(equipamento.corpo.race), equipamento.corpo.race.Length);
                }
                catch (Exception ex) {
                    // code that handles the exception
                    Console.WriteLine("An exception occurred: " + ex.Message);
                }

                Efeito effect = new Efeito();
                effect.id = serverIdentity.GetID();
                effect.power = -equipamento.Power;
                serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));
            }
        }
    }

    //gameplayFunctions
    public void comecarJogo() {
        serverIdentity.GetSocket().Emit("jogadorTurno", jogando);
        Comecou = true;
        DrawHand();
    }
    public void comecarPorta() {
        DrawPortEmit();
        ativarCartas();
        serverIdentity.GetSocket().Emit("comecarPorta");
    }
    public void terminarPortaEmit() {
        serverIdentity.GetSocket().Emit("terminarPorta");

        Card PortaInSlot = porta.GetComponentInChildren<Card>();
        if(PortaInSlot.Type == "Monster") {
            if(PortaInSlot.Power > jogadorAtual.power) {
                PortaInSlot.efeito.id = jogadorAtual.serverIdentity.GetID();
                serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(PortaInSlot.efeito));
            }else {
                Efeito effect = new Efeito();
                effect.id = serverIdentity.GetID();
                effect.level = PortaInSlot.Level;
                serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));

                for(int i = 0; i < PortaInSlot.Treasure ;i++) {
                    serverIdentity.GetSocket().Emit("getRecompensa");
                }

                buttons.SetActive(false);
                recompensaMenu.SetActive(true);
            }
        }
        if(PortaInSlot.Type == "Curse") {
            PortaInSlot.efeito.id = jogadorAtual.serverIdentity.GetID();
            Debug.Log(PortaInSlot.efeito.destruir);
            if(PortaInSlot.efeito.destruir) {
                removerItem(PortaInSlot.corpo);
            }
            serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(PortaInSlot.efeito));
        }
    }
    public void terminarPorta() {
        Card PortaInSlot = porta.GetComponentInChildren<Card>();
        availablePorta = true;
        PortaInSlot.destruir();
        ResetarPoder();
        ProximoJogador();
        ativarCartas();
    }

    public void atualizaLayout() {
        if(jogadorAtual.serverIdentity.GetID() == serverIdentity.GetID()) {
            menuButtons.SetActive(true);
            esperandoStatus.SetActive(false);
            jogadorSelecionadoStatus.SetActive(true);
        }else {
            menuButtons.SetActive(true);
            esperandoStatus.SetActive(false);
            jogadorSelecionadoStatus.SetActive(false);
        }
    }
    public void ativarCartas() {
        if(CartasAtivas) {
            CartasAtivas = false;
        }else {
            CartasAtivas = true;
        }
    }

    public bool todosProntos() {
        foreach (JogadorManager jogador in jogadores) {
            if(!jogador.Pronto) {
                return false;
            }
        }
        return true;
    }
    public void pronto() {
        jogadores[0].prontoAtv();
    }

    public void ResetarPoder() {
        int poderTotal = jogadores[0].level;
        for(int i = 0; i < inventario.Length; i++) {
            CardSlot slot = inventario[i];
            Card itemInSlot = slot.GetComponentInChildren<Card>();
            if(itemInSlot != null && itemInSlot.equipado) {
                poderTotal += itemInSlot.Power;
            }
        }

        Efeito effect = new Efeito();
        effect.id = jogadores[0].serverIdentity.GetID();
        effect.power = poderTotal;
        serverIdentity.GetSocket().Emit("zerarPoder", JsonSerializer.Serialize(effect));
    }
    public void removerItem(Corpo corpo) {
        for(int i = 0; i < inventario.Length; i++) {
            CardSlot slot = inventario[i];
            Card itemInSlot = slot.GetComponentInChildren<Card>();
            if(itemInSlot != null && itemInSlot.equipado) {
                Debug.Log(itemInSlot.parteDoCorpo(corpo) + " - " + itemInSlot.parteDoCorpo(itemInSlot.corpo));
                if(itemInSlot.parteDoCorpo(corpo) == itemInSlot.parteDoCorpo(itemInSlot.corpo) ) {
                    itemInSlot.desquipar();
                    itemInSlot.destruir();
                }
            }
        }
    }
    //cards functios
    public void DescreverCarta(Card selecionada) {
        GameObject childObjectText = descricao.transform.Find("Descricao").gameObject;
        TextMeshProUGUI textPro = childObjectText.GetComponent<TextMeshProUGUI>();
        textPro.text = selecionada.Descricao;

        GameObject childObjectCard = descricao.transform.Find("CardSlot").gameObject;
        DeleteChildren(childObjectCard.transform);
        GameObject prefab = Instantiate(cardPrefab, childObjectCard.transform);
        Card card = prefab.GetComponent<Card>();

        card.Id = selecionada.Id;
        card.SpriteName = selecionada.SpriteName;
        card.Deck = selecionada.Deck;
        card.Type = selecionada.Type;
        card.Descricao = selecionada.Descricao;
        card.Treasure = selecionada.Treasure;
        card.Level = selecionada.Level;
        card.Power = selecionada.Power;
        card.corpo = selecionada.corpo;
        card.efeito = selecionada.efeito;
        card.porta = true;
        
        card.atualiza();
    }

    public void DrawCard(Carta cartaData) {
        for(int i = 0; i < hand.Length; i++) {
            CardSlot slot = hand[i];
            Card itemInSlot = slot.GetComponentInChildren<Card>();
            if(itemInSlot == null) {
                GameObject prefab = Instantiate(cardPrefab, slot.transform);
                Card card = prefab.GetComponent<Card>();

                card.Id = cartaData.id;
                card.SpriteName = cartaData.image;
                card.Deck = cartaData.deck;
                card.Type = cartaData.type;
                card.Descricao = cartaData.descricao;
                card.Treasure = cartaData.treasure;
                card.Level = cartaData.level;
                card.Power = cartaData.power;
                card.corpo = cartaData.corpo;
                card.efeito = cartaData.efeito;

                card.porta = false;
                card.atualiza();
                card.handIndex = i;
                
                return;
            }
        }
    }
    public void DrawHand(){
        serverIdentity.GetSocket().Emit("getCardTesouro");
        serverIdentity.GetSocket().Emit("getCardTesouro");
        serverIdentity.GetSocket().Emit("getCardTesouro");
        serverIdentity.GetSocket().Emit("getCardPorta");
        serverIdentity.GetSocket().Emit("getCardPorta");
        serverIdentity.GetSocket().Emit("getCardPorta");
    }
    public void DrawHandPorta(){
        serverIdentity.GetSocket().Emit("getCardPorta");
    }
    public void DrawHandTesouro(){
        serverIdentity.GetSocket().Emit("getCardTesouro");
    }

    public void DrawPort(Carta cartaData) {
        if(availablePorta == true) {
            Quaternion rotation = Quaternion.identity;

            GameObject prefab = Instantiate(cardPrefab, porta);
            Card card = prefab.GetComponent<Card>();
            prefab.name = "CartaPorta";
            ServerIdentity si = prefab.GetComponent<ServerIdentity>();
            si.SetSocketReference(serverIdentity.GetSocket());
            si.SetID("1");

            card.Id = cartaData.id;
            card.SpriteName = cartaData.image;
            card.Deck = cartaData.deck;
            card.Type = cartaData.type;
            card.Descricao = cartaData.descricao;
            card.Treasure = cartaData.treasure;
            card.Level = cartaData.level;
            card.Power = cartaData.power;
            card.corpo = cartaData.corpo;
            card.efeito = cartaData.efeito;

            card.porta = true;
            card.atualiza();
            availablePorta = false;
                
            return;
        }
    }
    public void DrawPortEmit(){
        serverIdentity.GetSocket().Emit("getPorta");
    }

    public void DrawRecompensa(Carta cartaData) {
        for(int i = 0; i < recompensa.Length; i++) {
            CardSlot slot = recompensa[i];
            Card itemInSlot = slot.GetComponentInChildren<Card>();
            if(itemInSlot == null) {
                GameObject prefab = Instantiate(cardPrefab, slot.transform);
                Card card = prefab.GetComponent<Card>();

                card.Id = cartaData.id;
                card.SpriteName = cartaData.image;
                card.Deck = cartaData.deck;
                card.Type = cartaData.type;
                card.Descricao = cartaData.descricao;
                card.Treasure = cartaData.treasure;
                card.Level = cartaData.level;
                card.Power = cartaData.power;
                card.corpo = cartaData.corpo;
                card.efeito = cartaData.efeito;

                card.porta = false;
                card.atualiza();
                card.handIndex = i;
                
                return;
            }
        }
    }

    //effect function
    public void EffectMonster(Efeito efeito) {
        Card card = porta.GetComponentInChildren<Card>();
        card.Power += efeito.power;
        card.Treasure += efeito.treasure;
        card.atualiza();
    }
    
    //explosions functions
    public void GameExplosion(Vector3 objectPosition, int power, Efeito efeito) {
        Quaternion rotation = Quaternion.identity;
        GameObject explosao = Instantiate(explosionPrefab, objectPosition, rotation);

        Explosion ex = explosao.GetComponent<Explosion>();
        ex.Power = power;
        ex.efeito = efeito;

        ServerIdentity si = explosao.GetComponent<ServerIdentity>();
        si.SetSocketReference(this.serverIdentity.GetSocket());

        WhoActivate wa = explosao.GetComponent<WhoActivate>();
        wa.SetWho(this.serverIdentity.GetID());
    }
    public void EmitExplosion(Vector3 objectPosition) {
        Position pos = new Position();
        pos.x = objectPosition.x;
        pos.y = objectPosition.y;

        Explosao explosao = new Explosao();
        explosao.position = pos;

        serverIdentity.GetSocket().Emit("explosion", JsonSerializer.Serialize(explosao));
    }

    //extras
    void ProximoJogador() {
        if(jogando < (jogadores.Count - 1)) {
            jogando++;
        }else {
            jogando = 0;
        }
        //Debug.Log(jogando);
        serverIdentity.GetSocket().Emit("jogadorTurno", jogando);
    }

    void DeleteChildren(Transform parent)
    {
        int childCount = parent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            // Destroy the child object
            Destroy(parent.GetChild(i).gameObject);
        }
    }

}
