using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopsSettings : MonoBehaviour
{
    [SerializeField] GameObject settings;

    private void Start()
    {
        ResetSettings();
    }

    public void Settings()
    {
        settings.SetActive(!settings.activeInHierarchy);
    }

    public static void ResetSettings()
    {
        dungeonLevel = 0;
        dungeonIndex = -1;
        characterSettings = new CharacterSetting[5];
        for (int i = 0; i <= 4; i++)
            characterSettings[i] = new CharacterSetting();

        items = new int[10];

        foreach(CharacterSetting setting in characterSettings)
        {
            setting.level = 1;
            setting.a1level = 1;
            setting.a2level = 1;
            setting.a3level = 1;
            setting.hp = 0;
            setting.reg = 0;
            setting.spd = 0;
            setting.dex = 0;
            setting.str = 0;
            setting.cdr = 0;
        }
        for (int i = 0; i < items.Length; i++)
            items[i] = -1;
        coins = 0;
    }

    public static int dungeonLevel;
    public static int dungeonIndex;
    public static int coins;
    public static CharacterSetting[] characterSettings;
    public static int[] items;
    
}

public class CharacterSetting
{
    public int level;
    public int a1level;
    public int a2level;
    public int a3level;
    public int hp;
    public int reg;
    public int spd;
    public int dex;
    public int str;
    public int cdr;
}
