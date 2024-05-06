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
        //当前新手指引阶段
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

        //是否处于特定新手指引阶段
        public List<bool> isGuideStage = new List<bool>(3) { false, false, false };
        //0：移动，1：移动地点 2：建造

        //新手指引面板
        public UINoviceGuidePanel uINoviceGuidePanel;

        //点击位置去下一步
        IDisposable onClickToNext;

        public void OnStart()
        {
            if (!(UIMain.Instance.uiPanels[0] as UIStartPanel).isNovicGuideToggle.isOn)//判断是否进行新手教程
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
        /// 点击去下一个步
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
        /// 新手指引阶段改变
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
                    case PlayCondition_Type.任意位置:
                        this.ClickToNext();
                        break;
                    case PlayCondition_Type.移动:
                        this.isGuideStage[0] = true;
                        break;
                    case PlayCondition_Type.移动地点:
                        this.isGuideStage[1] = true;
                        break;
                    case PlayCondition_Type.建造:
                        this.isGuideStage[2] = true;
                        break;
                }

            }
            else
            {
                UIManager.Instance.Close<UINoviceGuidePanel>();
                QuestManager.Instance.GetQuest(-1);//接受第一个任务
            }
        }
    }
}
