using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JogadorManager : MonoBehaviour {

    [Header("Dados")]
    [SerializeField]
    private float speed = 5;
    public Transform movePoint;
    public Vector3 posi;
    public LayerMask stopMovement;
    private Animator animator;

    [Header("Class")]
    [SerializeField]
    private ServerIdentity serverIdentity;

    void Start() {
        movePoint.parent = null;
        movePoint.name = this.name;
        posi = transform.position;
        animator = GetComponent<Animator>();
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);

        if(serverIdentity.IsControlavel()) {
            checkInput();
        }else {
            movePoint.position = posi;
            if(transform.position != movePoint.position) {
                animator.SetFloat("X", movePoint.position.x - transform.position.x);
                animator.SetFloat("Y",  movePoint.position.y - transform.position.y);
                animator.SetBool("Moving", true);
            }else {
                animator.SetBool("Moving", false);
            }
        }
    }

    private void checkInput() {
        if(Vector3.Distance(transform.position, movePoint.position) <= 0.05f){
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f ) {
                Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
                makeMove(move);
            }else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f ) {
                Vector3 move  = new Vector3(0, Input.GetAxisRaw("Vertical"), 0);
                makeMove(move);
            }else {
                animator.SetBool("Moving", false);
            }
        }
    }

    private void makeMove(Vector3 vec) {
        if(!Physics2D.OverlapCircle(movePoint.position + vec, 0.2f, stopMovement)) {
            animator.SetFloat("X", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("Y", Input.GetAxisRaw("Vertical"));
            animator.SetBool("Moving", true);
            movePoint.position += vec;
        }
    }
}
