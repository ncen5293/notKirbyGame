using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {

	public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadBeginning()
    {
        hp.hp = 3;
        hp.power = 0;
        pc.weapon = 0;
        hp.durability = 0;
        pc.throwable = 0;
        PlayerPersistence.SaveData(hp);
        PlayerPersistence.SaveData(pc);
        SceneManager.LoadScene("level1-1");
    }

    public void LoadBoss()
    {
        hp.hp = 8;
        hp.power = 5;
        pc.weapon = 1;
        hp.durability = 10;
        pc.throwable = 10;
        PlayerPersistence.SaveData(hp);
        PlayerPersistence.SaveData(pc);
        SceneManager.LoadScene("level1-boss");
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void Continue()
    {
        Time.timeScale = 1;
        Unpaused();
    }

    public Image[] PauseMenu;
    public Sprite[] ShowMenu;
    public GameObject fakePlayer;

    private Vector3 mousePosition;
    private GameObject cursor;
    private Vector3 startButton = new Vector3(865, 360, 0);
    private Vector3 endButton = new Vector3(865, 300, 0);
    private Vector3 bossButton = new Vector3(1130, 330, 0);
    private Vector3 continueButton = new Vector3(779.8f, 377f, 0);
    private Vector3 gotoMenuButton = new Vector3(779.8f, 297f, 0);
    private GameObject playerInfo;
    private PlayerHealth hp;
    private PlayerController pc;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "mainmenu")
        {
            playerInfo = GameObject.FindWithTag("Player");
            hp = playerInfo.GetComponent<PlayerHealth>();
            pc = playerInfo.GetComponent<PlayerController>();
        }
        else if (SceneManager.GetActiveScene().name != "mainmenu")
        {
            Time.timeScale = 1;
            Unpaused();
        }
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        cursor = GameObject.FindWithTag("Cursor");
        print(mousePosition);
        if (SceneManager.GetActiveScene().name == "mainmenu")
        {
            MainMenu();
        }
        else if (SceneManager.GetActiveScene().name != "mainmenu" && Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                Paused();
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                Unpaused();
            }
        }

        if (Time.timeScale == 0)
        {
            InGame();
        }
        else if (Time.timeScale == 1)
        {
            Unpaused();
        }
    }

    void Paused()
    {
        for (int i = 0; i < PauseMenu.Length; i++)
        {
            PauseMenu[i].sprite = ShowMenu[i];
        }
    }

    void Unpaused()
    {
        for (int i = 0; i < PauseMenu.Length; i++)
        {
            PauseMenu[i].sprite = ShowMenu[5];
        }
    }

    void InGame()
    {
        /*
        if (mousePosition.y <= 0.6f && mousePosition.y > 0.5f && mousePosition.x > 0.7f && mousePosition.x < 1.2f)
        {
            cursor.transform.position = new Vector3(643, 236.5f, 0);
        }
        else if (mousePosition.y <= 0.4f && mousePosition.y > 0.3f && mousePosition.x > 0.7f && mousePosition.x < 1.2f)
        {
            cursor.transform.position = new Vector3(643, 196.5f, 0);
        }*/

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursor.transform.position = gotoMenuButton;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cursor.transform.position = continueButton;
        }

        if (cursor.transform.position == continueButton && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            Time.timeScale = 1;
            Unpaused();
        }
        else if (cursor.transform.position == gotoMenuButton && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("mainmenu");
        }
    }

    void MainMenu()
    {
        if (mousePosition.y <= 0 && mousePosition.y > -0.6f && mousePosition.x > 0 && mousePosition.x < 3.2f)
        {
            cursor.transform.position = startButton;
        }
        else if (mousePosition.y <= -0.9f && mousePosition.y > -1.4f && mousePosition.x > 0 && mousePosition.x < 3.2f)
        {
            cursor.transform.position = endButton;
        }
        else if (mousePosition.y <= -0.4f && mousePosition.y > -1 && mousePosition.x > 3.7f && mousePosition.x < 6.8f)
        {
            cursor.transform.position = bossButton;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursor.transform.position = endButton;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cursor.transform.position = startButton;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cursor.transform.position = bossButton;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && cursor.transform.position == bossButton)
        {
            cursor.transform.position = startButton;
        }

        if (cursor.transform.position == startButton && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            LoadBeginning();
        }
        else if (cursor.transform.position == endButton && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            Application.Quit();
        }
        else if (cursor.transform.position == bossButton && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            LoadBoss();
        }
    }
}
