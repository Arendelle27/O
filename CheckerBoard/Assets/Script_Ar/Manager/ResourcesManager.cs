using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class ResourcesManager :Singleton<ResourcesManager>
    {
        [SerializeField, LabelText("金钱"), ReadOnly]
        public int wealth;

        [SerializeField, LabelText("建筑资源"), ReadOnly]
        public List<int> buildingResources = new List<int>() {0,0,0 };

        [SerializeField, LabelText("行动点"), ReadOnly]
        public int execution=0;

        [SerializeField, LabelText("通过资源解锁建筑"), ReadOnly]
        public List<Subject<int>> unlockByResouces = new List<Subject<int>>() 
        {
            new Subject<int>(),
            new Subject<int>(),
            new Subject<int>()
        };

        public ResourcesManager()
        {
            this.ObserveEveryValueChanged(_ => this.wealth).Subscribe(_ =>
            {
                //变化时更新能量UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).wealthAmount.text = this.wealth.ToString();
                Debug.Log("能量变化");
            });

            this.ObserveEveryValueChanged(_ => this.buildingResources).Subscribe(_ =>
            {
                //变化时更资源UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).resourcesAmounts[0].text = this.buildingResources[0].ToString();
                (UIMain.Instance.uiPanels[1] as UIGamePanel).resourcesAmounts[1].text = this.buildingResources[1].ToString();
                (UIMain.Instance.uiPanels[1] as UIGamePanel).resourcesAmounts[2].text = this.buildingResources[2].ToString();
                Debug.Log("资源变化");
                for (int i = 0; i < this.buildingResources.Count; i++)
                {
                    if (this.unlockByResouces[i] != null)
                    {
                        this.unlockByResouces[i].OnNext(this.buildingResources[i]);
                    }
                }
            });

            this.ObserveEveryValueChanged(_ => this.execution).Subscribe(_ =>
            {
                //变化时更新行动点UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).uIExecutionPanel.UpdatUIExuection(this.execution);
                Debug.Log("行动点变化");
            });

        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
        }

        public void Restart()
        {
            this.Init();
            this.wealth = 50;
            this.execution = CapabilityManager.Instance.executionAmount;
        }

        public void ReadArchive()
        {
            this.Init();
            ArchiveManager.ResourcesManagerData resourcesManagerData = ArchiveManager.archive.resourcesManagerData;
            this.wealth = resourcesManagerData.wealth;
            this.buildingResources = resourcesManagerData.buildingResources;
            this.execution = resourcesManagerData.execution;

        }

        public void GameOver()
        {
            this.wealth = 0;
            this.buildingResources = new List<int> { 0, 0, 0 };
            this.execution = 0;
        }

        /// <summary>
        /// 增减能量
        /// </summary>
        /// <param name="variation"></param>
        public void ChangeWealth(int variation)
        {
            this.wealth += variation;
            //if(this.wealth<=0)
            //{
            //    Main.Instance.GameOver();//金钱不足时
            //}
        }

        /// <summary>
        /// 增减建筑资源
        /// </summary>
        /// <param name="variations"></param>
        public void ChangeBuildingResources(int[] variations,bool isAdd)//是否是增加资源
        {
            List<int> buildingResources=new List<int>();

            int res = 0;
            for (int i = 0; i < this.buildingResources.Count; i++)
            {
                if(isAdd)//增加资源
                {
                    res = this.buildingResources[i] + variations[i];
                }
                else
                {
                    res = this.buildingResources[i] - variations[i];
                }
                buildingResources.Add(res);
            }

            this.buildingResources = buildingResources;

            //if (isAdd)
            //{
            //    for (int i = 0; i < allBuildingResourcesGathered.Count; i++)
            //    {
            //        this.allBuildingResourcesGathered[i] += variations[i];
            //    }
            //}
        }

        /// <summary>
        /// 增减行动点
        /// </summary>
        /// <param name="variation"></param>
       public void ChangeExecution(int variation)
        {
            this.execution += variation;
        }

        /// <summary>
        /// 判断资源是否足够建造
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanBuild(Building_Type type)
        {
            if (this.execution < 1)
            {
                Debug.Log("行动力不足");
                MessageManager.Instance.AddMessage(Message_Type.机械, string.Format("行动力不足"));
                return false;
            }

            int[] cost = new int[3];
            int sort = BuildingManager.BuildingTypeToIndex(type);
            switch (sort)
            {
                case 0:
                    cost = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.自动采集建筑 - 1].ResourcesCost;
                    break;
                case 1:
                    cost = DataManager.BuildingScriptLists[1][(int)type - (int)Building_Type.生产建筑 - 1].ResourcesCost;
                    break;
                case 2:
                    cost = DataManager.BuildingScriptLists[2][(int)type - (int)Building_Type.战斗建筑 - 1].ResourcesCost;
                    break;
            }

            for (int i = 0; i < buildingResources.Count; i++)
            {
                if (buildingResources[i] < cost[i])
                {
                    Debug.LogFormat("资源{0}不足", (Resource_Type)i);
                    MessageManager.Instance.AddMessage(Message_Type.机械, string.Format("资源{0}不足", (Resource_Type)i));
                    return false;
                }
            }
            //消耗资源
            this.ChangeBuildingResources(cost,false);
            this.ChangeExecution(-1);//消耗一点行动力
            return true;
        }

        /// <summary>
        /// 判断是否能升级
        /// </summary>
        /// <returns></returns>
        public bool CanBuyUpgradePoint(int cost)
        {
            if(this.wealth>=cost)
            {
                this.ChangeWealth(-cost);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 回合结束
        /// </summary>
        public void RoundOver(int roundAmount)
        {
            this.execution = 5;


            this.execution = CapabilityManager.Instance.executionAmount;
        }
    }

}