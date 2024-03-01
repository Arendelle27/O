using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class Building : Entity
    {
        [SerializeField, LabelText("建筑的类型"), ReadOnly]
        public Building_Type type;

        [SerializeField, LabelText("建筑的Id"), ReadOnly]
        public int id;

        public void SetInfo(int id,Building_Type type)
        {
            this.id = id;
            this.type = type;
        }

        private void Update()
        {
            this.LookToCamera();
        }
    }
}
