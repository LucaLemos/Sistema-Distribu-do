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
    [SerializeField]
    private GameObject jogadorPrefab;

    public static string ClientID {get; private set;}
    private Dictionary<string, JogadorManager> serverObjects;

    void Start()
    {
        io = IO.Socket("http://localhost:4080"); 
        inicializar();
        eventos();

    }

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
        serverObjects = new Dictionary<string, JogadorManager>();
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

            ClientID = message.id;
	        Debug.Log("Seu id e: " + ClientID);
        });

        io.On("spaw", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            string id = message.id;
            float x = message.position.x;
            float y = message.position.y;
            
            Action myAction = () =>
            {
                GameObject myGameObject = Instantiate(jogadorPrefab, networkContainer);
                myGameObject.name = string.Format("Player: " + id);
                myGameObject.transform.position = new Vector3(x, y, 0);
                ServerIdentity si = myGameObject.GetComponent<ServerIdentity>();
                si.SetControllerID(id);
                si.SetSocketReference(this.io);
                serverObjects.Add(id, myGameObject.GetComponent<JogadorManager>());
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("mover", (data) => {
            //Debug.Log("move");
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            string id = message.id;
            float x = message.position.x;
            float y = message.position.y;

            //Debug.Log(new Vector3(x, y, 0));

            serverObjects[id].posi = new Vector3(x, y, 0);
        });
        
        io.On("disconnected", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            string id = message.id;

            Action myAction = () => {
                GameObject myGameObject = serverObjects[id].gameObject;
                Destroy(myGameObject);
                serverObjects.Remove(id);
            };
            RunOnMainThread.Enqueue(myAction);
        });        
    }

    void OnDestroy() {
        io.Disconnect();
    }
}

[Serializable]
public class Jogador {
        public string id { get; set; }
        public string username { get; set; }
        public Position position { get; set; }
}

[Serializable]
public class Position {
        public float x { get; set; }
        public float y { get; set; }
}

