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

    void Start() {
        io = IO.Socket("http://localhost:4080"); 
        //io = IO.Socket("http://192.168.68.139:4080"); 
        inicializar();
        eventos();
    }

    void Update() {
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
                jm.gold = message.gold;
                jm.corpo = message.corpo;
                jm.atualizaNome();

                serverObjects.Add(id, jm);
                
                gameManager.jogadores.Add(jm);
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

        io.On("pronto", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            serverObjects[message.id].Pronto = message.pronto;
        });

        io.On("jogadorTurno", (data) => {
            string id = (string)data;
            
            gameManager.jogadorAtual =  serverObjects[id];

            Action myAction = () => {
                gameManager.atualizaLayout();
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("comecarPorta", (data) => {
            Action myAction = () => {
                gameManager.ativarCartas();
                gameManager.menuButtons.SetActive(false);
            };
            RunOnMainThread.Enqueue(myAction);
        });
        io.On("terminarPorta", (data) => {
            Action myAction = () => {
                gameManager.terminarPorta();
            };
            RunOnMainThread.Enqueue(myAction);
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

        io.On("getRecompensa", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Carta>(jsonString);
            
            Action myAction = () => {
                gameManager.DrawRecompensa(message);
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
                gameManager.EffectMonster(message);
            };
            RunOnMainThread.Enqueue(myAction);
        });

        io.On("explosion", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Explosao>(jsonString);
            
            Action myAction = () => {
                gameManager.GameExplosion(new Vector3(message.position.x, message.position.y, 0), 0, new Efeito());
            };
            RunOnMainThread.Enqueue(myAction);
        });
        
        io.On("disconnected", (data) => {
            var jsonString = data.ToString();
            var message = JsonSerializer.Deserialize<Jogador>(jsonString);

            Action myAction = () => {
                JogadorManager jm = serverObjects[message.id];
                gameManager.jogadores.Remove(jm);
                serverObjects.Remove(message.id);
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
        public int level { get; set; }
        public int power { get; set; }
        public int gold { get; set; }
        public Corpo corpo { get; set; }
        public Position position { get; set; }
        public bool pronto { get; set; }
}

[Serializable]
public class Position {
        public float x { get; set; }
        public float y { get; set; }
}
[Serializable]
public class Corpo {
        public int helm { get; set; }
        public int armor { get; set; }
        public int hands { get; set; }
        public int boot { get; set; }
        public int grande { get; set; }
        public string classe { get; set; }
        public string race { get; set; }
}

[Serializable]
public class Carta {
        public string id { get; set; }
        public string image { get; set; }
        public string deck { get; set; }
        public string type { get; set; }
        public string descricao { get; set; }
        public int treasure { get; set; }
        public int level { get; set; }
        public int power { get; set; }
        public Corpo corpo { get; set; }
        public Efeito efeito { get; set; }
}

[Serializable]
public class Explosao {
        public Efeito efeito { get; set; }
        public Position position { get; set; }
}

[Serializable]
public class Efeito {
        public string id { get; set; }
        public int power { get; set; }
        public int level { get; set; }
        public int treasure { get; set; }
        public bool destruir { get; set; }
        public bool monsterOnly { get; set; }
        public bool playerOnly { get; set; }
}


