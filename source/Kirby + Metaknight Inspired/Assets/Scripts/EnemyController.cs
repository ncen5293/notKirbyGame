using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public bool spottedPlayer;
    public Vector2 move;
    public bool idle;
    public float movementCooldown = 0f;
    public float moveTime = 0f;
    public float vision;
    public float dirVision;
    public LayerMask groundLayer;
    public bool normalAttack;
    public float attackCooldown;
    public float attackDelay;
    public bool leftWallTouch;
    public bool rightWallTouch;
    public bool leftGroundTouch;
    public bool rightGroundTouch;
    public float dodgeAfter;
    public Vector2 spawnPosition;
    public bool startup;

    private GameObject pc;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D bc2d;
    private EnemyHealth hp;
    private Crashland cutscene;
    private GameObject playerscene;
    private Vector3 spawnCheck;

    // Use this for initialization
    void Start () {
        playerscene = GameObject.FindWithTag("Spawn");
        cutscene = playerscene.GetComponent<Crashland>();
        //pc = GameObject.FindWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        hp = GetComponent<EnemyHealth>();
        spottedPlayer = false;
        idle = false;
        spawnPosition = transform.position;
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (cutscene.transform.position == spawnCheck)
        {
            startup = true;
        }
        if (startup)
        {
            pc = GameObject.FindWithTag("Player");
            FindPlayer();
        }
        if (hp.health > 0)
        {
            Movement();
            Attack();
            RaycastCheck();
            ColliderCheck();
        }
	}

    void RaycastCheck()
    {
        leftWallTouch = WallCheck(-0.36f);
        rightWallTouch = WallCheck(0.36f);
        leftGroundTouch = GroundCheck(-0.36f);
        rightGroundTouch = GroundCheck(0.36f);
    }

    void ColliderCheck()
    {
        float bc2doffset = transform.position.x + bc2d.offset.x;
        if (spriteRenderer.flipX )
        {
            if (bc2doffset < transform.position.x)
            {
                bc2d.offset = new Vector2(bc2d.offset.x + .1f, bc2d.offset.y);
            }

        }
        else if (!spriteRenderer.flipX)
        {
            if (bc2doffset > transform.position.x)
            {
                bc2d.offset = new Vector2(bc2d.offset.x - .1f, bc2d.offset.y);
            }

        }

    }

    void FindPlayer()
    {
        vision = Mathf.Abs(Mathf.Abs(pc.transform.position.x) - Mathf.Abs(transform.position.x));
        dirVision = Mathf.Abs(pc.transform.position.x) - Mathf.Abs(transform.position.x);
        if (!spottedPlayer)
        {
            spottedPlayer = SpotPlayer(-2.5f) || SpotPlayer(2.5f);
            if (spottedPlayer)
            {
                animator.Play("blade_spotted0");
                movementCooldown = 0;
            }
        }
        if (vision > 5)
        {
            //transform.position = spawnPosition;
        }
        if (pc.transform.position.x < transform.position.x && spottedPlayer && attackDelay <= 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (pc.transform.position.x > transform.position.x && spottedPlayer && attackDelay <= 0)
        {
            spriteRenderer.flipX = false;
        }

        if (spottedPlayer && vision > 3.6f)
        {
            spottedPlayer = false;
        }
    }

    void Movement()
    {
        if (!spottedPlayer && movementCooldown > 0)
        {
            if ((move.x < 0 && (leftWallTouch || !leftGroundTouch)) || (move.x > 0 && (rightWallTouch || !rightGroundTouch)))
            {
                move = Vector2.zero;
            }
        }
        else if (!spottedPlayer && movementCooldown <= 0)
        {
            int moving;
            moving = Random.Range(1, 30);

            if (moving <= 10)
            {
                idle = true;
                movementCooldown = 3f;
            }
            else if (moving > 10 && moving <= 20 && !leftWallTouch && leftGroundTouch)
            {
                idle = false;
                spriteRenderer.flipX = true;
                move = Vector2.left;
                movementCooldown = 3f;
                moveTime = .85f;
            }
            else if (moving > 20 && !rightWallTouch && rightGroundTouch)
            {
                idle = false;
                spriteRenderer.flipX = false;
                move = Vector2.right;
                movementCooldown = 3f;
                moveTime = .85f;
            }
            else
                move = Vector2.zero;
        }
        else if (spottedPlayer && vision > .47f && movementCooldown <= 0)
        {
            if ((leftWallTouch || !leftGroundTouch) && !rightWallTouch && rightGroundTouch)
            {
                move = Vector2.right;
                movementCooldown = .5f;
                moveTime = movementCooldown;
            }
            else if ((rightWallTouch || !rightGroundTouch) && !leftWallTouch && leftGroundTouch)
            {
                move = Vector2.left;
                movementCooldown = .5f;
                moveTime = movementCooldown;
            }
            else if (!leftWallTouch && leftGroundTouch)
            {
                if (spriteRenderer.flipX || (!spriteRenderer.flipX && attackCooldown > 0))
                {
                    move = Vector2.left;
                }
                else if (!spriteRenderer.flipX || (spriteRenderer.flipX && attackCooldown > 0))
                {
                    move = Vector2.right;
                }
            }
            else if (!rightWallTouch && rightGroundTouch)
            {
                if (spriteRenderer.flipX || (!spriteRenderer.flipX && attackCooldown > 0))
                {
                    move = Vector2.left;
                }
                else if (!spriteRenderer.flipX || (spriteRenderer.flipX && attackCooldown > 0))
                {
                    move = Vector2.right;
                }
            }

        }
        else if (spottedPlayer && vision <= .47f && hp.movementCooldown <= 0)
        {
            if (attackCooldown <= 0)
            {
                move = Vector2.zero;
                normalAttack = true;
                animator.Play("blade_attack0");
                movementCooldown = .91f;
                moveTime = .91f;
                attackDelay = .91f;
            }
            else if (attackCooldown > 0 && movementCooldown <= 0)
            {
                if (rightWallTouch)
                {
                    move = Vector2.left;
                    movementCooldown = .5f;
                    moveTime = movementCooldown;
                }
                else if (leftWallTouch)
                {
                    move = Vector2.right;
                    movementCooldown = .5f;
                    moveTime = movementCooldown;
                }
                else if (dirVision < 0 && !rightWallTouch)
                {
                    move = Vector2.right;
                    movementCooldown = 1f;
                    moveTime = 1f;
                }
                else if (dirVision >= 0 && !leftWallTouch)
                {
                    move = Vector2.left;
                    movementCooldown = 1f;
                    moveTime = 1f;
                }
            }
        }

        if (movementCooldown > 0)
        {
            movementCooldown -= Time.deltaTime;
            moveTime -= Time.deltaTime;

            if (moveTime <= 0)
            {
                move.x = 0f;
            }
        }

        animator.SetBool("normal attack", normalAttack);
        animator.SetBool("spotted", spottedPlayer);
        animator.SetFloat("move", Mathf.Abs(move.x));
        if (hp.movementCooldown <= 0)
        {
            rb2d.velocity = new Vector2(move.x * .6f, rb2d.velocity.y);
        }
    }

    void Attack()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 4.25f && attackCooldown > 4.05f)
            {
                PlayerHealth playerHP = pc.GetComponent<PlayerHealth>();
                if ((AttackPlayer(0) || AttackPlayer(0.175f) || AttackPlayer(-0.175f)) && !playerHP.gotHit)
                {

                    DamagePlayer();
                }
            }
            else if (attackCooldown == 3.95f && attackCooldown > 3.9f)
            {
                movementCooldown = 1f;
                if (spriteRenderer.flipX)
                {
                    rb2d.AddForce((Vector2.right * 95) + (Vector2.up * 30));
                }
                else if (!spriteRenderer.flipX)
                {
                    rb2d.AddForce((Vector2.left * 95) + (Vector2.up * 30));
                }
            }
        }

        if (attackDelay > 0)
        {
            attackDelay -= Time.deltaTime;
        }

        if (normalAttack)
        {
            normalAttack = false;
            attackCooldown = 4.5f;
            //cc2d.isTrigger = false;
        }
    }

    void DamagePlayer()
    {
        PlayerHealth playerHP = pc.GetComponent<PlayerHealth>();
        PlayerController pcController = pc.GetComponent<PlayerController>();
        playerHP.hp--;
        playerHP.gotHit = true;
        if (pc.transform.position.x < transform.position.x)
        {
            playerHP.hitFromRight = true;
            playerHP.hitFromLeft = false;
        }
        else if (pc.transform.position.x > transform.position.x)
        {
            playerHP.hitFromLeft = true;
            playerHP.hitFromRight = false;
        }
        pcController.moveInputs = false;
        pcController.isGrounded = false;
        pcController.hitTime = .5f;
        pcController.hitAcceleration = 2f;
    }

    bool WallCheck(float distance)
    {
        Vector2 position = transform.position;

        Vector2 wallcheck = position;
        wallcheck.x += distance;
        Debug.DrawLine(position, wallcheck, Color.red);
        RaycastHit2D touch = Physics2D.Linecast(position, wallcheck);
        if (touch.collider != null)
        {
            if (touch.collider.name == "Ground")
            {
                return true;
            }
        }
        return false;
    }

    bool GroundCheck(float distance)
    {
        Vector2 position = transform.position;

        Vector2 groundcheck = position;
        groundcheck.x += distance;
        groundcheck.y -= .13f;
        Debug.DrawLine(position, groundcheck, Color.blue);
        RaycastHit2D touchRight = Physics2D.Linecast(position, groundcheck);
        if (touchRight.collider != null)
        {
            if (touchRight.collider.name == "Ground")
            {
                return true;
            }
        }
        return false;
    }

    bool SpotPlayer(float distance)
    {
        Vector2 position = transform.position;
        position.y += 0.1f;
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

    bool AttackPlayer(float yPosition)
    {
        Vector2 position = transform.position;
        Vector2 attackRange = position;
        attackRange.y += yPosition;
        if (spriteRenderer.flipX)
        {
            attackRange.x -= 0.45f;
            attackRange.x += Mathf.Abs(yPosition / 3f);
        }
        else if (!spriteRenderer.flipX)
        {
            attackRange.x += 0.45f;
            attackRange.x -= Mathf.Abs(yPosition / 3f);
        }
        Debug.DrawLine(position, attackRange, Color.green);
        RaycastHit2D hitPlayer = Physics2D.Linecast(position, attackRange);
        if (hitPlayer.collider != null)
        {
            if (hitPlayer.collider.name == "Player" || hitPlayer.collider.name == "Player(Clone)")
            {
                return true;
            }
        }
        return false;
    }

}
