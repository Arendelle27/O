using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MANAGER
{
    public class BuildingManager : MonoSingleton<BuildingManager>
    {

        public List<Building_Type> GatheringTypes = new List<Building_Type>() {
            Building_Type.�Զ��ɼ�����_1,
            Building_Type.�Զ��ɼ�����_2,
        };

        public List<Building_Type> ProductionTypes = new List<Building_Type>() {
            Building_Type.��������_1,
            Building_Type.��������_2,
        };
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

