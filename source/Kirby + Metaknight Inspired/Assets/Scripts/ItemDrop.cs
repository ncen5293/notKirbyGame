using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour {

    public GameObject sword;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void EnemyCheck()
    {
        foreach (Transform enemy in transform)
        {
            EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
            if (enemy.gameObject.name == "Blade Knight" && hp.dead)
            {
                Instantiate(sword, enemy.position, enemy.rotation);
                Destroy(enemy, .3f);
            }
        }
    }
}
