using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchforkSoldier : MonoBehaviour {

    public int hp;
    public float attackDelay;
    public bool spotted;
    public bool throwFork;
    public GameObject weapon;
    public float sight = 3.25f;
    public bool overrideSight = false;
    public bool startup;

    private BoxCollider2D bc2d;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private GameObject player;
    private GameObject cutscene;
    private Crashland crashed;
    private Vector3 spawnCheck;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        bc2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cutscene = GameObject.FindWithTag("Spawn");
        crashed = cutscene.GetComponent<Crashland>();
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);
        hp = 1;
        spotted = false;
        throwFork = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (crashed.transform.position == spawnCheck)
        {
            startup = true;
        }
        if (startup)
        {
            player = GameObject.FindWithTag("Player");
            HealthCheck();
            AttackPlayer();
        }
	}

    void AttackPlayer()
    {
        if (!spotted)
        {
            spotted = SpotPlayer(sight) || SpotPlayer(-sight) || overrideSight;
        }
        else if (spotted && Mathf.Abs(player.transform.position.x - transform.position.x) > (sight * 1.5f) && !overrideSight)
        {
            spotted = false;
        }
        else if (spotted && attackDelay <= 0 && throwFork == false)
        {
            throwFork = true;
            attackDelay = 2.5f;
            animator.Play("fork_throw");
        }

        if (player.transform.position.x < transform.position.x && spotted && attackDelay <= 0.5f)
        {
            spriteRenderer.flipX = true;
        }
        else if (player.transform.position.x > transform.position.x && spotted && attackDelay <= 0.5f)
        {
            spriteRenderer.flipX = false;
        }

        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
            if (attackDelay <= 1.5f && throwFork)
            {
                throwFork = false;
                Vector3 position = transform.position;
                GameObject thrown = null;
                if (spriteRenderer.flipX)
                {
                    position.x -= 0.28f;
                    thrown = Instantiate(weapon, position, transform.rotation);
                    thrown.GetComponent<SpriteRenderer>().flipX = true;
                }
                else if (!spriteRenderer.flipX)
                {
                    position.x += 0.28f;
                    thrown = Instantiate(weapon, position, transform.rotation);
                    thrown.GetComponent<SpriteRenderer>().flipX = false;
                }
                thrown.GetComponent<DropAnim>().enemyThrow = true;
                thrown.GetComponent<DropAnim>().throwable = true;
                thrown.GetComponent<BoxCollider2D>().isTrigger = true;
            }
            if (attackDelay <= 0.5f && attackDelay > 0)
            {
                animator.Play("fork_idle");
            }
        }
        else if (attackDelay <= 0)
        {
            throwFork = false;
        }
    }

    bool SpotPlayer(float distance)
    {
        Vector2 position = transform.position;
        position.y += 0.05f;
        Vector2 sight = position;
        sight.x += distance;
        Debug.DrawLine(position, sight, Color.yellow);
        RaycastHit2D vision = Physics2D.Linecast(position, sight);
        if (vision.collider != null)
        {
            if (vision.collider.name == "Player" || vision.collider.name == "Player(Clone)")
            {
                return true;
            }
        }
        return false;
    }

    void HealthCheck()
    {
        if (hp == 0)
        {
            animator.Play("fork_death");
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
            hp = -1;
            weapon.transform.parent = null;
            Destroy(gameObject, 0.5f);
        }
    }
}
