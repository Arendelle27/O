using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc :Entity
{
    //[SerializeField, LabelText("npcͼ����ʾ"), Tooltip("����npcͼ����ʾ")]
    //public Sprite npcSprite;
    [SerializeField, LabelText("npc����"), ReadOnly]
    public NPCDefine npcDefine;
    [SerializeField, LabelText("npcλ��"), ReadOnly]
    public Vector2Int pos;
    private void Start()
    {
        this.LookToCamera();
    }

    public void SetInfo(int npcDefinId,Plot plot)
    {
        this.npcDefine = DataManager.NPCDefines[npcDefinId];
        this.SR.sprite = SpriteManager.npcSprites[this.npcDefine.Name];
        this.pos = plot.pos;
        this.transform.position = plot.transform.position + new Vector3(-0.3f, ParameterConfig.entityForward, ParameterConfig.entityHigh);
        this.gameObject.SetActive(true);
    }

    public void ChatWithWander()
    {
        ChatManager.Instance.ChatWithNpc(this.npcDefine.Name);
        NpcManager.Instance.NPCAppearUnlock(2, this.npcDefine.Id);
    }
}
