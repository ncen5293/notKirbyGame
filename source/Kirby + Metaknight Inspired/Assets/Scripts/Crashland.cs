using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crashland : MonoBehaviour {

    public float crashTime;
    public bool spawnPlayer;
    public GameObject player;
    public Vector2 crashForce;
    public bool inUse;
    public bool stayCheck;
    public bool levelTransition;

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        //crashTime = 2.4f;
        spawnPlayer = false;
        inUse = true;
        stayCheck = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!levelTransition)
        {
            Crashing();
            Spawn();
        }
        else if (levelTransition)
        {
            Unused();
        }
	}

    void Unused()
    {
        transform.position = new Vector2(-1.5f, 1.5f);
        rb2d.gravityScale = 0;
        rb2d.velocity = Vector2.zero;
    }

    void Crashing()
    {
        if (crashTime > 0 && inUse)
        {
            crashTime -= Time.deltaTime;
            rb2d.AddForce(crashForce * 2);
            rb2d.rotation -= 8.25f;
        }
        else if (crashTime <= 0 && inUse)
        {
            spawnPlayer = true;
        }
    }

    void Spawn()
    {
        if (spawnPlayer)
        {
            stayCheck = true;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            spawnPlayer = false;
            GameObject spawn = Instantiate(player, transform.position, transform.rotation);
            spawn.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 100);
            spawn.GetComponent<PlayerHealth>().hp = 3;
            //Destroy(this.gameObject);
            transform.position = new Vector2(-1.5f, 1.5f);
            inUse = false;
            rb2d.gravityScale = 0;
            rb2d.velocity = Vector2.zero;
        }

        if (stayCheck)
        {
            transform.position = new Vector2(-1.5f, 1.5f);
        }
    }
}
