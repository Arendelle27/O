using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIBUILDING
{
    public class UIBuildingSprites
    {
        public List<string> BuildingItemSpriteNames = new List<string>()
        {
            "Gathering_1",
            "Gathering_2",
            "Production_1",
            "Production_2"
         };

        public List<Sprite> sprites = new List<Sprite>() { };

        public UIBuildingSprites() 
        {
            foreach (string name in this.BuildingItemSpriteNames)
            {
                string path = PathConfig.GetBuildingItemSpritePath(name);
                Sprite sp = Resources.Load<Sprite>(path);
                this.sprites.Add(sp);
            }

        }
    }
}