using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
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
    public string Body;
    [GreyOut]
    public bool Big;
    [GreyOut]
    public int Treasure;
    [GreyOut]
    public int Level;
    [GreyOut]
    public int Power;

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
            // Set the loaded sprite to the sprite renderer
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
        GameObject childObjectPoder = transform.Find("Poder").gameObject;
        TMP_Text poder = childObjectPoder.GetComponent<TMP_Text>();
        poder.text = Power.ToString();

        GameObject childObjectTitulo = transform.Find("Titulo").gameObject;
        TMP_Text titulo = childObjectTitulo.GetComponent<TMP_Text>();
        titulo.text = Id.ToString();

        if(Type == "Monster") {
            GameObject childObjectTesouro = transform.Find("Corpo").gameObject;
            TMP_Text tesouro = childObjectTesouro.GetComponent<TMP_Text>();
            tesouro.text = Treasure + " Tesouros";

            GameObject childObjectLevel = transform.Find("Tamanho").gameObject;
            TMP_Text level = childObjectLevel.GetComponent<TMP_Text>();
            level.text = Level + " Níveis";

            GameObject childObjectNulo = transform.Find("Valor").gameObject;
            TMP_Text nulo = childObjectNulo.GetComponent<TMP_Text>();
            nulo.text = "";
        }
        if(Type == "Item") {
            GameObject childObjectCorpo = transform.Find("Corpo").gameObject;
            TMP_Text corpo = childObjectCorpo.GetComponent<TMP_Text>();
            corpo.text = Body.ToString();

            GameObject childObjectTamanho = transform.Find("Tamanho").gameObject;
            TMP_Text tamanho = childObjectTamanho.GetComponent<TMP_Text>();
            if(Big) {
                tamanho.text = "Grande";
            }else {
                tamanho.text = "";
            }

            GameObject childObjectPreco = transform.Find("Valor").gameObject;
            TMP_Text preco = childObjectPreco.GetComponent<TMP_Text>();
            preco.text = Treasure + "PO";
        }

        
    }

    public void OnBeginDrag(PointerEventData eventData) {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        transform.position = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
        if(Type == "Effect") {
                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                gm.EmitExplosion(objectPosition, Power);
                gm.GameExplosion(objectPosition, Power);
                Destroy(gameObject);
            }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            if(Type == "Effect") {
                Efeito effect = new Efeito();
                effect.id = gm.serverIdentity.GetID();
                effect.power = Power;
                gm.serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));
                Destroy(gameObject);
            }
        }
    }
}
