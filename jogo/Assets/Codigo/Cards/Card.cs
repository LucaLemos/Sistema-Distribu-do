using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
    [Header("UI")]
    public Image image;

    [Header("Dados")]
    [GreyOut]
    public string Id;
    //[GreyOut]
    public string SpriteName;
    [GreyOut]
    public string Deck;
    [GreyOut]
    public string Type;
    [GreyOut]
    public string Descricao;
    [GreyOut]
    public int Treasure;
    [GreyOut]
    public int Level;
    [GreyOut]
    public int Power;
    [GreyOut]
    public Corpo corpo;
    [GreyOut]
    public Efeito efeito;

    [SerializeField]
    private CardActionPanel actionPanel;
    public Image equipadoIcon;
    public bool equipado;
    public GameManager gm;
    public int handIndex;
    public bool porta;
    private Image cardSprite;

    [HideInInspector] public Transform parentAfterDrag;

    private void Start() {
        GameObject childObject = transform.Find("Image").gameObject;
        cardSprite = childObject.GetComponent<Image>();

        string path = "Assets/Sprites/CardsImage/" + SpriteName;
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        
        if (sprite != null)
        {
            cardSprite.sprite = sprite;
        }
        else
        {
            Debug.LogError("Sprite not found at path: " + path);
        }

        gm = FindObjectOfType<GameManager>(); 
    }

    void Update() {
        
    }

    public void atualiza() {
        GameObject childObjectTitulo = transform.Find("Titulo").gameObject;
        TMP_Text titulo = childObjectTitulo.GetComponent<TMP_Text>();
        titulo.text = Id.ToString();

        GameObject childObjectPoder = transform.Find("Poder").gameObject;
        TMP_Text poder = childObjectPoder.GetComponent<TMP_Text>();
        poder.text = Power.ToString();

        if(Type == "Monster") {
            GameObject childObjectTesouro = transform.Find("Corpo").gameObject;
            TMP_Text tesouro = childObjectTesouro.GetComponent<TMP_Text>();
            tesouro.text = Treasure + " Tesouros";

            GameObject childObjectLevel = transform.Find("Valor").gameObject;
            TMP_Text level = childObjectLevel.GetComponent<TMP_Text>();
            level.text = Level + " Níveis";

            GameObject childObjectNulo = transform.Find("Tamanho").gameObject;
            TMP_Text nulo = childObjectNulo.GetComponent<TMP_Text>();
            nulo.text = "";

            actionPanel.AddButon("Descartar", destruir);
        }
        if(Type == "Curse") {
            poder.text = "";

            actionPanel.AddButon("Descartar", destruir);
        }
        if(Type == "Effect") {
            if(!this.efeito.monsterOnly) {
                actionPanel.AddButon("Usar", usarEfeito);
            }
            actionPanel.AddButon("Descartar", destruir);
        }
        if(Type == "Classe") {
            poder.text = "";

            if(equipado) {
                actionPanel.AddButon("Largar", desquipar);
            }else {
                actionPanel.AddButon("Usar", equipar);
            }
            actionPanel.AddButon("Descartar", destruir);
        }
        if(Type == "Race") {
            poder.text = "";

            if(!equipado) {
                actionPanel.AddButon("Usar", equipar);
                actionPanel.AddButon("Descartar", destruir);
            }
        }
        if(Type == "Equip") {
            GameObject childObjectParte = transform.Find("Corpo").gameObject;
            TMP_Text parte = childObjectParte.GetComponent<TMP_Text>();
            parte.text = parteDoCorpo(corpo);

            GameObject childObjectPreco = transform.Find("Valor").gameObject;
            TMP_Text preco = childObjectPreco.GetComponent<TMP_Text>();
            preco.text = Treasure + "PO";
            
            GameObject childObjectTamanho = transform.Find("Tamanho").gameObject;
            TMP_Text tamanho = childObjectTamanho.GetComponent<TMP_Text>();
            if(corpo.grande > 0) {
                tamanho.text = "Grande";
            }else {
                tamanho.text = "";
            }

            if(equipado) {
                actionPanel.AddButon("Desequipar", desquipar);
            }else {
                actionPanel.AddButon("Equipar", equipar);
            }
            actionPanel.AddButon("Vender", vender);
        }

        
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if(!porta) {
            image.raycastTarget = false;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if(!porta) {
            Vector3 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            transform.position = mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if(!porta) {
            image.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
            if(Type == "Effect" && gm.CartasAtivas && !efeito.playerOnly) {
                    Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                    Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                    gm.EmitExplosion(objectPosition);
                    gm.GameExplosion(objectPosition, efeito.power, efeito);
                    destruir();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(!porta) {
            if (eventData.button == PointerEventData.InputButton.Right) {
                actionPanel.Toggle(true);
            }
        }
        if (eventData.button == PointerEventData.InputButton.Left) {
            gm.DescreverCarta(this);
        }
    }

    private void OnMouseExit() {
        if(!porta) {
            actionPanel.Toggle(false);
        }
    }

    public void destruir() {
        Destroy(gameObject);
    }

    public void vender() {
        gm.venderItem(this);
    }

    public void usarEfeito() {
        this.efeito.id = gm.serverIdentity.GetID();
        gm.serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(this.efeito));
        destruir();
    }

    public void equipar() {
        bool consegue = gm.equiparItem(this);
        if(consegue) {
            equipadoIcon.gameObject.SetActive(true);
            actionPanel.Toggle(false);
            equipado = true;
            actionPanel.RemoveOldButtons();
            atualiza();
        }
    }
    public void desquipar() {
        gm.desEquiparItem(this);
        equipadoIcon.gameObject.SetActive(false);
        actionPanel.Toggle(false);
        equipado = false;
        actionPanel.RemoveOldButtons();
        atualiza();
    }

    public void showCardAction() {
        actionPanel.Toggle(true);
    }

    public string parteDoCorpo(Corpo corp) {
        if(corp.helm > 0) {
            return "Usar na Cabeça";
        }else if(corp.armor > 0) {
            return "Armadura";
        }else if(corp.hands == 1) {
                return "1 Mão";
        }else if(corp.hands == 2) {
                return "2 Mãos";
        }else if(corp.boot > 0) {
                return "Calçado";
        }else if(corp.grande > 0) {
                return "Grande";
        }else if(corp.classe != "") {
                return "Classe";
        }else if(corp.race != "") {
                return "Race";
        }
        return "";
    }
}
