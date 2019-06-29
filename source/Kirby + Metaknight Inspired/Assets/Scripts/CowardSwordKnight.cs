using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowardSwordKnight : MonoBehaviour {

    public bool knockedout = false;
    public GameObject healthItems;
    public float waitTime;

    private Crashland crash;
    private GameObject falling;
    private Animator animator;
    private Vector3 spawnCheck;
    private GameObject player;
    private PlayerController pc;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        falling = GameObject.FindWithTag("Spawn");
        crash = falling.GetComponent<Crashland>();
        animator = GetComponent<Animator>();
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (crash.transform.position == spawnCheck && !knockedout)
        {
            player = GameObject.FindWithTag("Player");
            pc = player.GetComponent<PlayerController>();
            animator.Play("sword_coward_alert0");
            knockedout = true;
            pc.moveInputs = false;
            waitTime = 0.5f;
        }

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
        }

        if (knockedout && waitTime <= 0)
        {
            spriteRenderer.flipX = false;
            rb2d.velocity = Vector2.right;
            if (transform.position.x > 2.35)
            {
                pc.moveInputs = true;
                Destroy(gameObject);
            }
        }
    }
}
