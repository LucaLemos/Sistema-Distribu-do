using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quobject.SocketIoClientDotNet.Client;

public class serverTest : MonoBehaviour
{
    private Socket socket;
    private GameObject playerPrefab;
    private int playerId;

    // Start is called before the first frame update
    void Start()
    {
        socket = IO.Socket("http://localhost:4080");

        socket.On("spaw", (data) =>
        {
            // Create a new player object with a unique ID
            GameObject newPlayer = Instantiate(playerPrefab);
        });
    }

    // Update is called once per frame
    void Update()
    {
        GameObject newPlayer = Instantiate(playerPrefab);
    }
}
