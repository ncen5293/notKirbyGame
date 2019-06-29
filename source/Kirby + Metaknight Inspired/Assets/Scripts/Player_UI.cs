using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_UI : MonoBehaviour {

    public Sprite[] powerSprites;
    public Sprite[] powerDecrease;
    public Sprite[] healthSprites;
    public Image healthUI;
    public Image powerUI;
    public Sprite[] swordSprites;
    public Sprite[] forkSprites;
    public Image[] swordUI;
    public Image[] forkUI;
    public float width = 16f;
    public float height = 9f;

    private GameObject player;
    private PlayerHealth hp;
    private PlayerController pc;
    private GameObject cutscene;
    private Crashland crashed;
    private Vector3 spawnCheck;

    void Awake()
    {
        Camera.main.aspect = width / height;
    }

    // Use this for initialization
    void Start () {
        cutscene = GameObject.FindWithTag("Spawn");
        crashed = cutscene.GetComponent<Crashland>();
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (crashed.transform.position == spawnCheck)
        {
            player = GameObject.FindWithTag("Player");
            hp = player.GetComponent<PlayerHealth>();
            pc = player.GetComponent<PlayerController>();
            HealthCheck();
            PowerCheck();
            WeaponCheck();
        }
	}

    void PowerCheck()
    {
        powerUI.sprite = powerSprites[hp.power];
    }

    void HealthCheck()
    {
        healthUI.sprite = healthSprites[hp.hp];
    }

    void WeaponCheck()
    {
        for (int i = 0; i < swordUI.Length; i++)
        {
            if (i < hp.durability)
            {
                swordUI[i].sprite = swordSprites[1];
            }
            else if (i >= hp.durability)
            {
                swordUI[i].sprite = swordSprites[0];
            }
        }

        for (int i = 0; i < forkUI.Length; i++)
        {
            if (i < pc.throwable)
            {
                forkUI[i].sprite = forkSprites[1];
            }
            else if (i >= pc.throwable)
            {
                forkUI[i].sprite = forkSprites[0];
            }
        }
    }
}
