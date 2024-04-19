using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class ChatManager : Singleton<ChatManager>
    {
        //��ǰ����Id
        int curChatId;
        public int CurChatId
        {
            get { return curChatId; }
            set 
            { 
                curChatId = value; 
                this.ShowChatWindow();
            }
        }

        //��ǰ��������Id
        int curChatContentId;
        public int CurChatContentId
        {
            get { return curChatContentId; }
            set
            {
                curChatContentId = value; 
                this.UpdateChatWindow(-1);
            }
        }
        //���촰��
        public UIChatWindow uiChatWindow;

        //���λ��ȥ��һ��
        IDisposable onClickToNext;

        /// <summary>
        /// ��ʾ���촰��
        /// </summary>
        void ShowChatWindow()
        { 
            if(DataManager.ChatDefines.ContainsKey(curChatId))
            {
                UIChatWindow uiChatWindow = UIManager.Instance.Show<UIChatWindow>();
                if (this.uiChatWindow == null)
                {
                    this.uiChatWindow = uiChatWindow;
                }
                this.CurChatContentId = 0;
            }
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="chatContentId"></param>
        public void UpdateChatWindow(int chatContentSort)//-1Ϊ�Ի����ݣ�0Ϊѡ��1,1Ϊѡ��2,2Ϊѡ��3
        {
            if (DataManager.ChatDefines[curChatId].ContainsKey(CurChatContentId))
            {
                ChatDefine chatDefine = DataManager.ChatDefines[curChatId][curChatContentId];

                switch(chatContentSort)
                {
                    case 0:
                        if(chatDefine.Answer1 == null)
                        {
                            this.CurChatContentId++;
                            return;
                        }
                        break;
                    case 1:
                        if (chatDefine.Answer2 == null)
                        {
                            this.CurChatContentId++;
                            return;
                        }
                        break;
                    case 2:
                        if (chatDefine.Answer3 == null)
                        {
                            this.CurChatContentId++;
                            return;
                        }
                        break;
                }

                this.uiChatWindow.SetInfo(chatDefine, chatContentSort);
                if (chatContentSort==-1&&chatDefine.Option1!=null)
                {
                    return;
                }
                this.ClickToNext();
            }
            else//�Ի�����
            {
                UIManager.Instance.Close<UIChatWindow>();
                switch (curChatId)
                {
                    case 0:
                        NoviceGuideManager.Instance.OnStart();
                        break;
                }
            }
        }

        /// <summary>
        /// ���ȥ��һ����
        /// </summary>
        void ClickToNext()
        {

                this.onClickToNext = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
                {
                    this.onClickToNext.Dispose();
                    this.CurChatContentId++;
                });
        }
    }
}
