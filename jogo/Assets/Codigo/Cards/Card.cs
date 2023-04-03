using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEditor;

public class Card : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    public string SpriteName;
    public string Type;
    public int Power;
    public bool Effect;
    public bool Equip;
    public bool Monster;
    public int Treasure;
    public int Level;

    private GameManager gm;
    public bool porta;
    public int handIndex;

    bool isMouseOver = false;
    private bool isDragging = false;
    private Vector3 startPosition;

    private void Start() {
        // Get the child object using its name
        GameObject childObject = transform.Find("Image").gameObject;
        // Get the SpriteRenderer component on the child object
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
        if (Effect && isMouseOver && Input.GetMouseButtonDown(1)) {
            Debug.Log("Right mouse button clicked over object!");

            Efeito effect = new Efeito();
            effect.id = gm.serverIdentity.GetID();
            effect.power = Power;
            gm.serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));
            Destroy(gameObject);
            // Code to execute when the right mouse button is pressed over the object
        }
    }

    void OnMouseDown() {
        if(!porta) {
            isDragging = true;
            startPosition = transform.position;
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
            if(Effect) {
                isDragging = false;
                gm.availableCardsSlots[handIndex] = true;
                gameObject.SetActive(false);
    
                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
                Vector3 objectPosition = Camera.main.ScreenToWorldPoint(mousePosition);
    
                gm.EmitExplosion(objectPosition, Power);
                gm.GameExplosion(objectPosition, Power);
                Destroy(gameObject);
            }
            transform.position = startPosition;
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
