using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MANAGER
{
    public class BuildingManager : MonoSingleton<BuildingManager>
    {

        public List<Building_Type> GatheringTypes = new List<Building_Type>() {
            Building_Type.自动采集建筑_1,
            Building_Type.自动采集建筑_2,
        };

        public List<Building_Type> ProductionTypes = new List<Building_Type>() {
            Building_Type.生产建筑_1,
            Building_Type.生产建筑_2,
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

