using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDispenser : MonoBehaviour {

    public GameObject item;
    public bool startup;
    public bool gotItem;

    private SpriteRenderer spriteRenderer;
    private GameObject cutscene;
    private Crashland crashed;
    private Vector3 spawnCheck;
    private GameObject player;
    private PlayerHealth pc_hp;
    private GameObject consumable;
    Vector3 position;

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cutscene = GameObject.FindWithTag("Spawn");
        crashed = cutscene.GetComponent<Crashland>();
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);
        position = transform.position;
        if (spriteRenderer.flipX)
        {
            position.x -= 0.2f;
        }
        else if (!spriteRenderer.flipX)
        {
            position.x += 0.2f;
        }
        consumable = Instantiate(item, position, transform.rotation);
        if (consumable.gameObject.name == "Power(Clone)")
        {
            consumable.gameObject.name = "Power";
        }
        ItemFloat dropping = consumable.GetComponent<ItemFloat>();
        dropping.dropTime = 2;
        dropping.dropStrength = Vector2.down * 10;
        dropping.dropped = false;
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
            pc_hp = player.GetComponent<PlayerHealth>();
            Dispense();
        }
        if (!gotItem)
        {
            if (consumable.transform.position.y < -10)
            {
                Destroy(consumable);
                consumable = Instantiate(item, position, transform.rotation);
                ItemFloat dropping = consumable.GetComponent<ItemFloat>();
                dropping.dropTime = 2;
                dropping.dropStrength = Vector2.down * 10;
                dropping.dropped = false;
            }
        }
    }

    void Dispense()
    {
        if (pc_hp.power <= 0 && gotItem)
        {
            position = transform.position;
            if (spriteRenderer.flipX)
            {
                position.x -= 0.2f;
            }
            else if (!spriteRenderer.flipX)
            {
                position.x += 0.2f;
            }
            consumable = Instantiate(item, position, transform.rotation);
            if (consumable.gameObject.name == "Power(Clone)")
            {
                consumable.gameObject.name = "Power";
            }
            ItemFloat dropping = consumable.GetComponent<ItemFloat>();
            dropping.dropTime = 2;
            dropping.dropStrength = Vector2.down * 10;
            dropping.dropped = false;
            gotItem = false;
        }
        else if (pc_hp.power > 0 && !gotItem)
        {
            gotItem = true;
        }
    }
}
