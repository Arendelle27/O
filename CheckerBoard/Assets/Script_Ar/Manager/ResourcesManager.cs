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
        [SerializeField, LabelText("��Ǯ"), ReadOnly]
        public int wealth;

        [SerializeField, LabelText("������Դ"), ReadOnly]
        public List<int> buildingResources = new List<int>() {0,0,0 };

        [SerializeField, LabelText("�ж���"), ReadOnly]
        public int execution=0;

        [SerializeField, LabelText("ͨ����Դ��������"), ReadOnly]
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
                //�仯ʱ��������UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).wealthAmount.text = this.wealth.ToString();
                Debug.Log("�����仯");
            });

            this.ObserveEveryValueChanged(_ => this.buildingResources).Subscribe(_ =>
            {
                //�仯ʱ����ԴUI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).resourcesAmounts[0].text = this.buildingResources[0].ToString();
                (UIMain.Instance.uiPanels[1] as UIGamePanel).resourcesAmounts[1].text = this.buildingResources[1].ToString();
                (UIMain.Instance.uiPanels[1] as UIGamePanel).resourcesAmounts[2].text = this.buildingResources[2].ToString();
                Debug.Log("��Դ�仯");
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
                //�仯ʱ�����ж���UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).uIExecutionPanel.UpdatUIExuection(this.execution);
                Debug.Log("�ж���仯");
            });

        }

        /// <summary>
        /// ��ʼ��
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
        /// ��������
        /// </summary>
        /// <param name="variation"></param>
        public void ChangeWealth(int variation)
        {
            this.wealth += variation;
            //if(this.wealth<=0)
            //{
            //    Main.Instance.GameOver();//��Ǯ����ʱ
            //}
        }

        /// <summary>
        /// ����������Դ
        /// </summary>
        /// <param name="variations"></param>
        public void ChangeBuildingResources(int[] variations,bool isAdd)//�Ƿ���������Դ
        {
            List<int> buildingResources=new List<int>();

            int res = 0;
            for (int i = 0; i < this.buildingResources.Count; i++)
            {
                if(isAdd)//������Դ
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
        /// �����ж���
        /// </summary>
        /// <param name="variation"></param>
       public void ChangeExecution(int variation)
        {
            this.execution += variation;
        }

        /// <summary>
        /// �ж���Դ�Ƿ��㹻����
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanBuild(Building_Type type)
        {
            if (this.execution < 1)
            {
                Debug.Log("�ж�������");
                MessageManager.Instance.AddMessage(Message_Type.��е, string.Format("�ж�������"));
                return false;
            }

            int[] cost = new int[3];
            int sort = BuildingManager.BuildingTypeToIndex(type);
            switch (sort)
            {
                case 0:
                    cost = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.�Զ��ɼ����� - 1].ResourcesCost;
                    break;
                case 1:
                    cost = DataManager.BuildingScriptLists[1][(int)type - (int)Building_Type.�������� - 1].ResourcesCost;
                    break;
                case 2:
                    cost = DataManager.BuildingScriptLists[2][(int)type - (int)Building_Type.ս������ - 1].ResourcesCost;
                    break;
            }

            for (int i = 0; i < buildingResources.Count; i++)
            {
                if (buildingResources[i] < cost[i])
                {
                    Debug.LogFormat("��Դ{0}����", (Resource_Type)i);
                    MessageManager.Instance.AddMessage(Message_Type.��е, string.Format("��Դ{0}����", (Resource_Type)i));
                    return false;
                }
            }
            //������Դ
            this.ChangeBuildingResources(cost,false);
            this.ChangeExecution(-1);//����һ���ж���
            return true;
        }

        /// <summary>
        /// �ж��Ƿ�������
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
        /// �غϽ���
        /// </summary>
        public void RoundOver(int roundAmount)
        {
            this.execution = 5;


            this.execution = CapabilityManager.Instance.executionAmount;
        }
    }

}