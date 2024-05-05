using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc :Entity
{
    [SerializeField, LabelText("npcÊý¾Ý"), ReadOnly]
    public NPCDefine npcDefine;
    [SerializeField, LabelText("npcÎ»ÖÃ"), ReadOnly]
    public Vector2Int pos;
    private void Start()
    {
        this.LookToCamera();
    }

    public void SetInfo(int npcDefinId,Plot plot)
    {
        this.npcDefine = DataManager.NPCDefines[npcDefinId];
        this.pos = plot.pos;
        this.transform.position = plot.transform.position + new Vector3(0, 0, ParameterConfig.entityHigh);
        this.gameObject.SetActive(true);
    }

    public void ChatWithWander()
    {
        ChatManager.Instance.ChatWithNpc(this.npcDefine.Name);
        NpcManager.Instance.NPCAppearUnlock(2, this.npcDefine.Id);
    }
}
