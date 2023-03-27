using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class GridSystem : MonoBehaviour {
    public static GridSystem current;

    public GridLayout gridLayout;
    public Tilemap MainTilemap;
    public Tilemap TempTilemap;

    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private Card temp;
    private Vector3 prevPos;

    private void Awake() {
        current = this;
    }

    private void Start() {
    }

    private void Update() {
        if(!temp) {
            return;
        }

        if(Input.GetMouseButtonDown(0)) {
            if(EventSystem.current.IsPointerOverGameObject(0)) {
                return;
            }
            if(!temp.placed) {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = gridLayout.LocalToCell(touchPos);

                if(prevPos != cellPos) {
                    temp.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPos + new Vector3(.5f, .5f, .0f));
                    prevPos = cellPos;
                }
            }
        }
    }

    public void InitializeWithCard(GameObject card) {
        temp = Instantiate(card, Vector3.zero, Quaternion.identity).GetComponent<Card>();
        
        Material material = temp.GetComponent<SpriteRenderer>().material;
        Color color = material.color;
        color.a = 0.5f;
        material.color = color;
    }

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap) {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach(var v in area.allPositionsWithin) {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }

        return array;
    }

    private static void FillTiles(TileBase[] arr, TileType type) {
        for(int i = 0; i < arr.Length; i++) {
            arr[i] = tileBases[type];
        }
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap) {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] array = new TileBase[size];
        FillTiles(array, type);
        tilemap.SetTilesBlock(area, array);
    }

}

public enum TileType {
    Empty,
    White,
    Green,
    Red
}