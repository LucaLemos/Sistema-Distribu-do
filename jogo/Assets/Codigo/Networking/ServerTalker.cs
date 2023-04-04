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
    [Header("GameManager")]
    [SerializeField]
    private GameManager gameManager;

    public static string ClientID {get; private set;}
    private Dictionary<string, JogadorManager> serverObjects;

    void Start()
    {
        io = IO.Socket("http://192.168.68.137:4080"); 
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

        ServerIdentity si = gameManager.GetComponent<ServerIdentity>();
        si.SetSocketReference(this.io);
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

            Action myAction = () => {
                ServerIdentity si = gameManager.GetComponent<ServerIdentity>();
                si.SetID(ClientID);
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("spaw", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            string id = message.id;
            float x = message.position.x;
            float y = message.position.y;
            
            Action myAction = () => {
                GameObject myGameObject = Instantiate(jogadorPrefab, networkContainer);
                myGameObject.name = string.Format("Player: " + id);
                myGameObject.transform.position = new Vector3(x, y, 0);

                ServerIdentity si = myGameObject.GetComponent<ServerIdentity>();
                si.SetControllerID(id);
                si.SetSocketReference(this.io);

                JogadorManager jm = myGameObject.GetComponent<JogadorManager>();
                jm.level = message.level;
                jm.power = message.power;
                jm.atualizaNome();

                if(id == gameManager.GetComponent<ServerIdentity>().GetID()) {
                    UiInventoryController ic = myGameObject.GetComponent<UiInventoryController>();
                    ic.inventoryPage = gameManager.inventoryPage;
                }

                serverObjects.Add(id, jm);
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("move", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            string id = message.id;
            float x = message.position.x;
            float y = message.position.y;

            //Debug.Log("andou aqui");
            //Debug.Log(id);
            serverObjects[id].posiAndar = new Vector3(x, y, 0);

        });

        io.On("getCard", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Carta>(jsonString);
            
            Action myAction = () => {
                gameManager.DrawCard(message);
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("getPorta", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Carta>(jsonString);
            
            Action myAction = () => {
                gameManager.DrawPort(message);
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("explosion", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Efeito>(jsonString);
            
            Action myAction = () => {
                gameManager.GameExplosion(new Vector3(message.position.x, message.position.y, 0), 0);
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("effect", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Efeito>(jsonString);

            Action myAction = () => {
                JogadorManager jm = serverObjects[message.id];
                jm.level = message.level;
                jm.power = message.power;
                jm.atualizaNome();
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("effectMonster", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Efeito>(jsonString);

            Action myAction = () => {
                gameManager.EffectMonster(message.power);
            };
            RunOnMainThread.Enqueue(myAction);
        });
        
        io.On("disconnected", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            Action myAction = () => {
                JogadorManager jm = serverObjects[message.id];
                serverObjects.Clear();
                DestroyImmediate(jm.gameObject);
            };
            RunOnMainThread.Enqueue(myAction);
        });        
    }

    public void AttemptToJoinLobby() {
        io.Emit("joinGame");
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
        public int power { get; set; }
        public int level { get; set; }
}

[Serializable]
public class Position {
        public float x { get; set; }
        public float y { get; set; }
}

[Serializable]
public class Carta {
        public string id { get; set; }
        public string image { get; set; }
        public string type { get; set; }
        public int power { get; set; }
        public bool effect { get; set; }
        public bool equip { get; set; }
        public bool monster { get; set; }
        public int treasure { get; set; }
        public int level { get; set; }
}

[Serializable]
public class Efeito {
        public string id { get; set; }
        public int power { get; set; }
        public int level { get; set; }
        public Position position { get; set; }
}

