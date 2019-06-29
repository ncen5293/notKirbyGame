using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPersistence {
    
    public static void SaveData(PlayerHealth hp)
    {
        PlayerPrefs.SetInt("hp", hp.hp);
        PlayerPrefs.SetInt("power", hp.power);
        PlayerPrefs.SetInt("durability", hp.durability);
    }

    public static void SaveData(PlayerController pc)
    {
        PlayerPrefs.SetFloat("x", pc.transform.position.x);
        PlayerPrefs.SetFloat("y", pc.transform.position.y);
        PlayerPrefs.SetFloat("z", pc.transform.position.z);
        PlayerPrefs.SetString("name", pc.gameObject.name);
        PlayerPrefs.SetInt("weapon", pc.weapon);
        PlayerPrefs.SetInt("throwable", pc.throwable);
    }

    public static Character LoadData()
    {
        float x = PlayerPrefs.GetFloat("x");
        float y = PlayerPrefs.GetFloat("y");
        float z = PlayerPrefs.GetFloat("z");
        string name = PlayerPrefs.GetString("name");
        int hp = PlayerPrefs.GetInt("hp");
        int power = PlayerPrefs.GetInt("power");
        int weapon = PlayerPrefs.GetInt("weapon");
        int durability = PlayerPrefs.GetInt("durability");
        int throwable = PlayerPrefs.GetInt("throwable");

        Character character = new Character();
        character.Name = name;
        character.Hp = hp;
        if (character.Hp <= 0)
        {
            character.Hp = 1;
        }
        character.Power = power;
        character.Weapon = weapon;
        character.Durability = durability;
        character.Throwable = throwable;
        character.Position = new Vector3(x, y, z);

        return character;
    }
}
