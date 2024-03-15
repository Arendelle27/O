using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class Building : Entity
    {
        [SerializeField, LabelText("������λ��"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("����������"), ReadOnly]
        public Building_Type type;

        public void SetInfo(Vector2Int pos,Building_Type type)
        {
            this.pos = pos;
            this.type = type;
        }

        private void Update()
        {
            this.LookToCamera();
        }
    }
}
