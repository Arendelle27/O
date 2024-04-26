using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UniRx;
using UnityEngine;


namespace MANAGER
{
    public class MessageManager : Singleton<MessageManager>
    {
        //消息
        public class Message
        {
            public Message_Type type;
            public string message;
        }

        [SerializeField, LabelText("存储两天"), Tooltip("当前等级显示")]
        public List<List<Message>> messages = new List<List<Message>>(2)
        {
            new List<Message>(),//昨天
            new List<Message>() //今天
        };

        //public List<List<List<string>>> messages = new List<List<List<string>>>(2)
        //{
        //    new List<List<string>>(7)//昨天
        //    {
        //        new List<string>(), //指引
        //        new List<string>(), //移动
        //        new List<string>(), //机械
        //        new List<string>(), //交易
        //        new List<string>(), //对抗
        //        new List<string>(), //小队
        //        new List<string>()  //任务
        //    },
        //    new List<List<string>>(7)//今天
        //    {
        //        new List<string>(), //指引
        //        new List<string>(), //移动
        //        new List<string>(), //机械
        //        new List<string>(), //交易
        //        new List<string>(), //对抗
        //        new List<string>(), //小队
        //        new List<string>()  //任务
        //    },
        //};

        StringBuilder stringBuilder = new StringBuilder();

        public MessageManager()
        {
            this.ObserveEveryValueChanged(_ => this.stringBuilder.Length).Subscribe(_ =>
            {
                (UIMain.Instance.uiPanels[1] as UIGamePanel).uiMessage.Message = this.stringBuilder.ToString();
            });
        }

        void Init()
        {

        }

        public void ReStart()
        {
            for (int d = 0; d < this.messages.Count; d++)
            {
                this.messages[d].Clear();
            }
        }

        public void ReadArchive()
        {
            ArchiveManager.MessageManagerData messageManagerData = ArchiveManager.archive.messageManagerData;

            foreach (var message in messageManagerData.yesterdayMessages)
            {
                this.messages[0].Add(new Message() { type = message.type, message = message.message });
            }

            foreach (var message in messageManagerData.todayMessages)
            {
                this.messages[1].Add(new Message() { type = message.type, message = message.message });
            }

            this.ReFreshMessages();
        }

        /// <summary>
        /// 存储信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public void AddMessage(Message_Type type, string message)
        {
            Message m = new Message() 
            {
                type = type,
                message = message
            };

            this.messages[1].Add(m);

            StringBuilder sb = this.stringBuilder;
            sb.AppendLine(FormatMessage(1, type, message));
            this.stringBuilder = sb;
        }

        /// <summary>
        /// 全部刷新信息
        /// </summary>
        public void ReFreshMessages()
        {
            StringBuilder sb = new StringBuilder();
            for(int day=0;day<this.messages.Count;day++)
            {
                foreach (var message in this.messages[day])
                {
                    sb.AppendLine(FormatMessage(day, message.type, message.message));
                }
                if(day==0)
                {
                    string partingLine = "-------------------";
                    sb.AppendLine(partingLine);
                }
            }
            this.stringBuilder = sb;
        }

        /// <summary>
        /// 格式化信息
        /// </summary>
        /// <param name="day"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private string FormatMessage(int day, Message_Type type,string message)
        {
            string dayStr = day == 0 ? "昨天" : "今天";

            if(type==Message_Type.指引)
            {
                return string.Format("[<color=cyan>{0}</color>]{1}", dayStr,message);
            }
            else
            {
                return string.Format("[<color=cyan>{0}</color>][<color=blue>{1}</color>]{2}", dayStr, type.ToString(), message);
            }
            //switch(message.Channel)
            //{
            //    case ChatChannel.Local:
            //        return string.Format("[本地]{0}{1}", FormatFromPlayer(message), message.Message);
            //    case ChatChannel.World:
            //        return string.Format("<color=cyan>[世界]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            //    case ChatChannel.System:
            //        return string.Format("<color=yellow>[系统]{0}</color>",  message.Message);
            //    case ChatChannel.Private:
            //        return string.Format("<color=magenta>[私聊]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            //    case ChatChannel.Team:
            //        return string.Format("<color=green>[队伍]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            //    case ChatChannel.Guild:
            //        return string.Format("<color=blue>[公会]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            //}
            //return "";
        }

        //private string FormatFromPlayer(ChatMessage message)
        //{
        //    if(message.FromId==User.Instance.CurrentCharacterInfo.Id)
        //    {
        //        return "<a name=\"\" class=\"player\">[我]</a>";
        //    }
        //    else
        //    {
        //        return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId, message.FromName);
        //    }
        //}

        public void RoundOver()
        {
            this.messages[0]= this.messages[1];
            this.messages[1] = new List<Message>();

            this.ReFreshMessages();
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void GameOver()
        {
            for(int d = 0; d < this.messages.Count; d++)
            {
                for (int i = 0; i < this.messages[d].Count; i++)
                {
                    this.messages[d].Clear();
                }
            }
            this.stringBuilder.Clear();
        }
    }
}
