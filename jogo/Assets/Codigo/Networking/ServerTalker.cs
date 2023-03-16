using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.Json;
using UnityEngine;
using Quobject.SocketIoClientDotNet.Client;

public class ServerTalker : MonoBehaviour
{
    private Socket io;
    public static readonly ConcurrentQueue<Action> RunOnMainThread = new ConcurrentQueue<Action>();

    [Header("Network Client")]
    [SerializeField]
    private Transform networkContainer;
    
    private Dictionary<string, GameObject> serverObjects;

    void Start()
    {
        io = IO.Socket("http://localhost:4080");   
        inicializar();
        eventos();
    }

    // Update is called once per frame
    void Update()
    {
        if(!RunOnMainThread.IsEmpty)
        {
            while(RunOnMainThread.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }

    

    private void inicializar() {
        serverObjects = new Dictionary<string, GameObject>();
    }

    private void eventos() {
        io.On(Socket.EVENT_CONNECT, () => {
	        Debug.Log("Conectei!!!");
        });

        io.On(Socket.EVENT_DISCONNECT, (data) => {
            Debug.Log("Desconectei :(");
        });

        io.On("register", (data) => {
	        var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            string id = message.id;
	        Debug.Log("Seu id e: " + id);
        });

        io.On("spaw", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            string id = message.id;
            
            Action myAction = () =>
            {
                GameObject myObject = new GameObject("ID: " + id);
                myObject.transform.SetParent(networkContainer);
                serverObjects.Add(id, myObject);
            };
            RunOnMainThread.Enqueue(myAction);
        });
        
        io.On("disconnected", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            string id = message.id;

            Action myAction = () =>
            {
                GameObject go = serverObjects[id];
                Destroy(go);
                serverObjects.Remove(id);
            };
            RunOnMainThread.Enqueue(myAction);
            
        });
    }

    public class Jogador {
        public string username { get; set; }
        public string id { get; set; }
    }

    void OnDestroy() {
        io.Disconnect();
    }
}

