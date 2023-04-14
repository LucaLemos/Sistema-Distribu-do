using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardActionPanel : MonoBehaviour {
    [SerializeField]
    private GameObject buttonPrefab;

    public void AddButon(string name, Action onClickAction) {
        GameObject button = Instantiate(buttonPrefab, transform);
        button.GetComponent<Button>().onClick.AddListener(() => onClickAction());
        button.GetComponentInChildren<TMPro.TMP_Text>().text = name;
    }

    internal void Toggle(bool val) {
        if(val == true) {
            //RemoveOldButtons();
        }
        gameObject.SetActive(val);
    }

    public void RemoveOldButtons() {
        foreach(Transform trasnformChildObjects in transform) {
            Destroy(trasnformChildObjects.gameObject);
        }
    }
}
