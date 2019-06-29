using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwordKnightBoss : MonoBehaviour {

    public float cutsceneTime = 5;
    public int hp = 10;
    public bool fight = false;
    public bool changePhase = false;
    public bool leftEdge;
    public bool rightEdge;
    public bool ceiling;
    public float moveTime;
    public bool damaged;
    public float damageTime;
    public int attackPattern;
    public bool attack;
    public float attackCooldown;
    public float attacking;
    public bool jump;
    public float jumpCooldown;
    public bool deflect;
    public float deflectTime;
    public bool attackAnim;
    public bool invincible;

    private GameObject player;
    private PlayerController pc;
    private PlayerHealth player_hp;
    private Rigidbody2D rb2d;
    private BoxCollider2D bc2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        player_hp = player.GetComponent<PlayerHealth>();
        rb2d = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        SaveLoad.Save();
	}
	
	// Update is called once per frame
	void Update () {
        Cutscene();
        if (damageTime <= 0 && hp > 0)
        {
            Deflect();
            PlayerTouch();
            PhaseOne();
            PhaseTwo();
            Look();
        }
        if (hp > 0)
        {
            TakeDamage();
        }
        Death();
	}

    void Deflect()
    {
        if (deflect)
        {
            if (!attackAnim && !damaged && jumpCooldown <= 0)
            {
                animator.Play("sword_deflect0");
                deflectTime = 0.1f;
            }
            else if (jumpCooldown > 1)
            {
                animator.Play("sword_jump0");
                deflectTime = 0.1f;
            }
            else
            {
                deflect = false;
            }
        }

        if (deflectTime > 0)
        {
            deflectTime -= Time.deltaTime;
            deflect = false;
        }
    }

    void Look()
    {
        if ((rb2d.velocity.x > 0 || (rb2d.velocity.x == 0 && player.transform.position.x > transform.position.x)) && attackCooldown <= 0 && !damaged)
        {
            if (spriteRenderer.flipX)
            {
                bc2d.offset = new Vector2(-bc2d.offset.x, bc2d.offset.y);
            }
            spriteRenderer.flipX = false;
        }
        else if ((rb2d.velocity.x < 0 || (rb2d.velocity.x == 0 && player.transform.position.x < transform.position.x)) && attackCooldown <= 0 && !damaged)
        {
            if (!spriteRenderer.flipX)
            {
                bc2d.offset = new Vector2(-bc2d.offset.x, bc2d.offset.y);
            }
            spriteRenderer.flipX = true;
        }

        if (rb2d.velocity == Vector2.zero && jumpCooldown <= 0 && attackCooldown <= 0 && cutsceneTime <= 0 && deflectTime <= 0)
        {
            animator.Play("sword_idle0");
        } 
    }

    void Cutscene()
    {
        if (cutsceneTime > 0)
        {
            cutsceneTime -= Time.deltaTime;
            fight = false;
            pc.moveInputs = false;
            if (hp == 10)
            {
                StartScene();
            }
            /*
            else if (hp == 5)
            {
                HalfWayScene();
            }
            */
            else if (hp == 0)
            {
                EndScene();
            }
        }
        else if (cutsceneTime <= 0 && hp > 0)
        {
            fight = true;
            pc.moveInputs = true;
        }
    }

    void StartScene()
    {
        if (cutsceneTime > 2)
        {
            pc.GetComponent<Animator>().Play("player_move0");
            pc.GetComponent<Rigidbody2D>().velocity = Vector2.right;
        }
        if (cutsceneTime > 1.1f)
        {
            animator.Play("sword_walk0");
            rb2d.velocity = Vector2.left;
        }
        else if (cutsceneTime <= 1 && cutsceneTime > 0.9f)
        {
            animator.Play("sword_up0");
        }
        else if (cutsceneTime <= 0)
        {
            animator.Play("sword_idle0");
            GameObject outsideWalls = GameObject.FindWithTag("Wall");
            outsideWalls.GetComponent<PolygonCollider2D>().isTrigger = false;
        }
    }

    void HalfWayScene()
    {

    }

    void EndScene()
    {
        if (cutsceneTime > 5.5f)
        {
            animator.Play("sword_die0");
            bc2d.isTrigger = false;
            rb2d.velocity = Vector2.down;
            pc.GetComponent<Animator>().Play("player_receive");
        }
        else if (cutsceneTime > 3 && cutsceneTime < 3.5f)
        {
            pc.GetComponent<Animator>().Play("player_boost1");
            pc.GetComponent<Rigidbody2D>().velocity = Vector2.up * 2;
            bc2d.isTrigger = true;
            rb2d.velocity = Vector2.zero;
        }
        else if (cutsceneTime < 2.5f && cutsceneTime > 0)
        {
            pc.GetComponent<Animator>().Play("player_takeoff");
            pc.GetComponent<Rigidbody2D>().gravityScale = 0;
            pc.GetComponent<Rigidbody2D>().velocity = Vector2.up;
            rb2d.velocity = Vector2.zero;
        }
        else if (cutsceneTime <= 0)
        {
            SceneManager.LoadScene("credits");
        }
    }

    void PhaseOne()
    {
        if (fight && hp > 0)
        {
            FightNormal();
        }
    }

    void PhaseTwo()
    {
        if (fight && changePhase && hp < 5)
        {

        }
        else if (fight && changePhase && hp <= 0)
        {
            hp = 0;
            fight = false;
            changePhase = false;
            cutsceneTime = 6;
        }
    }

    void FightNormal()
    {
        if (moveTime <= 0 && !attack)
        {
            moveTime = 2.5f;
            attackPattern = Random.Range(1, 9);
        }
        else if (moveTime > 1)
        {
            moveTime -= Time.deltaTime;

            if (attackPattern >= 1 && attackPattern <= 3)
            {
                FrontAttack();
            }
            else if (attackPattern >= 4 && attackPattern <= 7)
            {
                DashAttack();
            }
            else if (attackPattern >= 8)
            {
                Kick();
            }
        }
        else if (moveTime > 0)
        {
            moveTime -= Time.deltaTime;
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (attackCooldown <= 0.5f && jump)
        {
            jumpCooldown = 2;
            jump = false;
            attackAnim = false;
        }

        if (attacking > 0)
        {
            attacking -= Time.deltaTime;
            attack = true;
        }
        else if (attacking <= 0)
        {
            attack = false;
        }

        Jump();
        if(NormalAttack() && attackCooldown > 0)
        {
            //rb2d.velocity = Vector2.zero;
        }
    }

    void Jump()
    {
        if (jumpCooldown > 0 && attackCooldown <= 0)
        {
            if (jumpCooldown >= 2)
            {
                EdgeCheck();
                animator.Play("sword_jumpaway");
            }
            jumpCooldown -= Time.deltaTime;
            if ((player.transform.position.x > transform.position.x && !leftEdge) || rightEdge)
            {
                if (jumpCooldown > 1.9f)
                {
                    rb2d.AddForce((Vector2.up * 50) + (Vector2.left * 12));
                }

            }
            else if ((player.transform.position.x < transform.position.x && !rightEdge) || leftEdge)
            {
                if (jumpCooldown > 1.9f)
                {
                    rb2d.AddForce((Vector2.up * 50) + (Vector2.right * 12));
                }

            }
        }
    }

    void FrontAttack()
    {
        if (FindPlayer() && attackCooldown <= 0 && jumpCooldown <= 0)
        {
            attackAnim = true;
            animator.Play("sword_readyattack");
            rb2d.velocity = Vector2.zero;
            attackCooldown = 2;
            attacking = 1;
            jump = true;
            attack = true;
        }

        if (player.transform.position.x > transform.position.x && attackCooldown <= 0 && jumpCooldown <= 0 && deflectTime <= 0)
        {
            attackAnim = false;
            animator.Play("sword_run0");
            rb2d.velocity = Vector2.right * 1.33f;
        }
        else if (player.transform.position.x < transform.position.x && attackCooldown <= 0 && jumpCooldown <= 0 && deflectTime <= 0)
        {
            attackAnim = false;
            animator.Play("sword_run0");
            rb2d.velocity = Vector2.left * 1.33f;
        }
    }

    void DashAttack()
    {
        if (attackCooldown <= 0 && jumpCooldown <= 0)
        {
            attackAnim = true;
            animator.Play("sword_dash0");
            attackCooldown = 1.25f;
            attacking = 0.5f;
            jump = true;
            attack = true;
            if (player.transform.position.x > transform.position.x)
            {
                rb2d.AddForce(Vector2.right * 160);
            }
            else if (player.transform.position.x < transform.position.x)
            {
                rb2d.AddForce(Vector2.left * 160);
            }
        }
    }

    void Kick()
    {
        if (attackCooldown <= 0 && jumpCooldown <= 0)
        {
            attackAnim = true;
            animator.Play("sword_kick0");
            rb2d.velocity = Vector2.zero;
            attackCooldown = 1;
            attacking = 0.75f;
            jump = true;
            attack = true;
            if (player.transform.position.x > transform.position.x)
            {
                rb2d.AddForce(Vector2.right * 120);
            }
            else if (player.transform.position.x < transform.position.x)
            {
                rb2d.AddForce(Vector2.left * 120);
            }
        }
    }

    bool NormalAttack()
    {
        if (attackPattern <= 3 && attackCooldown < 1.72f && attackCooldown > 1.56f)
        {
            invincible = true;
            return Slash(0.5f, 0.055f) || Slash(0.5f, 0) || Slash(0.5f, -0.055f);
        }
        else if (attackPattern <= 3 && attackCooldown < 1.36f && attackCooldown > 1.24f)
        {
            invincible = true;
            return Slash(0.5f, 0.055f) || Slash(0.5f, 0) || Slash(0.5f, -0.055f);
        }
        else if (attackPattern >= 4 && attackPattern <= 7 && attackCooldown > 0.5f)
        {
            invincible = true;
            return Slash(0.47f, 0);
        }
        else
        {
            invincible = false;
        }
        return false;
    }

    bool Slash(float range, float height)
    {
        Vector3 position = transform.position;
        Vector3 sword = position;
        if (spriteRenderer.flipX)
        {
            sword.x -= range;
        }
        else if (!spriteRenderer.flipX)
        {
            sword.x += range;
        }
        RaycastHit2D hit = Physics2D.Linecast(position, sword);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Player" && player_hp.inviniciblitiy <= 0)
            {
                DamagePlayer();
                return true;
            }
        }
        return false;
    }

    void TakeDamage()
    {
        if (damaged && damageTime <= 0 && !invincible)
        {
            damageTime = 2;
            hp--;
            damaged = false;
            moveTime = 0;
            jumpCooldown = 0;
            attacking = 0;
            attackCooldown = 0;
            attack = false;
            rb2d.velocity = Vector2.zero;
            if (player.transform.position.x > transform.position.x)
            {
                rb2d.AddForce(Vector2.left * 125);
            }
            else if (player.transform.position.x < transform.position.x)
            {
                rb2d.AddForce(Vector2.right * 125);
            }
        }

        if (damageTime > 0)
        {
            if (damageTime > 1)
            {
                animator.Play("sword_gothit0");
                EdgeCheck();
            }
            else if (damageTime < 1 && damageTime > 0.9f)
            {
                animator.Play("sword_hitjump0");
                if ((player.transform.position.x > transform.position.x && !leftEdge) || rightEdge)
                {
                    rb2d.AddForce((Vector2.up * 36) + (Vector2.left * 16));
                }
                else if ((player.transform.position.x < transform.position.x && !rightEdge) || leftEdge)
                {
                    rb2d.AddForce((Vector2.up * 36) + (Vector2.right * 16));
                }
            }
            damageTime -= Time.deltaTime;
        }
    }

    void Death()
    {
        if (hp <= 0 && cutsceneTime <= 0)
        {
            hp = 0;
            cutsceneTime = 6;
            rb2d.velocity = Vector2.zero;
            rb2d.gravityScale = 0;
            bc2d.isTrigger = true;
            GameObject outsideWalls = GameObject.FindWithTag("Wall");
            outsideWalls.GetComponent<PolygonCollider2D>().isTrigger = true;
        }
    }

    void PlayerTouch()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, 0.15f);
        foreach (Collider2D player in players)
        {
            if (player.gameObject.tag == "Player" && player_hp.inviniciblitiy <= 0)
            {
                DamagePlayer();
            }
            else if(player.gameObject.tag == "Item")
            {
                player.GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }
    }

    void DamagePlayer()
    {
        player_hp.inviniciblitiy = 1;
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
        if (player_hp.hitFromLeft && transform.position.y > (pc.transform.position.y + 0.1f))
        {
            pc.transform.position = new Vector3(pc.transform.position.x + 0.2f, pc.transform.position.y);
        }
        else if (player_hp.hitFromRight && transform.position.y > (pc.transform.position.y + 0.1f))
        {
            pc.transform.position = new Vector3(pc.transform.position.x - 0.2f, pc.transform.position.y);
        }
        pc.moveInputs = false;
        pc.isGrounded = false;
        pc.hitTime = .5f;
        pc.hitAcceleration = 2f;
    }

    bool FindPlayer()
    {
        Vector2 position = transform.position;
        Vector2 front = position;
        if (spriteRenderer.flipX)
        {
            front.x -= 0.3f;
        }
        else if (!spriteRenderer.flipX)
        {
            front.x += 0.3f;
        }

        Debug.DrawLine(position, front, Color.red);
        RaycastHit2D touch = Physics2D.Linecast(position, front);
        if (touch.collider != null)
        {
            if (touch.collider.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    void EdgeCheck()
    {
        if (transform.position.x < 0)
        {
            leftEdge = true;
        }
        else
        {
            leftEdge = false;
        }
        if (transform.position.x > 2)
        {
            rightEdge = true;
        }
        else
        {
            rightEdge = false;
        }
        if (transform.position.y > 1.5f)
        {
            ceiling = true;
        }
        else
        {
            ceiling = false;
        }
    }
}
