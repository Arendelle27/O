using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class NoviceGuideManager : Singleton<NoviceGuideManager>
    {
        //��ǰ����ָ���׶�
        int noviceGuideStage = 0;

        public int NoviceGuideStage
        {
            get
            {
                return this.noviceGuideStage;
            }
            set
            {
                this.noviceGuideStage = value;
                this.OnNoviceGuideStageChange();
            }
        }

        //�Ƿ����ض�����ָ���׶�
        public List<bool> isGuideStage = new List<bool>(3) { false, false, false };
        //0���ƶ���1���ƶ��ص� 2������

        //����ָ�����
        public UINoviceGuidePanel uINoviceGuidePanel;

        //���λ��ȥ��һ��
        IDisposable onClickToNext;

        public void OnStart()
        {
            if (!(UIMain.Instance.uiPanels[0] as UIStartPanel).isNovicGuideToggle.isOn)//�ж��Ƿ�������ֽ̳�
            {
                return;
            }
            if (this.uINoviceGuidePanel == null)
            {
                this.uINoviceGuidePanel = UIManager.Instance.Show<UINoviceGuidePanel>();
            }

            this.NoviceGuideStage = 0;
        }

        /// <summary>
        /// ���ȥ��һ����
        /// </summary>
        void ClickToNext()
        {
            this.onClickToNext = Observable
                .EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ =>
                {
                    this.onClickToNext.Dispose();
                    this.NoviceGuideStage++;
                });
        }

        /// <summary>
        /// ����ָ���׶θı�
        /// </summary>
        void OnNoviceGuideStageChange()
        {
            if (DataManager.NoviceGuideDefines.ContainsKey(this.NoviceGuideStage))
            {
                NoviceGuideDefine noviceGuideDefine = DataManager.NoviceGuideDefines[this.NoviceGuideStage];
                this.uINoviceGuidePanel.SetInfo(noviceGuideDefine);
                this.isGuideStage = new List<bool>(3) { false, false, false };
                switch (noviceGuideDefine.PlayCondition)
                {
                    case PlayCondition_Type.����λ��:
                        this.ClickToNext();
                        break;
                    case PlayCondition_Type.�ƶ�:
                        this.isGuideStage[0] = true;
                        break;
                    case PlayCondition_Type.�ƶ��ص�:
                        this.isGuideStage[1] = true;
                        break;
                    case PlayCondition_Type.����:
                        this.isGuideStage[2] = true;
                        break;
                }

            }
            else
            {
                UIManager.Instance.Close<UINoviceGuidePanel>();
                QuestManager.Instance.GetQuest(-1);//���ܵ�һ������
            }
        }
    }
}
