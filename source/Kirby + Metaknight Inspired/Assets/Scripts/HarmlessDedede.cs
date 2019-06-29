using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmlessDedede : MonoBehaviour {

    public bool knockedout = false;
    public GameObject healthItems;

    private Crashland crash;
    private GameObject falling;
    private Animator animator;
    private Vector3 spawnCheck;

	// Use this for initialization
	void Start () {
        falling = GameObject.FindWithTag("Spawn");
        crash = falling.GetComponent<Crashland>();
        animator = GetComponent<Animator>();
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);

    }
	
	// Update is called once per frame
	void Update () {
		if (crash.transform.position == spawnCheck && !knockedout)
        {
            animator.Play("dedede_knockedout0");
            Vector2 spawnPosition = transform.position;
            spawnPosition.x -= .2f;
            //spawnPosition.y -= .2f;
            GameObject spawn = Instantiate(healthItems, spawnPosition, transform.rotation);
            spawn.GetComponent<ItemFloat>().dropStrength = Vector2.left + Vector2.down;
            spawnPosition.x += .5f;
            GameObject spawn2 = Instantiate(healthItems, spawnPosition, transform.rotation);
            spawn2.GetComponent<ItemFloat>().dropStrength = Vector2.right + Vector2.down;
            spawnPosition.y += .2f;
            GameObject spawn3 = Instantiate(healthItems, spawnPosition, transform.rotation);
            spawn3.GetComponent<ItemFloat>().dropStrength = Vector2.right + Vector2.down;
            knockedout = true;
        }
	}
}
