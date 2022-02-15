using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int tileID = -1;

    public Image tileIcon;

    public void SetIcon(Sprite icon)
    {
        tileIcon.sprite = icon;
    }
}
