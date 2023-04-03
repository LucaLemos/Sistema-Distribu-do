using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

public class Explosion : MonoBehaviour {

    [SerializeField]
    private ServerIdentity serverIdentity;
    [SerializeField]
    private WhoActivate whoActivate;

    public int Power;

    public void DestroyObject() {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        ServerIdentity si = collision.gameObject.GetComponent<ServerIdentity>();
        if(Power != 0) {
            if(si != null && si.GetID() != "") {
            //Debug.Log(si.GetID());
            //Debug.Log(Power);
            Efeito effect = new Efeito();
            effect.id = si.GetID();
            effect.power = Power;

            serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));
            }
        }
    }
}
