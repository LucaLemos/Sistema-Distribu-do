using System;
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
        ServerIdentity si = null;
        try
        {
            // code that might throw an exception
            si = collision.gameObject.GetComponent<ServerIdentity>();
        }
        catch (Exception ex)
        {
            // code that handles the exception
            Console.WriteLine("An exception occurred: " + ex.Message);
        }
        
        if(Power != 0 && si != null) {
            if(si.GetID() != "" && si.GetID() != "1") {
                Debug.Log(si.GetID());

                Efeito effect = new Efeito();
                effect.id = si.GetID();
                effect.power = Power;

                serverIdentity.GetSocket().Emit("effect", JsonSerializer.Serialize(effect));
            }
            else if(si.GetID() == "1") {
                Debug.Log(si.GetID());

                Efeito effect = new Efeito();
                effect.id = si.GetID();
                effect.power = Power;

                serverIdentity.GetSocket().Emit("effectMonster", JsonSerializer.Serialize(effect));
            }
        }
    }
}
