using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class DataManager :MonoSingleton<DataManager>
    {
        [SerializeField, LabelText("��Ǯ"), ReadOnly]
        public int wealth;

        [SerializeField, LabelText("������Դ"), ReadOnly]
        public int[] buildingResources = new int[3];

        [SerializeField, LabelText("�ж���"), ReadOnly]
        public int execution;

        [SerializeField, LabelText("��չ̽��С������"),ReadOnly]
        public int levelPromptionAmount;
        public void Start()
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
                //�仯ʱ�����ж���UI
                (UIMain.Instance.uiPanels[2] as UIExtendExpTeamPanel).UpdateUI();

            });
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init()
        {
            this.wealth = 900;
            this.buildingResources = new int[3] { 5, 5, 5 };
            this.execution = 5;
            this.levelPromptionAmount = 0;
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
            this.buildingResources = new int[3] { i1, i2, i3 };


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
            for (int i = 0; i < buildingResources.Length; i++)
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

        public void RoundOver()
        {
            this.execution = 5;
        }
    }

}