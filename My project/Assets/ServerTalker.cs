using WebSocketSharp;
using UnityEngine;

public class ServerTalker : MonoBehaviour
{
    WebSocket ws;
    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket("ws://localhost:4080");
        ws.OnMessage += (sender, e) => {
            Debug.Log("Mensagem recebida do " + ((WebSocket)sender).Url + ", Data: " + e.Data);
        };

        ws.Connect();

    }

    // Update is called once per frame
    void Update()
    {
        if(ws == null) {
            return;
        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            ws.Send("Funciona!!");
        }
        
    }
}