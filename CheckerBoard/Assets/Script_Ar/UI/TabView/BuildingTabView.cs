using ENTITY;
using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UILIST;
using UnityEngine;
using UnityEngine.Events;

public class BuildingTabView : TabView {

    public override void SelectTab(int index)
    {
        if (this.index != index)
        {
            Plot plot = WandererManager.Instance.wanderer.plot;
            int sort = index;
            if(index==0)
            {
                switch (plot.buildingResources[0])
                {
                    case 1:
                        sort = 3;
                        break;
                    case 2:
                        sort = 4;
                        break;
                }
            }
            for (int i = 0; i < tabPages.Length; i++)
            {
                if (i<tabButtons.Length)
                {
                    tabButtons[i].Select(i == index);
                }
                tabPages[i].gameObject.SetActive(i == sort);


            }
            if (OnTabSelect != null)
                OnTabSelect(index);
        }
    }
}
