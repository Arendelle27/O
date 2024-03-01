using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIBUILDING
{
    public class UIBuildingSprites
    {
        public List<Sprite> sprites = new List<Sprite>() { };

        public UIBuildingSprites() 
        {
            foreach (string path in PathConfig.UI_Building_Sprite_Paths)
            {
                Sprite sp = Resources.Load<Sprite>(path);
                this.sprites.Add(sp);
            }

        }
    }
}