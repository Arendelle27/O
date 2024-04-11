using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MANAGER
{
    public static class SpriteManager
    {
        public static Dictionary<string, Sprite> plotSprites=new Dictionary<string, Sprite>(); 

        public static void Load()
        {
            foreach (var plotDefine in DataManager.PlotDefines.Values)
            {
                Sprite sprite = Resources.Load<Sprite>(PathConfig.GetPlotSpritePath(plotDefine.Name.ToString()));
                if(sprite != null&&!plotSprites.ContainsKey(plotDefine.Name))
                {
                    plotSprites.Add(plotDefine.Name, sprite);
                }
            }
        }
    }
}
