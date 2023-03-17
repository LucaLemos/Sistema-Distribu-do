using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JogadorManager : MonoBehaviour {

    [Header("Dados")]
    [SerializeField]
    private float speed = 4;

    [Header("Class")]
    [SerializeField]
    private ServerIdentity serverIdentity;

    public Vector3 posi;

    void Start() {
         posi = transform.position;
    }

    void Update() {
        if(serverIdentity.IsControlavel()) {
            checkMovimento();
        }else {
            transform.position = posi;
        }
    }

    private void checkMovimento() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //serverIdentity.UpdatePosition(new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime);
        transform.position += new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;
    }
}
