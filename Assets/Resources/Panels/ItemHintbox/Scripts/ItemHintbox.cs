using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHintbox : Hintbox
{
    [SerializeField] private Image icon;

    public void SetIcon(Sprite sprite) {
        icon?.SetSprite(sprite);
    }
}
