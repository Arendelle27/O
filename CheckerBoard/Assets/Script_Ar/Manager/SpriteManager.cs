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

        public static Dictionary<HeadPortrait_Type,Sprite> headPortraitSprites =new Dictionary<HeadPortrait_Type, Sprite>();

        public static Dictionary<Npc_Name, Sprite> npcNormalSprites = new Dictionary<Npc_Name, Sprite>();

        public static Dictionary<Npc_Name, Sprite> npcChatSprites = new Dictionary<Npc_Name, Sprite>();

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

            for (int i = 0; i < (int)HeadPortrait_Type.头像; i++)
            {
                Sprite sprite = Resources.Load<Sprite>(PathConfig.GetHeadPortraitSpritePath(((HeadPortrait_Type)i).ToString()));
                if (sprite != null && !headPortraitSprites.ContainsKey((HeadPortrait_Type)i))
                {
                    headPortraitSprites.Add((HeadPortrait_Type)i, sprite);
                }
            }

            for (int i = 0; i < (int)Npc_Name.Npc; i++)
            {
                Sprite sprite = Resources.Load<Sprite>(PathConfig.GetNpcNormalSpritePath(((Npc_Name)i).ToString()));
                if (sprite != null && !npcNormalSprites.ContainsKey((Npc_Name)i))
                {
                    npcNormalSprites.Add((Npc_Name)i, sprite);
                }
            }

            for (int i = 0; i < (int)Npc_Name.Npc; i++)
            {
                Sprite sprite = Resources.Load<Sprite>(PathConfig.GetNpcChatSpritePath(((Npc_Name)i).ToString()));
                if (sprite != null && !npcChatSprites.ContainsKey((Npc_Name)i))
                {
                    npcChatSprites.Add((Npc_Name)i, sprite);
                }
            }
        }
    }
}
