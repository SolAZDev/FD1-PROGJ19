using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class Player : Actor {

    [Header ("===== Player Settings ======")]
    public int MP = 10;
    public int MaxMP = 10;
    public int JumpingCount = 1;
    public int MaxJumpCount = 1;
    public float JumpHeight = 10;
    public float Speed = 4;

    [Header ("===== Controls Settings ======")]
    public Controls control;

    [Header ("==== Other Settings =====")]
    public int JumpCount = 1;
    public Transform FloorDetector;
    public LayerMask groundedLayers;

    Vector2 mDir = Vector2.zero;
    Vector2 floorDectector;
    public bool grounded = false;

    // Start is called before the first frame update

    void Awake () {
        control.Default.Jump.performed += j => Jump ();
        control.Default.Movement.performed += mv => JoyDir (mv.ReadValue<Vector2> ());
        control.Default.Movement.cancelled += mv => JoyDir (Vector2.zero);
        control.Default.Attack.performed += a => Attack ();
    }

    void OnEnable () {
        control.Enable ();
        Keyboard kb = InputSystem.GetDevice<Keyboard> ();
    }

    // Update is called once per frame
    void Update () {
        //rigid.velocity = new Vector2 (mDir.x * Speed, rigid.velocity.y);
        if (rigid != null) {
            //rigid.position = Vector2.Lerp (rigid.position, rigid.position + new Vector2 (mDir.x, 0), Time.deltaTime * Speed);
            rigid.velocity = new Vector2 (mDir.x * Speed, rigid.velocity.y);
            if (rigid.velocity.y < 0) {
                rigid.velocity += Vector2.up * Physics2D.gravity * (2.5f - 1) * Time.deltaTime;
            } else if (rigid.velocity.y > 0 && control.Default.Jump.phase == InputActionPhase.Performed) {
                rigid.velocity += Vector2.up * Physics2D.gravity * (2f - 1) * Time.deltaTime;
            }
        }

        //Quick and Dirty... But really just nasty
        anim.transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, (mDir.x<0 ? 180 : mDir.x> 0 ? 0 : transform.rotation.eulerAngles.y), 0), Time.deltaTime * Speed * 4);

        floorDectector = new Vector2 (transform.position.x, transform.position.y); //+ (-col2d.bounds.extents.y / 2));
        Debug.DrawRay (floorDectector, -Vector3.up, Color.red);
    }

    void FixedUpdate () {
        if (Physics2D.OverlapCircle (FloorDetector.position, 0.25f, groundedLayers)) {
            EnableJump ();
        }

    }
    void OnCollisionEnter2D (Collision2D other) {
        if (other.transform.tag == "Enemy") {
            EnemyAI enemy = other.transform.GetComponent<EnemyAI> ();
            Health -= enemy.Damage;
            int dflash = enemy.Damage / 2;
            //Knockback Direction assuming the mDir is towards the player. Could be troublesome
            Vector2 knockDir = new Vector2 ((mDir.x < 0 ? -1 : (mDir.x > 0 ? 1 : 0)), 1);
            //rigid.AddForce (-knockDir * enemy.Damage * 1.5f, ForceMode2D.Impulse); //Tring to make a knockback
            rigid.AddForce (-rigid.velocity * Speed);
            HurtFlash (dflash, .1f / dflash); // Higher Damage, Faster Flash
        }
    }
    void OnCollisionExit2D (Collision2D other) {
        if (other.transform.tag == "Ground") { grounded = false; }
    }
    void OnTriggerEnter2D (Collider2D other) {

    }
    public void Attack () { }
    public void JoyDir (Vector2 dir) {
        mDir = dir;
    }
    public void Jump () {
        print ("Jump Pressed");
        if (grounded || JumpCount > 0) { //if grounded
            //JumpCount -= 1;
            rigid.AddForce (Vector2.up * JumpHeight * 1, ForceMode2D.Impulse);
        }
    }

    void EnableJump () {
        JumpCount = MaxJumpCount;
        grounded = true;
    }
    void OnDrawGizmos () {
        //  Gizmos.DrawCube (new Vector3 (floorDectector.x, floorDectector.y, 0), Vector3.one);
    }
}