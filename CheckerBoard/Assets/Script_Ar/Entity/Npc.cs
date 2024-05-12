using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc :Entity
{
    //[SerializeField, LabelText("npc图像显示"), Tooltip("放入npc图像显示")]
    //public Sprite npcSprite;
    [SerializeField, LabelText("npc数据"), ReadOnly]
    public NPCDefine npcDefine;
    [SerializeField, LabelText("npc位置"), ReadOnly]
    public Vector2Int pos;
    private void Start()
    {
        this.LookToCamera();
    }

    public void SetInfo(int npcDefinId,Plot plot)
    {
        this.npcDefine = DataManager.NPCDefines[npcDefinId];
        this.ShowSwitch(false);
        this.pos = plot.pos;
        this.transform.position = plot.transform.position + new Vector3(-0.2f, ParameterConfig.entityForward, ParameterConfig.entityHigh);
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// 与流浪者对话
    /// </summary>
    public void ChatWithWander()
    {
        ChatManager.Instance.ChatWithNpc(this.npcDefine.Id);
        this.ShowSwitch(false);

        NpcManager.Instance.NPCAppearUnlock(2, this.npcDefine.Id);
        QuestManager.Instance.QuestEnd(0, this.npcDefine.Id);
    }

    /// <summary>
    /// Npc可聊天状态切换
    /// </summary>
    /// <param name="canChat"></param>
    public void ShowSwitch(bool canChat)
    {
        this.SR.sprite = canChat ? SpriteManager.npcChatSprites[this.npcDefine.Name] : SpriteManager.npcNormalSprites[this.npcDefine.Name];
    }
}
