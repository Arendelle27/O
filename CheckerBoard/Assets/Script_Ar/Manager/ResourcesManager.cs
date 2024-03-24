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

        [SerializeField, LabelText("所有收集的建筑资源"), ReadOnly]
        public List<int> allBuildingResourcesGathered = new List<int>() { 0, 0, 0 };

        [SerializeField, LabelText("行动点"), ReadOnly]
        public int execution;

        [SerializeField, LabelText("扩展探索小队数量"),ReadOnly]
        public int levelPromptionAmount;
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
            });

            this.ObserveEveryValueChanged(_ => this.execution).Subscribe(_ =>
            {
                //变化时更新行动点UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).executionAmount.text = this.execution.ToString();
                Debug.Log("行动点变化");
            });

            this.ObserveEveryValueChanged(_ => this.levelPromptionAmount).Subscribe(_ =>
            {
                (UIMain.Instance.uiPanels[2] as UIExtendExpTeamPanel).UpdateUI(this.levelPromptionAmount);

            });
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            this.levelPromptionAmount = 0;
            this.allBuildingResourcesGathered = new List<int>() { 0, 0, 0 };
    }

        public void Restart()
        {
            this.Init();
            this.wealth = 100;
            this.buildingResources = new List<int> { 5, 5, 5 };
            this.execution = 5;
        }

        public void ReadArchive()
        {
            this.Init();
            this.wealth = ArchiveManager.archive.wealth;
            this.buildingResources = ArchiveManager.archive.buildingRes;
            this.execution = ArchiveManager.archive.execution;
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
        public void ChangeBuildingResources(int[] variations)
        {
            int i1 = this.buildingResources[0] + variations[0];
            int i2 = this.buildingResources[1] + variations[1];
            int i3 = this.buildingResources[2] + variations[2];
            this.buildingResources = new List<int>{ i1, i2, i3 };

            if (variations[0] > 0)
            {
                for (int i = 0; i < allBuildingResourcesGathered.Count; i++)
                {
                    this.allBuildingResourcesGathered[i] += variations[i];
                }
            }
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
        /// 是否有足够的行动点，是则返回true，消耗行动点
        /// </summary>
        /// <param name="excutionCost"></param>
        /// <returns></returns>
        public bool CanMove(int excutionCost)
        {
            if(this.execution>=excutionCost)
            {
                this.ChangeExecution(-excutionCost);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断资源是否足够建造
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanBuild(Building_Type type)
        {
            for (int i = 0; i < buildingResources.Count; i++)
            {
                if (buildingResources[i] < 1)
                {
                    return false;
                }
            }
            if(this.execution<1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断是否能升级
        /// </summary>
        /// <returns></returns>
        public bool CanUpgrade()
        {
            if(this.wealth>=WandererManager.Instance.wanderer.level*10)
            {
                WandererManager.Instance.Upgrade();//升级
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
        public void RoundOver()
        {
            this.execution = 5;

            this.ChangeWealth(-10*WandererManager.Instance.wanderer.level);
        }

        /// <summary>
        /// 根据获得资源总数解锁建筑
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool UnlockBuildingTypeByResource(int type,int amount)
        {
            switch(type)
            {
                case 0:
                    if (this.allBuildingResourcesGathered[0]>=amount)
                    {
                        return true;
                    }
                    break;
                case 1:
                    if (this.allBuildingResourcesGathered[1] >= amount)
                    {
                        return true;
                    }
                    break;
                case 2:
                    if (this.allBuildingResourcesGathered[2] >= amount)

                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
    }

}