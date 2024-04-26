using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransactionSwitchButton : TabButton
{
    public Image textImage;
    public override void Select(bool select)
    {
        if (!this.gameObject.activeSelf)
            return;
        if (this.tabImage != null)
        {
            tabImage.color= select ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0f);
            textImage.overrideSprite = select ? activeImage : normalImage;
        }
    }
}
