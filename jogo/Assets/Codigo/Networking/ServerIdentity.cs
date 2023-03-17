using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quobject.SocketIoClientDotNet.Client;

public class ServerIdentity : MonoBehaviour {

    [Header("Dados")]
    [SerializeField]
    [GreyOut]
    private string ID;
    [SerializeField]
    [GreyOut]
    private bool Controlavel;

    private Socket io;

    void Awake() {
        Controlavel = false;
    }

    public void SetControllerID(string id) {
        ID = id;
        Controlavel = (ServerTalker.ClientID == ID) ? true : false;
    }

    public void SetSocketReference(Socket socket) {
        io = socket;
    }

    public string GetID() {
        return ID;
    }

    public bool IsControlavel() {
        return Controlavel;
    }

    public Socket GetSocket() {
        return io;
    }
}
