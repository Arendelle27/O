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

        [SerializeField, LabelText("�����ռ��Ľ�����Դ"), ReadOnly]
        public List<int> allBuildingResourcesGathered = new List<int>() { 0, 0, 0 };

        [SerializeField, LabelText("�ж���"), ReadOnly]
        public int execution;

        [SerializeField, LabelText("��չ̽��С������"),ReadOnly]
        public int levelPromptionAmount;
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
            });

            this.ObserveEveryValueChanged(_ => this.execution).Subscribe(_ =>
            {
                //�仯ʱ�����ж���UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).executionAmount.text = this.execution.ToString();
                Debug.Log("�ж���仯");
            });

            this.ObserveEveryValueChanged(_ => this.levelPromptionAmount).Subscribe(_ =>
            {
                (UIMain.Instance.uiPanels[2] as UIExtendExpTeamPanel).UpdateUI(this.levelPromptionAmount);

            });
        }

        /// <summary>
        /// ��ʼ��
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
        /// �����ж���
        /// </summary>
        /// <param name="variation"></param>
       public void ChangeExecution(int variation)
        {
            this.execution += variation;
        }

        /// <summary>
        /// �Ƿ����㹻���ж��㣬���򷵻�true�������ж���
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
        /// �ж���Դ�Ƿ��㹻����
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
        /// �ж��Ƿ�������
        /// </summary>
        /// <returns></returns>
        public bool CanUpgrade()
        {
            if(this.wealth>=WandererManager.Instance.wanderer.level*10)
            {
                WandererManager.Instance.Upgrade();//����
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
        public void RoundOver()
        {
            this.execution = 5;

            this.ChangeWealth(-10*WandererManager.Instance.wanderer.level);
        }

        /// <summary>
        /// ���ݻ����Դ������������
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