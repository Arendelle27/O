using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Sprite activeImage;
    public Sprite normalImage;

    public TabView tabView;

    public int tabIndex;
    public bool selected = false;

    protected Image tabImage;

    void Awake()
    {
        tabImage = this.GetComponent<Image>();
        //normalImage = tabImage.sprite;

        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public virtual void Select(bool select)
    {
        if (!this.gameObject.activeSelf)
            return;
        if(this.tabImage!=null)
        {
            tabImage.overrideSprite = select ? activeImage : normalImage;
        }
    }

    public void OnClick()
    {
        this.tabView?.SelectTab(this.tabIndex);
    }
}