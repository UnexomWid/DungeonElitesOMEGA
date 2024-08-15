using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopsModifySetting : MonoBehaviour
{
    public int index;

    public void ModifySetting()
    {
        switch(index)
        {
            case 1:
                ShopsSettings.dungeonIndex = int.Parse(GetComponent<InputField>().text);
                break;
            case 2:
                ShopsSettings.dungeonLevel = int.Parse(GetComponent<InputField>().text);
                break;
            case 3:
                ShopsSettings.characterSettings[0].level = int.Parse(GetComponent<InputField>().text);
                break;
            case 4:
                ShopsSettings.characterSettings[0].a1level = int.Parse(GetComponent<InputField>().text);
                break;
            case 5:
                ShopsSettings.characterSettings[0].a2level = int.Parse(GetComponent<InputField>().text);
                break;
            case 6:
                ShopsSettings.characterSettings[0].a3level = int.Parse(GetComponent<InputField>().text);
                break;
            case 7:
                ShopsSettings.characterSettings[1].level = int.Parse(GetComponent<InputField>().text);
                break;
            case 8:
                ShopsSettings.characterSettings[1].a1level = int.Parse(GetComponent<InputField>().text);
                break;
            case 9:
                ShopsSettings.characterSettings[1].a2level = int.Parse(GetComponent<InputField>().text);
                break;
            case 10:
                ShopsSettings.characterSettings[1].a3level = int.Parse(GetComponent<InputField>().text);
                break;
            case 11:
                ShopsSettings.characterSettings[2].level = int.Parse(GetComponent<InputField>().text);
                break;
            case 12:
                ShopsSettings.characterSettings[2].a1level = int.Parse(GetComponent<InputField>().text);
                break;
            case 13:
                ShopsSettings.characterSettings[2].a2level = int.Parse(GetComponent<InputField>().text);
                break;
            case 14:
                ShopsSettings.characterSettings[2].a3level = int.Parse(GetComponent<InputField>().text);
                break;
            case 15:
                ShopsSettings.characterSettings[3].level = int.Parse(GetComponent<InputField>().text);
                break;
            case 16:
                ShopsSettings.characterSettings[3].a1level = int.Parse(GetComponent<InputField>().text);
                break;
            case 17:
                ShopsSettings.characterSettings[3].a2level = int.Parse(GetComponent<InputField>().text);
                break;
            case 18:
                ShopsSettings.characterSettings[3].a3level = int.Parse(GetComponent<InputField>().text);
                break;
            case 19:
                ShopsSettings.characterSettings[4].level = int.Parse(GetComponent<InputField>().text);
                break;
            case 20:
                ShopsSettings.characterSettings[4].a1level = int.Parse(GetComponent<InputField>().text);
                break;
            case 21:
                ShopsSettings.characterSettings[4].a2level = int.Parse(GetComponent<InputField>().text);
                break;
            case 22:
                ShopsSettings.characterSettings[4].a3level = int.Parse(GetComponent<InputField>().text);
                break;
            case 23:
                ShopsSettings.items[0] = int.Parse(GetComponent<InputField>().text);
                break;
            case 24:
                ShopsSettings.items[1] = int.Parse(GetComponent<InputField>().text);
                break;
            case 25:
                ShopsSettings.items[2] = int.Parse(GetComponent<InputField>().text);
                break;
            case 26:
                ShopsSettings.items[3] = int.Parse(GetComponent<InputField>().text);
                break;
            case 27:
                ShopsSettings.items[4] = int.Parse(GetComponent<InputField>().text);
                break;
            case 28:
                ShopsSettings.items[5] = int.Parse(GetComponent<InputField>().text);
                break;
            case 29:
                ShopsSettings.items[6] = int.Parse(GetComponent<InputField>().text);
                break;
            case 30:
                ShopsSettings.items[7] = int.Parse(GetComponent<InputField>().text);
                break;
            case 31:
                ShopsSettings.items[8] = int.Parse(GetComponent<InputField>().text);
                break;
            case 32:
                ShopsSettings.items[9] = int.Parse(GetComponent<InputField>().text);
                break;
            case 33:
                ShopsSettings.characterSettings[0].hp = int.Parse(GetComponent<InputField>().text);
                break;
            case 34:
                ShopsSettings.characterSettings[0].reg = int.Parse(GetComponent<InputField>().text);
                break;
            case 35:
                ShopsSettings.characterSettings[0].spd = int.Parse(GetComponent<InputField>().text);
                break;
            case 36:
                ShopsSettings.characterSettings[0].dex = int.Parse(GetComponent<InputField>().text);
                break;
            case 37:
                ShopsSettings.characterSettings[0].str = int.Parse(GetComponent<InputField>().text);
                break;
            case 38:
                ShopsSettings.characterSettings[0].cdr = int.Parse(GetComponent<InputField>().text);
                break;
            case 39:
                ShopsSettings.characterSettings[1].hp = int.Parse(GetComponent<InputField>().text);
                break;
            case 40:
                ShopsSettings.characterSettings[1].reg = int.Parse(GetComponent<InputField>().text);
                break;
            case 41:
                ShopsSettings.characterSettings[1].spd = int.Parse(GetComponent<InputField>().text);
                break;
            case 42:
                ShopsSettings.characterSettings[1].dex = int.Parse(GetComponent<InputField>().text);
                break;
            case 43:
                ShopsSettings.characterSettings[1].str = int.Parse(GetComponent<InputField>().text);
                break;
            case 44:
                ShopsSettings.characterSettings[1].cdr = int.Parse(GetComponent<InputField>().text);
                break;
            case 45:
                ShopsSettings.characterSettings[2].hp = int.Parse(GetComponent<InputField>().text);
                break;
            case 46:
                ShopsSettings.characterSettings[2].reg = int.Parse(GetComponent<InputField>().text);
                break;
            case 47:
                ShopsSettings.characterSettings[2].spd = int.Parse(GetComponent<InputField>().text);
                break;
            case 48:
                ShopsSettings.characterSettings[2].dex = int.Parse(GetComponent<InputField>().text);
                break;
            case 49:
                ShopsSettings.characterSettings[2].str = int.Parse(GetComponent<InputField>().text);
                break;
            case 50:
                ShopsSettings.characterSettings[2].cdr = int.Parse(GetComponent<InputField>().text);
                break;
            case 51:
                ShopsSettings.characterSettings[3].hp = int.Parse(GetComponent<InputField>().text);
                break;
            case 52:
                ShopsSettings.characterSettings[3].reg = int.Parse(GetComponent<InputField>().text);
                break;
            case 53:
                ShopsSettings.characterSettings[3].spd = int.Parse(GetComponent<InputField>().text);
                break;
            case 54:
                ShopsSettings.characterSettings[3].dex = int.Parse(GetComponent<InputField>().text);
                break;
            case 55:
                ShopsSettings.characterSettings[3].str = int.Parse(GetComponent<InputField>().text);
                break;
            case 56:
                ShopsSettings.characterSettings[3].cdr = int.Parse(GetComponent<InputField>().text);
                break;
            case 57:
                ShopsSettings.characterSettings[4].hp = int.Parse(GetComponent<InputField>().text);
                break;
            case 58:
                ShopsSettings.characterSettings[4].reg = int.Parse(GetComponent<InputField>().text);
                break;
            case 59:
                ShopsSettings.characterSettings[4].spd = int.Parse(GetComponent<InputField>().text);
                break;
            case 60:
                ShopsSettings.characterSettings[4].dex = int.Parse(GetComponent<InputField>().text);
                break;
            case 61:
                ShopsSettings.characterSettings[4].str = int.Parse(GetComponent<InputField>().text);
                break;
            case 62:
                ShopsSettings.characterSettings[4].cdr = int.Parse(GetComponent<InputField>().text);
                break;
            case 63:
                ShopsSettings.coins = int.Parse(GetComponent<InputField>().text);
                break;
        }
    }
}
