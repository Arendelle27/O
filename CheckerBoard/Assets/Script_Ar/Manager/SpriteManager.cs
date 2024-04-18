using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MANAGER
{
    public static class SpriteManager
    {
        public static Dictionary<Resource_Type,Sprite> buildingResourceSprites=new Dictionary<Resource_Type, Sprite>();

        public static Dictionary<Prop_Type,Sprite> propSprites=new Dictionary<Prop_Type, Sprite>();

        public static Dictionary<string, Sprite> plotSprites=new Dictionary<string, Sprite>(); 

        public static void Load()
        {
            for(int i=0;i<(int)Resource_Type.建筑资源;i++)
            {
                Sprite sprite = Resources.Load<Sprite>(PathConfig.GetBuildingResourceSpritePath(((Resource_Type)i).ToString()));
                if(sprite != null&&!buildingResourceSprites.ContainsKey((Resource_Type)i))
                {
                    buildingResourceSprites.Add((Resource_Type)i, sprite);
                }
            }

            for(int i=0;i<(int)Prop_Type.道具;i++)
            {
                Sprite sprite = Resources.Load<Sprite>(PathConfig.GetPropSpritePath(((Prop_Type)i).ToString()));
                if(sprite != null&&!propSprites.ContainsKey((Prop_Type)i))
                {
                    propSprites.Add((Prop_Type)i, sprite);
                }
            }

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
