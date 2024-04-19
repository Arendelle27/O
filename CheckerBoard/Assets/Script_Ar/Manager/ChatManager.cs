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
        //当前聊天Id
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

        //当前聊天内容Id
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
        //聊天窗口
        public UIChatWindow uiChatWindow;

        //点击位置去下一步
        IDisposable onClickToNext;

        /// <summary>
        /// 显示聊天窗口
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
        /// 更新聊天内容
        /// </summary>
        /// <param name="chatContentId"></param>
        public void UpdateChatWindow(int chatContentSort)//-1为对话内容，0为选项1,1为选项2,2为选项3
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
            else//对话结束
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
        /// 点击去下一个步
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
