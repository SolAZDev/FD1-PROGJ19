using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : Actor {
    [Header ("===== Enemy Settings =====")]
    public MovementType Movement;
    public float Speed = 4f;
    public float ChargeSpeed = 6f;

    public bool CanJump = true;
    public float JumpHeight = 4;
    public Transform FloorDetector;

    [Header ("===== Detection Settings =====")]
    public float DetectDistance = 2f;
    public float ForgetDistance = 4f;
    public float MaxWalkDistace = 3f;
    public float ClosingDistance = .5f;
    public LayerMask FLayer;

    public enum MovementType {
        MoveLR,
        FlyAround,
        FlyAndCharge,
        HopAbout,
        HopTowardsPlayer,
        StandAndCharge,
        StandStill
    }

    Transform PlayerTransform;
    Player PlayerComponent;

    Vector2 mDir = Vector2.one;
    bool dead = false;
    bool grounded = false;
    bool jumpable = true;
    bool playerFound = false;
    bool canLookForSpot = true;
    float rotateY = 0; //A hack for rotation.
    void Start () {
        //mDir = transform.position;
        base.Start ();
        PlayerTransform = GameObject.FindWithTag ("Player").transform;
        PlayerComponent = PlayerTransform.GetComponent<Player> ();

    }

    // Update is called once per frame        
    void Update () {
        if (Health <= 0) { dead = true; StartCoroutine (Die ()); }
        if (!dead) {
            Move ();
        }

        //Same Dirty Trick
        anim.transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, (rotateY<transform.position.x ? 180 : rotateY> transform.position.x ? 0 : transform.rotation.eulerAngles.y), 0), Time.deltaTime * Speed * 4);
    }

    void LateUpdate () { }
    void OnEnable () {
        Health = MaxHealth;
        dead = false;
    }

    void Move () {
        if (Movement != MovementType.StandStill) {
            //Normal Movement
            if (Vector2.Distance (transform.position, PlayerTransform.position) > ForgetDistance) { playerFound = false; }
            if (Vector2.Distance (transform.position, PlayerTransform.position) < DetectDistance) { playerFound = true; }
            if (Movement != MovementType.StandAndCharge) {
                if (canLookForSpot) {
                    //mDir = FindPosition (MaxWalkDistace);
                    StartCoroutine (FindPosition ());
                }
                if (Vector2.Distance (transform.position, mDir) < ClosingDistance) {
                    StopCoroutine (FindPosition ());
                    canLookForSpot = true;
                }
            }

            switch (Movement) {
                case MovementType.FlyAround:
                    rigid.position = Vector2.MoveTowards (rigid.transform.position, mDir, Time.deltaTime * Speed);
                    rotateY = mDir.x;
                    break;
                case MovementType.FlyAndCharge:
                    rigid.position = Vector2.MoveTowards (rigid.transform.position, (playerFound?new Vector2 (PlayerTransform.position.x, PlayerTransform.position.y) : mDir), Time.deltaTime * (playerFound?ChargeSpeed : Speed));
                    rotateY = mDir.x;
                    break;
                case MovementType.HopAbout:
                    if (grounded && jumpable) {
                        float x = mDir.x > transform.position.x?1: (mDir.x < transform.position.x? - 1 : 0) * MaxWalkDistace;
                        rigid.AddForce (new Vector2 (x, JumpHeight), ForceMode2D.Impulse);
                        rotateY = x;
                        StartCoroutine (JumpWait ());
                    }
                    break;
                case MovementType.HopTowardsPlayer:
                    if (grounded) { // Trying to reimplement the wait for when not near the player
                        if (!playerFound && jumpable) {
                            float x = mDir.x > transform.position.x?1: (mDir.x < transform.position.x? - 1 : 0) * MaxWalkDistace;
                            rigid.AddForce (new Vector2 (x, JumpHeight), ForceMode2D.Impulse);
                            rotateY = x;
                            StartCoroutine (JumpWait ());
                        }
                        if (playerFound) {
                            float y = PlayerTransform.position.x > transform.position.x?1: (PlayerTransform.position.x < transform.position.x? - 1 : 0) * MaxWalkDistace;
                            rigid.AddForce (new Vector2 (y, JumpHeight), ForceMode2D.Impulse);
                            rotateY = y;
                        }
                    }
                    break;
                case MovementType.MoveLR:
                    if (CanJump && grounded && mDir.y > transform.position.y) {
                        rigid.AddForce (Vector2.up * JumpHeight, ForceMode2D.Impulse);
                    }
                    rigid.position = Vector2.MoveTowards (rigid.transform.position, new Vector2 ((playerFound?PlayerTransform.position.x : mDir.x), rigid.position.y), Time.deltaTime * (playerFound?ChargeSpeed : Speed));
                    rotateY = mDir.x;
                    break;
                case MovementType.StandAndCharge:
                    if (playerFound) {
                        if (CanJump && grounded && mDir.y > transform.position.y) {
                            rigid.AddForce (Vector2.up * JumpHeight, ForceMode2D.Impulse);
                        }
                        rigid.position = Vector2.MoveTowards (rigid.transform.position, new Vector2 (PlayerTransform.position.x, rigid.position.y), Time.deltaTime * (playerFound?ChargeSpeed : Speed));
                        rotateY = mDir.x;
                    }
                    break;
            }
        }
    }

    IEnumerator FindPosition (float maxDis = 3) {
        canLookForSpot = false;
        Vector3 pos = Random.insideUnitCircle * MaxWalkDistace;
        Vector3 npos = transform.position + pos;
        print ("Going to " + npos);
        mDir = npos;
        yield return new WaitForSeconds (Random.Range (5, 15));
        canLookForSpot = true;
    }

    IEnumerator JumpWait () {
        jumpable = false;
        yield return new WaitForSeconds (Random.Range (1, 10));
        jumpable = true;
    }
    IEnumerator Die () {
        foreach (Renderer mesh in Meshes) {
            //Might not work, look for  a fix.
            mesh.material.color = Color.Lerp (mesh.material.color, new Color (mesh.material.color.r, mesh.material.color.g, mesh.material.color.b, 0), Time.deltaTime);
        }
        yield return new WaitForSeconds (1);
        gameObject.SetActive (false);
    }

    void FixedUpdate () { }

    void OnCollisionEnter2D (Collision2D other) {
        if (other.transform.tag == "Ground") {
            if (CanJump || Movement == MovementType.HopAbout || Movement == MovementType.HopTowardsPlayer) {
                grounded = Physics2D.OverlapCircle (FloorDetector.position, 0.25f, FLayer);
            }
        }
    }

    void OnCollisionExit2D (Collision2D other) {
        if (other.transform.tag == "Ground") {
            grounded = false;
        }
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "PlayerAttack") {
            Health -= PlayerComponent.Damage;
            int dflash = PlayerComponent.Damage / 2;
            //Knockback Direction assuming the mDir is towards the player. Could be troublesome
            Vector2 knockDir = new Vector2 ((mDir.x < 0 ? -1 : (mDir.x > 0 ? 1 : 0)), (mDir.y < 0 ? -1 : (mDir.y > 0 ? 1 : 0)));
            //rigid.AddForce (-knockDir * PlayerComponent.Damage * 1.5f, ForceMode2D.Impulse); //Tring to make a knockback
            rigid.AddForce (-rigid.velocity * Speed);
            HurtFlash (dflash, 1 / dflash); // Higher Damage, Faster Flash
        }
    }

    private void OnDrawGizmos () {
        Gizmos.DrawWireSphere (transform.position, MaxWalkDistace);
        Gizmos.DrawWireSphere (transform.position, DetectDistance);
        Gizmos.DrawWireSphere (transform.position, ForgetDistance);
        Gizmos.DrawLine (transform.position, mDir);
    }
}