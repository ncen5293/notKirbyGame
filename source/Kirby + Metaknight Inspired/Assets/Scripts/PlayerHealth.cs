using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {

    public Character PlayerInfo { get; private set; }

    public int hp = 8;
    public bool gotHit;
    public bool regainHealth;
    public bool enemyRight;
    public bool hitFromRight;
    public bool hitFromLeft;
    public int durability;
    public float deathTime;
    public bool dead;
    public float inviniciblitiy;
    public int power;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb2d;
    private PlayerController pc;
    private BoxCollider2D bc2d;

    private void OnEnable()
    {
        
    }

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
        bc2d = GetComponent<BoxCollider2D>();
        gotHit = false;
        regainHealth = false;
        hitFromLeft = false;
        hitFromRight = false;
        dead = false;
        PlayerInfo = PlayerPersistence.LoadData();
        //transform.position = PlayerInfo.Position;
        if (hp > 0)
        {
            hp = PlayerInfo.Hp;
        }
        power = PlayerInfo.Power;
        durability = PlayerInfo.Durability;
	}
	
	// Update is called once per frame
	void Update () {
        HealthCheck();
        NoDamage();
	}

    void NoDamage()
    {
        if (inviniciblitiy > 0)
        {
            inviniciblitiy -= Time.deltaTime;
        }
    }

    void HealthCheck()
    {
        if (hp < 0)
        {
            hp = 0;
        }
        if ((hp == 0 || transform.position.y <= -1.25f) && !dead)
        {
            bc2d.isTrigger = true;
            rb2d.gravityScale = 0;
            dead = true;
            deathTime = 1.167f;
            //hp = -1;
            animator.Play("player_deathstart");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //print("death");
        }
        if (deathTime <= 0 && dead)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (dead)
        {
            pc.moveInputs = false;
            deathTime -= Time.deltaTime;
            if (deathTime <= 1 && deathTime > 0.9f)
            {
                rb2d.gravityScale = 1;
                rb2d.AddForce(Vector2.up * 100);
            }
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Exit")
        {
            if (SceneManager.GetActiveScene().name != "level1-1" || SceneManager.GetActiveScene().name != "level1-2" || SceneManager.GetActiveScene().name != "start")
            gameObject.transform.position = new Vector3(0, 0, 0);
            /*if (col.gameObject.name == "level1-boss")
            {
                DontDestroyOnLoad(gameObject);
                gameObject.transform.position = new Vector3(-1, -.185f, 0);
            }*/
            PlayerPersistence.SaveData(this);
            PlayerPersistence.SaveData(pc);
            SceneManager.LoadScene(col.gameObject.name);
        }
        if (!pc.ledge)
        {
            if ((col.gameObject.tag == "Environment" || col.gameObject.tag == "Enemy") && !gotHit)
            {
                hp--;
                gotHit = true;
                animator.Play("player_hurt0");
                if (transform.position.x < col.transform.position.x)
                {
                    hitFromRight = true;
                    hitFromLeft = false;
                }
                else if (transform.position.x > col.transform.position.x)
                {
                    hitFromLeft = true;
                    hitFromRight = false;
                }
                pc.moveInputs = false;
                pc.isGrounded = false;
                pc.hitTime = .5f;
                pc.hitAcceleration = 2f;
            }
        }

    }
}
