using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAnim : MonoBehaviour
{

    public bool dropped = false;
    public int durability = 5;
    public float dropWait;
    public bool playerDrop = false;
    public bool playerPickup = false;
    public bool playerGet = false;
    public float pickupTime;
    public bool playerThrow = false;
    public bool playerThrown = false;
    public bool throwable;
    public float destoryTime = 5;
    public bool enemyThrow = false;
    public bool enemyThrown = false;
    public bool dead = false;

    private Rigidbody2D rb2d;
    private GameObject pc;
    private SpriteRenderer pc_sprite;
    private PlayerHealth player_hp;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D bc2d;
    private Crashland cutscene;
    private GameObject playerscene;
    private Vector3 spawnCheck;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        playerscene = GameObject.FindWithTag("Spawn");
        cutscene = playerscene.GetComponent<Crashland>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bc2d = GetComponent<BoxCollider2D>();
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (cutscene.transform.position == spawnCheck)
        {
            pc = GameObject.FindWithTag("Player");
            player_hp = pc.GetComponent<PlayerHealth>();
            pc_sprite = pc.GetComponent<SpriteRenderer>();
            if (!throwable)
            {
                DropAnimation();
                DropDurability();
                PickUp();
            }
            else if (throwable)
            {
                Thrown();
            }
        }
    }

    void Thrown()
    {
        if (playerThrow)
        {
            rb2d.gravityScale = 0;
            if (pc_sprite.flipX)
            {
                spriteRenderer.flipX = true;
                rb2d.velocity = Vector2.left * 2;
            }
            else if (!pc_sprite.flipX)
            {
                spriteRenderer.flipX = false;
                rb2d.velocity = Vector2.right * 2;
            }
            destoryTime = 5;
            playerThrow = false;
            playerThrown = true;
        }
        else if (enemyThrow)
        {
            rb2d.gravityScale = 0;
            if (spriteRenderer.flipX)
            {
                rb2d.velocity = Vector2.left * 2;
            }
            else if (!spriteRenderer.flipX)
            {
                rb2d.velocity = Vector2.right * 2;
            }
            destoryTime = 5;
            enemyThrow = false;
            enemyThrown = true;
        }

        if (destoryTime > 0)
        {
            destoryTime -= Time.deltaTime;
        }
        else if (destoryTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    bool ThrowHit(float distance)
    {
        Vector2 position = transform.position;
        Vector2 ledgecheck = position;
        ledgecheck.x += distance;
        Debug.DrawLine(position, ledgecheck, Color.blue);
        RaycastHit2D touch = Physics2D.Linecast(position, ledgecheck);
        if (touch.collider != null)
        {
            if (touch.collider.name == "Blade Knight")
            {
                touch.collider.gameObject.GetComponent<EnemyHealth>().health--;
                touch.collider.gameObject.GetComponent<EnemyHealth>().hit = true;
                animator.Play("fork_break");
                Destroy(gameObject, 0.583f);
                return true;
            }
            else if (touch.collider.name == "Pitchfork Soldier")
            {
                touch.collider.gameObject.GetComponent<PitchforkSoldier>().hp--;
                animator.Play("fork_break");
                Destroy(gameObject, 0.583f);
                return true;
            }
            else
            {
                animator.Play("fork_break");
                Destroy(gameObject, 0.583f);
                return false;
            }
        }
        return false;
    }

    void DropAnimation()
    {
        if (dropped)
        {
            animator.Play("wep_drop");
            rb2d.AddForce(Vector2.up * 125);
            dropped = false;
            dropWait = .3f;
        }
        else if (playerDrop)
        {
            animator.Play("wep_drop");

            SpriteRenderer pcSprite = pc.GetComponent<SpriteRenderer>();
            if (pcSprite.flipX)
            {
                spriteRenderer.flipX = true;
                rb2d.AddForce((Vector2.up * 110) + (Vector2.right * 45));
            }
            else if (!pcSprite.flipX)
            {
                spriteRenderer.flipX = false;
                rb2d.AddForce((Vector2.up * 110) + (Vector2.left * 45));
            }
            playerDrop = false;
            dropWait = .3f;
        }

        if (dropWait > -0.33f)
        {
            dropWait -= Time.deltaTime;
        }
        else if (dropWait <= -0.33f)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y * 0.5f);
        }
    }

    void DropDurability()
    {
        if (durability == 0)
        {
            bc2d.isTrigger = true;
            Destroy(gameObject, .56f);
        }
    }

    void PickUp()
    {
        PlayerController player = pc.GetComponent<PlayerController>();
        if (playerPickup)
        {
            animator.Play("wep_drop");
            bc2d.isTrigger = true;
            rb2d.AddForce(Vector2.up * 20);
            playerPickup = false;
            playerGet = true;
            pickupTime = 0.5f;
        }
        if (playerGet && pickupTime > 0.4f)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 0.03f);
            pickupTime -= Time.deltaTime;
        }
        if (playerGet && pickupTime > 0.2f && pickupTime <= 0.4f)
        {
            //transform.position = new Vector2(pc.transform.position.x, transform.position.y + 0.03f);
            if (pc.transform.position.x > transform.position.x)
            {
                transform.position = new Vector2(transform.position.x + 0.01f, transform.position.y + 0.02f);
            }
            else if (pc.transform.position.x < transform.position.x)
            {
                transform.position = new Vector2(transform.position.x - 0.01f, transform.position.y + 0.02f);
            }
            else if (pc.transform.position.x == transform.position.x)
            {
                transform.position = new Vector2(pc.transform.position.x, transform.position.y + 0.02f);
            }
            pickupTime -= Time.deltaTime;
        }
        else if (playerGet && pickupTime <= 0.2f && pickupTime > 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, -0.75f);
            pickupTime -= Time.deltaTime;
        }
        else if (playerGet && pickupTime <= 0)
        {
            playerGet = false;
            if (gameObject.name == "Sword" || gameObject.name == "Sword(Clone)")
            {
                player.weapon = 1;
            }
            if (gameObject.name == "Pitchfork" || gameObject.name == "Pitchfork(Clone)")
            {
                //player.throwable = 1;
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (throwable && !dead)
        {
            if (col.gameObject.name == "Blade Knight" && playerThrown)
            {
                col.gameObject.GetComponent<EnemyHealth>().health--;
                col.gameObject.GetComponent<EnemyHealth>().hit = true;
                animator.Play("fork_break");
                Destroy(gameObject, 0.583f);
                rb2d.velocity = Vector2.zero;
            }
            else if (col.gameObject.name == "Pitchfork Soldier" && playerThrown)
            {
                col.gameObject.GetComponent<PitchforkSoldier>().hp--;
                animator.Play("fork_break");
                Destroy(gameObject, 0.583f);
                rb2d.velocity = Vector2.zero;
            }
            else if (col.gameObject.name == "Sword Knight" && playerThrown)
            {
                col.gameObject.GetComponent<SwordKnightBoss>().deflect = true;
                throwable = false;
                playerPickup = true;
            }
            else if ((col.gameObject.tag == "Player" || col.gameObject.name == "Player(Clone)") && enemyThrown)
            {
                animator.Play("fork_break");
                dead = true;
                Destroy(gameObject, 0.583f);
                rb2d.velocity = Vector2.zero;
                DamagePlayer();
            }
            else if (col.gameObject.tag == "Floor")
            {
                animator.Play("fork_break");
                Destroy(gameObject);
            }
            else if ((col.gameObject.name == "Pitchfork" || col.gameObject.name == "Pitchfork(Clone)") && (enemyThrown || playerThrown))
            {
                animator.Play("fork_break");
                dead = true;
                Destroy(gameObject, 0.583f);
                rb2d.velocity = Vector2.zero;
            }
        }
    }

    void DamagePlayer()
    {
        PlayerController pcController = pc.GetComponent<PlayerController>();
        player_hp.hp--;
        player_hp.gotHit = true;
        if (pc.transform.position.x < transform.position.x)
        {
            player_hp.hitFromRight = true;
            player_hp.hitFromLeft = false;
        }
        else if (pc.transform.position.x > transform.position.x)
        {
            player_hp.hitFromLeft = true;
            player_hp.hitFromRight = false;
        }
        pcController.moveInputs = false;
        pcController.isGrounded = false;
        pcController.hitTime = .5f;
        pcController.hitAcceleration = 2f;
    }
}
