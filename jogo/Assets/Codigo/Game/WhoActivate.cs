using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhoActivate : MonoBehaviour {
    [SerializeField]
    [GreyOut]
    private string who;

    public void SetWho(string ID) {
        who = ID;
    }

    public string GetWho() {
        return who;
    }

}
