using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Character PlayerInfo { get; private set; }

    public float speed = 2;
    public float jumpforce = 167;
    public float gravity;
    public float idleTime = 5;
    public float boredChance;
    public Vector2 move;
    public bool jump;
    public bool ledge;
    public bool moveInputs;
    public bool isGrounded;
    public float hitTime;
    public float hitAcceleration;
    public float boostTime;
    public float boostAcceleration;
    public bool boosted;
    public LayerMask groundLayer;
    public bool attack;
    public float attackCooldown;
    public int wepDurability;
    public int weapon;  //0 == no wep
                        //1 == sword
                        //2 == ?
                        //3 == ?
                        //4 == ?
    public int throwable;
    public float throwCooldown;
    public float damageCooldown;
    public GameObject sword;
    public GameObject pitchfork;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb2d;
    private PlayerHealth hp;

    private void OnEnable()
    {
        
    }

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        hp = GetComponent<PlayerHealth>();
        moveInputs = true;
        ledge = false;
        gravity = rb2d.gravityScale;
        boredChance = 0;
        hitTime = -1f;
        boostTime = -1f;
        boosted = false;
        attack = false;
        attackCooldown = 0;
        weapon = 0;
        throwable = 0;
        throwCooldown = 0;

        PlayerInfo = PlayerPersistence.LoadData();
        weapon = PlayerInfo.Weapon;
        throwable = PlayerInfo.Throwable;
    }
	
	// Update is called once per frame
	void Update () {
        Movement();
        Bored();
        Damage();
        Boost();
        Attack();
        DropWeapon();
        GetItem();
        isGrounded = RightGrounded() || LeftGrounded();
    }

    bool RightGrounded()
    {
        Vector2 position = transform.position;
        position.x += .08f;
        RaycastHit2D onGround = Physics2D.Raycast(position, Vector2.down, .2f, groundLayer);
        if (onGround.collider != null)
        {
            return true;
        }
        return false;
    }

    bool LeftGrounded()
    {
        Vector2 position = transform.position;
        position.x -= .08f;
        RaycastHit2D onGround = Physics2D.Raycast(position, Vector2.down, .2f, groundLayer);
        if (onGround.collider != null)
        {
            return true;
        }
        return false;
    }

    void Movement()
    {
        if(moveInputs)
        {
            move.x = Input.GetAxis("Horizontal");
            if (move.x < 0 && !spriteRenderer.flipX && !attack)
            {
                spriteRenderer.flipX = true;
            }
            else if (move.x > 0 && spriteRenderer.flipX && !attack)
            {
                spriteRenderer.flipX = false;
            }
        }
        else if(!moveInputs)
        {
            move.x = 0f;
        }
        if ((move.x > 0 && (NextToWall(0.13f, -0.06f) || NextToWall(0.13f, 0.08f) || NextToWall(0.13f, 0f))) || (move.x < 0 && (NextToWall(-0.13f, -0.06f) || NextToWall(-0.13f, 0.08f) || NextToWall(-0.13f, 0f))))
        {
            move.x = 0;
        }
        if (move.x > 0 && NextToEnemy(0.13f) || move.x < 0 && NextToEnemy(0.13f))
        {
            move.x = 0;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && !ledge)
        {
            rb2d.AddForce(Vector2.up * jumpforce);
            jump = true;
            isGrounded = false;
        }
        else
            jump = false;

        if (NextToWall(0.13f, -0.06f) && NextToLedge(0.13f) && !ledge && !spriteRenderer.flipX)
        {
            boostTime = 0;
            isGrounded = false;
            ledge = true;
            animator.Play("player_quickgrab");
            if (ShortLedge(0.13f))
            {
                animator.Play("player_halfgrab");
            }
            rb2d.velocity = new Vector2(0, 0);
            StartCoroutine(LedgeGrab(1, false));

        }
        if (NextToWall(-0.13f, -0.06f) && NextToLedge(-0.13f) && !ledge && spriteRenderer.flipX)
        {
            boostTime = 0;
            isGrounded = false;
            ledge = true;
            if (!ShortLedge(-0.13f))
            {
                animator.Play("player_quickgrab");
            }
            else if (ShortLedge(-0.13f))
            {
                animator.Play("player_halfgrab");
            }
            rb2d.velocity = new Vector2(0, 0);
            StartCoroutine(LedgeGrab(-1, false));
        }
        if (!NextToWall(-0.13f, 0f) && NextToWall(-0.13f, 0.08f) && !ledge && spriteRenderer.flipX)
        {
            boostTime = 0;
            isGrounded = false;
            ledge = true;
            animator.Play("player_quickgrab");
            rb2d.velocity = new Vector2(0, 0);
            StartCoroutine(LedgeGrab(-1, true));
        }
        if (!NextToWall(0.13f, 0f) && NextToWall(0.13f, 0.08f) && !ledge && !spriteRenderer.flipX)
        {
            boostTime = 0;
            isGrounded = false;
            ledge = true;
            animator.Play("player_quickgrab");
            rb2d.velocity = new Vector2(0, 0);
            StartCoroutine(LedgeGrab(1, true));
        }

        animator.SetBool("ledge", ledge);
        animator.SetBool("jump", jump);
        animator.SetFloat("move", Mathf.Abs(move.x));

        if(!hp.gotHit)
        {
            rb2d.velocity = new Vector2(move.x * speed, rb2d.velocity.y);
        }
        

    }

    void Bored()
    {
        if (rb2d.velocity.x == 0 && rb2d.velocity.y == 0)
        {
            idleTime -= Time.deltaTime;
        }
        else if (rb2d.velocity.x > 0 || rb2d.velocity.y > 0)
        {
            idleTime = 5;
            boredChance = 0;
        }
        if (idleTime < 0.1f)
        {
            boredChance = Random.Range(0, 50);
            idleTime = 5;
        }

        animator.SetFloat("bored", boredChance);
    }

    void Damage()
    {
        if (hitTime > 0)
        {
            boostTime = 0;
            hitTime -= Time.deltaTime;
            float accelerationY = -.5f;
            hitAcceleration *= .85f;
            if (hitTime > .25f && hp.hitFromLeft)
            {
                rb2d.velocity = new Vector2(1.5f * hitAcceleration, 1f * hitAcceleration);
            }
            else if (hitTime > .25f && hp.hitFromRight)
            {
                rb2d.velocity = new Vector2(-1.5f * hitAcceleration, 1f * hitAcceleration);
            }
            else if (hitTime <= .25f && hitTime > .1f)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x * .87f, accelerationY);
                accelerationY *= 1.8f;
            }
            else if (hitTime <= .1f)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y);
            }
        }
        if (hp.gotHit && hitTime <= 0f)
        {
            moveInputs = true;
            isGrounded = true;
            hp.gotHit = false;
            hp.hitFromLeft = false;
            hp.hitFromRight = false;
        }

        animator.SetBool("hit", hp.gotHit);
    }

    void Boost()
    {
        if(Input.GetKeyDown(KeyCode.Space) && hp.power > 0 && !boosted)
        {
            animator.Play("player_boost1");
            isGrounded = false;
            boosted = true;
            boostTime = 1.5f;
            hp.power--;
            boostAcceleration = 5f;
        }
        if(boostTime > 0)
        {
            boosted = true;
            boostTime -= Time.deltaTime;
            if(!ledge && !hp.gotHit)
            {
                if (boostTime > 1.1f)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, boostAcceleration);
                }
                if (boostTime < 1 && boostAcceleration > 0)
                {
                    boostAcceleration = -.825f;
                }
                else if(boostTime < .5f)
                {
                    boostAcceleration *= 1.01f;
                }

                if (boostAcceleration > 2)
                {
                    boostAcceleration *= .875f;
                }
                else if (boostAcceleration < 2 && boostAcceleration > 0 && boostTime >= 1)
                {
                    boostAcceleration *= .925f;
                    boosted = false;
                }
            }

        }
        if (boostTime <= 0)
        {
            boosted = false;
        }
        animator.SetBool("boost", boosted);
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !hp.gotHit && !boosted && !ledge && !attack)
        {
            if (weapon == 0)
            {
                animator.Play("player_attack0");
                attack = true;
                attackCooldown = .65f;
            }
            else if (weapon == 1 && hp.durability > 0)
            {
                animator.Play("player_sword2");
                attack = true;
                attackCooldown = .85f;
                hp.durability--;
            }
            //moveInputs = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt) && !hp.gotHit && !boosted && !ledge && !attack)
        {
            if (throwable > 0)
            {
                Vector3 position = transform.position;
                GameObject thrown = null;
                throwable--;
                throwCooldown = 0.6f;
                attack = true;
                if (spriteRenderer.flipX)
                {
                    position.x -= 0.13f;
                    thrown = Instantiate(pitchfork, position, transform.rotation);
                }
                else if (!spriteRenderer.flipX)
                {
                    position.x += 0.13f;
                    thrown = Instantiate(pitchfork, position, transform.rotation);
                }
                animator.Play("player_shoot2");
                thrown.GetComponent<DropAnim>().playerThrow = true;
                thrown.GetComponent<DropAnim>().throwable = true;
                thrown.GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown < .37f && attackCooldown >= .3f && weapon == 0)
            {
                TakeItem(.38f, 0);
                TakeItem(.38f, 0.06f);
                TakeItem(.38f, -0.06f);
                TakeItem(.38f, 0.03f);
                TakeItem(.38f, -0.03f);
                TakeItem(.38f, 0.04f);
                TakeItem(.38f, -0.04f);
                TakeItem(.38f, 0.05f);
                TakeItem(.38f, -0.05f);
            }
            if (attackCooldown < .3f && attackCooldown > .2f && weapon == 0)
            {
                bool gaveDamage = HitEnemy(.39f);
                TakeItem(.58f, 0);
                TakeItem(.58f, 0.06f);
                TakeItem(.58f, -0.06f);
                TakeItem(.58f, 0.03f);
                TakeItem(.58f, -0.03f);
                TakeItem(.58f, 0.04f);
                TakeItem(.58f, -0.04f);
                TakeItem(.58f, 0.05f);
                TakeItem(.58f, -0.05f);
                damageCooldown = 0.6f;
                if (gaveDamage)
                {
                    attackCooldown = 0.19f;
                }
            }
            else if (attackCooldown < .683f && attackCooldown > .41f && weapon == 1)
            {
                bool gaveDamage = HitEnemy(.54f);
                TakeItem(.54f, 0);
                TakeItem(.54f, 0.06f);
                TakeItem(.54f, -0.06f);
                TakeItem(.54f, 0.03f);
                TakeItem(.54f, -0.03f);
                TakeItem(.54f, 0.04f);
                TakeItem(.54f, -0.04f);
                TakeItem(.54f, 0.05f);
                TakeItem(.54f, -0.05f);
                damageCooldown = 0.6f;
                if (gaveDamage)
                {
                    attackCooldown = 0.4f;
                }
            }

        }
        else if (throwCooldown > 0)
        {
            throwCooldown -= Time.deltaTime;
        }
        else if (attackCooldown <= 0 && throwCooldown <= 0)
        {
            attack = false;
            //moveInputs = true;
        }

        if (damageCooldown > 0)
        {
            damageCooldown -= Time.deltaTime;
        }
        
    }

    bool TakeItem(float distance, float height)
    {
        Vector2 position = transform.position;
        position.y += height;
        Vector2 item = position;
        if (spriteRenderer.flipX)
        {
            item.x -= distance;
        }
        else if (!spriteRenderer.flipX)
        {
            item.x += distance;
        }
        //Debug.DrawLine(position, item, Color.blue);
        RaycastHit2D touch = Physics2D.Linecast(position, item);
        if (touch.collider != null)
        {
            if (touch.collider.gameObject.name == "Sword" || touch.collider.gameObject.name == "Sword(Clone)")
            {
                DropAnim wep = touch.collider.gameObject.GetComponent<DropAnim>();
                if (!wep.playerGet && damageCooldown <= 0)
                {
                    if (weapon != 1)
                    {
                        //pc.weapon = 1;
                        //int prevDurability = hp.durability;
                        hp.durability = wep.durability;
                    }
                    else if (weapon == 1)
                    {
                        hp.durability += wep.durability;
                    }
                    //Destroy(col.gameObject);
                    if (hp.durability > 5)
                    {
                        hp.durability = 5;
                    }
                    wep.playerPickup = true;
                }
                return true;
            }
            else if (touch.collider.gameObject.name == "Pitchfork" || touch.collider.gameObject.name == "Pitchfork(Clone)")
            {
                DropAnim wep = touch.collider.gameObject.GetComponent<DropAnim>();
                if (!wep.playerGet && throwCooldown <= 0 && damageCooldown <= 0)
                {
                    wep.throwable = false;
                    wep.playerPickup = true;
                }
                return true;
            }
        }
        return false;
    }

    bool HitEnemy(float rangeX)
    {
        EnemyHealth enemyHealth = null;
        PitchforkSoldier forkSoldier = null;
        SwordKnightBoss swordBoss = null;
        Vector2 attackRod = transform.position;
        Vector2 attackRod2 = attackRod;
        attackRod2.y += 0.19f;
        Vector2 attackRod3 = attackRod;
        attackRod3.y -= 0.19f;
        bool range1 = false;
        bool range2 = false;
        bool range3 = false;
        if (spriteRenderer.flipX)
        {
            attackRod.x -= rangeX;
            attackRod2.x -= (rangeX + 0.09f);
            attackRod3.x -= (rangeX - 0.09f);
        }
        else if (!spriteRenderer.flipX)
        {
            attackRod.x += rangeX;
            attackRod2.x += (rangeX + 0.09f);
            attackRod3.x += (rangeX - 0.09f);
        }

        Debug.DrawLine(transform.position, attackRod, Color.green);
        RaycastHit2D rod1 = Physics2D.Linecast(transform.position, attackRod);
        if (rod1.collider != null)
        {
            if (rod1.collider.tag == "Enemy")
            {
                if (rod1.collider.gameObject.name == "Blade Knight")
                {
                    enemyHealth = rod1.collider.GetComponent<EnemyHealth>();
                }
                else if (rod1.collider.gameObject.name == "Pitchfork Soldier")
                {
                    forkSoldier = rod1.collider.GetComponent<PitchforkSoldier>();
                }
                else if (rod1.collider.gameObject.name == "Sword Knight")
                {
                    swordBoss = rod1.collider.GetComponent<SwordKnightBoss>();
                }
                range1 = true;
            }
        }

        Debug.DrawLine(transform.position, attackRod, Color.green);
        RaycastHit2D rod2 = Physics2D.Linecast(transform.position, attackRod2);
        if (rod2.collider != null)
        {
            if (rod2.collider.tag == "Enemy")
            {
                if (rod2.collider.gameObject.name == "Blade Knight")
                {
                    enemyHealth = rod2.collider.GetComponent<EnemyHealth>();
                }
                else if (rod2.collider.gameObject.name == "Pitchfork Soldier")
                {
                    forkSoldier = rod2.collider.GetComponent<PitchforkSoldier>();
                }
                else if (rod2.collider.gameObject.name == "Sword Knight")
                {
                    swordBoss = rod2.collider.GetComponent<SwordKnightBoss>();
                }
                range2 = true;
            }
        }

        Debug.DrawLine(transform.position, attackRod, Color.green);
        RaycastHit2D rod3 = Physics2D.Linecast(transform.position, attackRod3);
        if (rod3.collider != null)
        {
            if (rod3.collider.tag == "Enemy")
            {
                if (rod3.collider.gameObject.name == "Blade Knight")
                {
                    enemyHealth = rod3.collider.GetComponent<EnemyHealth>();
                }
                else if (rod3.collider.gameObject.name == "Pitchfork Soldier")
                {
                    forkSoldier = rod3.collider.GetComponent<PitchforkSoldier>();
                }
                else if (rod3.collider.gameObject.name == "Sword Knight")
                {
                    swordBoss = rod3.collider.GetComponent<SwordKnightBoss>();
                }
                range3 = true;
            }
        }
        if (range1 || range2 || range3)
        {
            if (enemyHealth != null && enemyHealth.knockbackTime == 0)
            {
                enemyHealth.health--;
                enemyHealth.hit = true;
            }
            else if (forkSoldier != null && forkSoldier.hp > 0)
            {
                forkSoldier.hp--;
            }
            else if (swordBoss != null && !swordBoss.damaged && swordBoss.hp > 0)
            {
                swordBoss.damaged = true;
            }
        }
        return range1 || range2 || range3;
    }

    void DropWeapon()
    {
        if ((Input.GetKeyDown(KeyCode.X) || (hp.durability == 0 && attack == false)) && weapon == 1)
        {
            Vector3 wepPosition = transform.position;
            wepPosition.y += .2f;
            GameObject wep = Instantiate(sword, wepPosition, transform.rotation);
            DropAnim dropped = wep.GetComponent<DropAnim>();
            dropped.playerDrop = true;
            dropped.durability = hp.durability;
            weapon = 0;
            hp.durability = 0;
        }
    }

    void GetItem()
    {
        Collider2D [] items = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D item in items)
        {
            if (item.gameObject.name == "Sword" || item.gameObject.name == "Sword(Clone)")
            {
                DropAnim wep = item.gameObject.GetComponent<DropAnim>();
                if (!wep.playerGet && attackCooldown <= 0)
                {
                    if (weapon != 1)
                    {
                        //pc.weapon = 1;
                        //int prevDurability = hp.durability;
                        hp.durability = wep.durability;
                    }
                    else if (weapon == 1)
                    {
                        hp.durability += wep.durability;
                    }
                    if (hp.durability > 10)
                    {
                        hp.durability = 10;
                    }
                    //Destroy(col.gameObject);
                    wep.playerPickup = true;
                }
            }
            else if (item.gameObject.name == "Pitchfork" || item.gameObject.name == "Pitchfork(Clone)")
            {
                DropAnim wep = item.gameObject.GetComponent<DropAnim>();
                if (!wep.playerGet && throwCooldown <= 0 && !wep.enemyThrown)
                {
                    if (throwable < 10)
                    {
                        throwable += 3;
                    }
                    if (throwable > 10)
                    {
                        throwable = 10;
                    }
                    //Destroy(col.gameObject);
                    wep.playerPickup = true;
                }
            }
            else if (item.gameObject.tag == "Item")
            {
                if (item.gameObject.name == "Energy" || item.gameObject.name == "Energy(Clone)")
                {
                    hp.hp += 2;
                    if (hp.hp > 8)
                    {
                        hp.hp = 8;
                    }
                    hp.regainHealth = true;
                    Destroy(item.gameObject);
                }
                else if (item.gameObject.name == "Power")
                {
                    hp.power++;
                    if (hp.power > 5)
                    {
                        hp.power = 5;
                    }
                    Destroy(item.gameObject);
                }

                hp.regainHealth = false;
            }
        }
    }

    bool NextToWall(float distance, float length)
    {
        Vector2 position = transform.position;
        position.y += length;
        Vector2 wallCheck = position;
        wallCheck.x += distance;
        //Debug.DrawLine(transform.position, wallCheck, Color.blue);
        RaycastHit2D touch = Physics2D.Linecast(position, wallCheck);
        if (touch.collider != null)
        {
            if (touch.collider.name == "Ground")
            {
                return true;
            }
        }
        return false;
    }

    bool NextToEnemy(float distance)
    {
        Vector2 position = transform.position;
        Vector2 wallCheck = position;
        wallCheck.x += distance;
        RaycastHit2D touch = Physics2D.Linecast(position, wallCheck);
        if (touch.collider != null)
        {
            if (touch.collider.tag == "Enemy")
            {
                return true;
            }
        }
        return false;
    }

    bool NextToLedge(float distance)
    {
        Vector2 position = transform.position;

        Vector2 ledgecheck = position;
        ledgecheck.x += distance;
        ledgecheck.y += .13f;
        Debug.DrawLine(position, ledgecheck, Color.blue);
        RaycastHit2D touchRight = Physics2D.Linecast(position, ledgecheck);
        if (touchRight.collider != null)
        {
            if (touchRight.collider.name == "Ground")
            {
                return false;
            }
        }
        return true;
    }

    bool ShortLedge(float distance)
    {
        Vector2 position = transform.position;
        position.y += 0.08f;
        Vector2 ledgecheck = position;
        ledgecheck.x += distance;
        Debug.DrawLine(position, ledgecheck, Color.blue);
        RaycastHit2D touch = Physics2D.Linecast(position, ledgecheck);
        if (touch.collider != null)
        {
            if (touch.collider.name == "Ground")
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator LedgeGrab(float direction, bool thin)
    {
        moveInputs = false;
        rb2d.gravityScale = 0f;
        if (((!ShortLedge(0.13f) && direction > 0) || (!ShortLedge(-0.13f) && direction < 0)) && !hp.gotHit)
        {
            yield return new WaitForSeconds(.08f);
        }
        if ((((NextToWall(-0.13f, -0.06f) && NextToLedge(-0.13f)) || (NextToWall(0.13f, -0.06f) && NextToLedge(0.13f))) || thin) && !hp.gotHit)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + .05f);
        }
        if (!hp.gotHit)
        {
            yield return new WaitForSeconds(.08f);
        }
        if ((((NextToWall(-0.13f, -0.06f) && NextToLedge(-0.13f)) || (NextToWall(0.13f, -0.06f) && NextToLedge(0.13f))) || thin) && !hp.gotHit)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + .05f);
        }
        if (!hp.gotHit)
        {
            yield return new WaitForSeconds(.08f);
        }
        if ((((NextToWall(-0.13f, -0.06f) && NextToLedge(-0.13f)) || (NextToWall(0.13f, -0.06f) && NextToLedge(0.13f))) || thin) && !hp.gotHit)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + .05f);
        }
        if (!hp.gotHit)
        {
            yield return new WaitForSeconds(.08f);
        }
        if ((((NextToWall(-0.13f, -0.06f) && NextToLedge(-0.13f)) || (NextToWall(0.13f, -0.06f) && NextToLedge(0.13f))) || thin) && !hp.gotHit)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + .04f);
        }
        if (!hp.gotHit)
        {
            yield return new WaitForSeconds(.18f);
        }
        if (!hp.gotHit)
        {
            transform.position = new Vector2(transform.position.x + (.1f * direction) / 2, transform.position.y);
            yield return new WaitForSeconds(.12f);
        }
        rb2d.gravityScale = gravity;
        moveInputs = true;
        if (!hp.gotHit)
        {
            yield return new WaitForSeconds(.06f);
            transform.position = new Vector2(transform.position.x + (.1f * direction) / 2, transform.position.y);
        }
        ledge = false;
    }
}
