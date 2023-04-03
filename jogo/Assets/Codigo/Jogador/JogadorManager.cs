using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using TMPro;
using UnityEngine;

public class JogadorManager : MonoBehaviour {

    [Header("Dados")]
    [SerializeField]
    public int level;
    public int power;
    private float speed = 5;
    //public Transform movePoint;
    public Vector3 posi;
    public Vector3 posiAndar;
    public LayerMask stopMovement;
    private Animator animator;

    [Header("Class")]
    [SerializeField]
    private ServerIdentity serverIdentity;

    void Start() {
        //movePoint.parent = null;
        //movePoint.name = this.name;
        posi = transform.position;
        posiAndar = transform.position;
        animator = GetComponent<Animator>();
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, posiAndar, speed * Time.deltaTime);

        if(serverIdentity.IsControlavel()) {
            checkInput();
        }else {
            if(transform.position != posiAndar) {
                animator.SetFloat("X", posiAndar.x - transform.position.x);
                animator.SetFloat("Y",  posiAndar.y - transform.position.y);
                animator.SetBool("Moving", true);
            }else {
                animator.SetBool("Moving", false);
            }
        }
    }

    private void checkInput() {
        if(Vector3.Distance(transform.position, posiAndar) <= 0.05f){
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f ) {
                animator.SetFloat("X", Input.GetAxisRaw("Horizontal"));
                animator.SetFloat("Y", Input.GetAxisRaw("Vertical"));
                Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
                makeMove(move);
            }else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f ) {
                animator.SetFloat("X", Input.GetAxisRaw("Horizontal"));
                animator.SetFloat("Y", Input.GetAxisRaw("Vertical"));
                Vector3 move  = new Vector3(0, Input.GetAxisRaw("Vertical"), 0);
                makeMove(move);
            }else {
                animator.SetBool("Moving", false);
            }
        }
    }

    private void makeMove(Vector3 vec) {
        if(!Physics2D.OverlapCircle(posiAndar + vec, 0.2f, stopMovement)) {
            animator.SetBool("Moving", true);
            posiAndar += vec;

            Position andando = new Position();
            andando.x = posiAndar.x;
            andando.y = posiAndar.y;
            serverIdentity.GetSocket().Emit("mover", JsonSerializer.Serialize(andando));
        }
    }

    public void atualizaNome() {
        GameObject childObject = transform.Find("Nome").gameObject;
        TMP_Text textMash = childObject.GetComponent<TMP_Text>();
        textMash.text = "LV:" + level + " Pw:" + power;
    }
}
