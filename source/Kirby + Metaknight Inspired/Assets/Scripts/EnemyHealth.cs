using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public int health = 2;
    public bool hit = false;
    public bool knockback = false;
    public float knockbackTime;
    public float movementCooldown;
    public bool dead = false;
    public GameObject weapon;

    private EnemyController controller;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D bc2d;
    private GameObject player;
    private Crashland cutscene;
    private GameObject playerscene;
    private Vector3 spawnCheck;

    // Use this for initialization
    void Start () {
        controller = GetComponent<EnemyController>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        bc2d = GetComponent<BoxCollider2D>();
        playerscene = GameObject.FindWithTag("Spawn");
        cutscene = playerscene.GetComponent<Crashland>();
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);

    }
	
	// Update is called once per frame
	void Update () {
        if (cutscene.transform.position == spawnCheck)
        {
            player = GameObject.FindWithTag("Player");
        }
        HealthCheck();
        KnockbackPause();
	}

    void HealthCheck()
    {
        if (health == 0)
        {
            Death();
        }
        else if (hit && health > 0)
        {
            hit = false;
            knockback = true;
            knockbackTime = .5f;
            movementCooldown = 1.33f;
            controller.movementCooldown = movementCooldown;
            controller.attackCooldown = 3;
            animator.Play("blade_damaged0");
            rb2d.velocity = Vector2.zero;
            if (player.transform.position.x < transform.position.x)
            {
                rb2d.AddForce((Vector2.right * 72) + (Vector2.up * 115));
            }
            else if (player.transform.position.x >= transform.position.x)
            {
                rb2d.AddForce((Vector2.left * 72) + (Vector2.up * 115));
            }
        }
    }

    void Death()
    {
        controller.attackCooldown = 3;
        animator.Play("blade_death1");
        bc2d.isTrigger = true;
        rb2d.gravityScale = 0;
        rb2d.velocity = Vector2.zero;
        if (spriteRenderer.flipX)
        {
            rb2d.AddForce((Vector2.right * 56) + (Vector2.down * 5));
        }
        else if (!spriteRenderer.flipX)
        {
            rb2d.AddForce((Vector2.left * 56) + (Vector2.down * 5));
        }
        GameObject spawn = Instantiate(weapon, transform.position, transform.rotation);
        //Animator spawnAnim = spawn.GetComponent<Animator>();
        //spawnAnim.Play("wep_drop");
        DropAnim drop = spawn.GetComponent<DropAnim>();
        drop.dropped = true;
        health = -1;
        weapon.transform.parent = null;
        Destroy(controller.gameObject, 0.55f);

        
    }

    void KnockbackPause()
    {
        if (knockback && knockbackTime > 0)
        {
            knockbackTime -= Time.deltaTime;
        }
        else if (knockback && knockbackTime <= 0)
        {
            knockbackTime = 0;
            knockback = false;
            rb2d.velocity = new Vector2(0, 0);
            controller.moveTime = .3f;
        }

        if (movementCooldown > 0)
        {
            movementCooldown -= Time.deltaTime;
        }
    }
}
