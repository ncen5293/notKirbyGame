using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrab : MonoBehaviour {

    public Vector2 climb;

    private Animator animator;

    // Use this for initialization
    void Start () {
        //GetComponent<Rigidbody2D>().velocity = new Vector2(4, 8);
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.name == "Ledge")
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(1, 1);
            GetComponent<Rigidbody2D>().gravityScale = 0;
            animator.SetBool("ledge", true);
            climb = GetComponent<Transform>().position;
        }
    }

    void Climb()
    {
        GetComponent<Transform>().position = new Vector2(climb.x + 1, climb.y + 1);
    }
}
