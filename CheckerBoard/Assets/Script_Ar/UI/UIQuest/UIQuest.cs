using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    [SerializeField, LabelText("任务类型"), Tooltip("放入任务类型文本")]
    public Text questType;
    [SerializeField, LabelText("开关按键"), Tooltip("放入开关按键")]
    public Toggle switchToggle;
    [SerializeField, LabelText("任务名称"), Tooltip("放入任务名称文本")]
    public Text questName;
    [SerializeField, LabelText("任务内容"), Tooltip("放入任务内容文本")]
    public Text questContent;

    public Image toggleImage;
    public Sprite openSprite;
    public Sprite closeSprite;
    

    [SerializeField, LabelText("自适应的窗口"), Tooltip("放入需要只适应的窗口")]
    public List<RectTransform> rectTransforms = new List<RectTransform>();

    private void Start()
    {
        this.switchToggle.OnPointerClickAsObservable().Subscribe(_ =>
        {
            this.questName.gameObject.SetActive(switchToggle.isOn);
            this.questContent.gameObject.SetActive(switchToggle.isOn);

            this.toggleImage.sprite = switchToggle.isOn ? openSprite : closeSprite;

            foreach (var rectTransform in rectTransforms)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        });
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    public void SetInfo(QuestDefine questDefine)
    {
        //if(questDefine.IsMain)
        //{
        //    this.questType.text = "主";
        //}
        //else
        //{
        //    this.questType.text = "支";
        //}
        this.switchToggle.isOn = true;
        this.questName.text = questDefine.QuestName;
        List<string> questContents = new List<string>(3) {"","","" };//0为回合数，1为空间币数，2为npc
        if(questDefine.RoundCondition!=0)
        {
            questContents[0] =string.Format( "在第{0}回合结束之前",QuestManager.Instance.curMainQuestRound);
        }
        if(questDefine.CurrencyCondition!=0)
        {
            questContents[1] = string.Format("赚钱{0}空间币", questDefine.CurrencyCondition);
        }
        //if(questDefine.AimNpc != Npc_Name.无)
        //{
        //    questContents[2] = string.Format("交给{0}", questDefine.NpcCondition);
        //}

        this.questContent.text = string.Format("{0}{1}{2}", questContents[0], questContents[1], questContents[2]);

        foreach (var rectTransform in rectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
