using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEditor;

public class Card : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    [Header("Dados")]
    [GreyOut]
    public string SpriteName;
    [GreyOut]
    public string Type;
    [GreyOut]
    public int Power;
    [GreyOut]
    public bool Effect;
    [GreyOut]
    public bool Equip;
    [GreyOut]
    public bool Monster;
    [GreyOut]
    public int Treasure;
    [GreyOut]
    public int Level;

    public GameManager gm;
    public int handIndex;
    public bool porta;

    public bool isMouseOver = false;
    public bool isDragging = false;
    public bool isActive = false;

    private void Start() {
        GameObject childObject = transform.Find("Image").gameObject;
        spriteRenderer = childObject.GetComponent<SpriteRenderer>();

        string path = "Assets/Sprites/CardsImage/" + SpriteName;
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        
        if (sprite != null)
        {
            // Set the loaded sprite to the sprite renderer
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogError("Sprite not found at path: " + path);
        }

        gm = FindObjectOfType<GameManager>(); 
    }

    void Update()
    {
        if (isMouseOver && Input.GetMouseButtonDown(1)) {
            if(isDragging) {
                isDragging = false;
                //transform.position = gm.PosiSlot(handIndex);
            }else if(Effect) {
                Efeito effect = new Efeito();
                effect.id = gm.serverIdentity.GetID();
                effect.power = Power;
                gm.serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));
                //gm.FreeSlot(handIndex);
                Destroy(gameObject);
            }
        }
    }

    void OnMouseDown() {
        //Debug.Log("apertou");
        if(!porta) {
        isDragging = true;
        }
    }

    void OnMouseDrag() {
        if(!porta) {
            if (isDragging)
            {
                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                transform.position = objectPosition;
            }
        }
    }

    void OnMouseUp() {
        if(!porta) {
            if(Effect && isDragging && isActive) {
                isDragging = false;

                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                //gm.availableCardsSlots[handIndex] = true;
                gm.EmitExplosion(objectPosition, Power);
                gm.GameExplosion(objectPosition, Power);
                //gm.FreeSlot(handIndex);
                Destroy(gameObject);
            }else {
                isDragging = false;
                //transform.position = gm.PosiSlot(handIndex);
            }
        }
    }

    void OnMouseOver()
    {
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        isMouseOver = false;
    }

    public void atualiza() {
        GameObject childObject = transform.Find("Numero").gameObject;
        TMP_Text textMash = childObject.GetComponent<TMP_Text>();
        textMash.text = Power.ToString();
    }
}
