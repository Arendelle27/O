using System.Collections;
using System.Collections.Generic;
using UILIST;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour {

    public TabButton[] tabButtons;
    public ListView[] tabPages;

    public UnityAction<int> OnTabSelect;

    public int index = -1;
    IEnumerator Start () {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;
            tabButtons[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame();

        this.SelectTab(1);
    }

    void OnEnable()
    {
        //for (int i = 0; i < tabPages.Length; i++)
        //{
        //    if (tabPages[i].items.Count > 0)
        //    {
        //        tabButtons[i].OnClick();
        //        break;
        //    }
        //}
    }

    public virtual void SelectTab(int index)
    {
        if (this.index != index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index);
                if (i < tabPages.Length)
                {
                    tabPages[i].gameObject.SetActive(i == index);
                }
            }
            if (OnTabSelect != null)
                OnTabSelect(index);
        }
    }
}
