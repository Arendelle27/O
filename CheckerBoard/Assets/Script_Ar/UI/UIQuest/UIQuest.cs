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
    [SerializeField, LabelText("��������"), Tooltip("�������������ı�")]
    public Text questType;
    [SerializeField, LabelText("���ذ���"), Tooltip("���뿪�ذ���")]
    public Toggle switchToggle;
    [SerializeField, LabelText("��������"), Tooltip("�������������ı�")]
    public Text questName;
    [SerializeField, LabelText("��������"), Tooltip("�������������ı�")]
    public Text questContent;

    public Image toggleImage;
    public Sprite openSprite;
    public Sprite closeSprite;
    

    [SerializeField, LabelText("����Ӧ�Ĵ���"), Tooltip("������Ҫֻ��Ӧ�Ĵ���")]
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
    /// ������Ϣ
    /// </summary>
    public void SetInfo(QuestDefine questDefine)
    {
        //if(questDefine.IsMain)
        //{
        //    this.questType.text = "��";
        //}
        //else
        //{
        //    this.questType.text = "֧";
        //}
        this.switchToggle.isOn = true;
        this.questName.text = questDefine.QuestName;
        List<string> questContents = new List<string>(3) {"","","" };//0Ϊ�غ�����1Ϊ�ռ������2Ϊnpc
        if(questDefine.RoundCondition!=0)
        {
            questContents[0] =string.Format( "�ڵ�{0}�غϽ���֮ǰ",QuestManager.Instance.curMainQuestRound);
        }
        if(questDefine.CurrencyCondition!=0)
        {
            questContents[1] = string.Format("׬Ǯ{0}�ռ��", questDefine.CurrencyCondition);
        }
        //if(questDefine.AimNpc != Npc_Name.��)
        //{
        //    questContents[2] = string.Format("����{0}", questDefine.NpcCondition);
        //}

        this.questContent.text = string.Format("{0}{1}{2}", questContents[0], questContents[1], questContents[2]);

        foreach (var rectTransform in rectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
