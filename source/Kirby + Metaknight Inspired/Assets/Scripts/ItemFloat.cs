using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemFloat : MonoBehaviour {

    public float floatTime;
    public bool switchFloat;
    public bool dropped;
    public float dropTime = 0.5f;
    public Vector2 dropStrength;

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        floatTime = 0;
        switchFloat = false;
        dropped = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (dropped)
        {
            Floating();
        }
        else if (!dropped)
        {
            Dropping();
        }
        if (SceneManager.GetActiveScene().name == "level1-boss" && transform.position.y <= 0.2f)
        {
            rb2d.velocity = Vector2.zero;
        }
	}

    void Floating()
    {
        if (floatTime >= .15f)
        {
            switchFloat = true;
        }
        else if (floatTime <= -.15f)
        {
            switchFloat = false;
        }
        
        if (switchFloat)
        {
            floatTime -= Time.deltaTime;
            rb2d.velocity = Vector2.up * .09f;
        }
        else if (!switchFloat)
        {
            floatTime += Time.deltaTime;
            rb2d.velocity = Vector2.down *.09f;
        }
    }

    void Dropping()
    {
        if (dropTime > 0)
        {
            dropTime -= Time.deltaTime;
            rb2d.AddForce(dropStrength);
        }
        else if (dropTime <= 0)
        {
            dropped = true;
        }
    }
}
