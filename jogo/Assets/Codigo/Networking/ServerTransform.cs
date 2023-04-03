using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

[RequireComponent(typeof(ServerIdentity))]
public class ServerTransform : MonoBehaviour {

    [SerializeField]
    [GreyOut]
    private Vector3 lastPosition;
    private ServerIdentity serverIdentity;
    private Jogador jogador;

    private float updateTimer = 0f;
    private const float updateInterval = 0.05f;

    void Start() {
        serverIdentity = GetComponent<ServerIdentity>();
        lastPosition = transform.position;
        jogador = new Jogador();
        jogador.position = new Position();
        jogador.position.x = 0;
        jogador.position.y = 0;

        if(!serverIdentity.IsControlavel()) {
            enabled = false;
        }
    }

    void Update()
    {
        if(serverIdentity.IsControlavel()) {
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateInterval) {
                updateTimer -= updateInterval;
                if(lastPosition != transform.position) {
                    lastPosition = transform.position;
                    //Debug.Log("andei");
                    sendData();
                }
            }
        }
    }

    private void sendData() {
        jogador.position.x = Mathf.Round(transform.position.x * 1000.0f) / 1000.0f;
        jogador.position.y = Mathf.Round(transform.position.y * 1000.0f) / 1000.0f;

        //Debug.Log("1: " + JsonSerializer.Serialize(jogador.position));
        serverIdentity.GetSocket().Emit("mover", JsonSerializer.Serialize(jogador.position));
    }
}
