using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayer : MonoBehaviour {

    private Cinemachine.CinemachineVirtualCamera cam;
    private GameObject cutscene;
    private Crashland crashed;
    private Vector3 spawnCheck;

    // Use this for initialization
    void Start () {
        cam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        cutscene = GameObject.FindWithTag("Spawn");
        crashed = cutscene.GetComponent<Crashland>();
        spawnCheck = new Vector3(-1.5f, 1.5f, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (crashed.transform.position == spawnCheck)
        {
            cam.Follow = GameObject.FindWithTag("Player").transform;
        }
	}
}
