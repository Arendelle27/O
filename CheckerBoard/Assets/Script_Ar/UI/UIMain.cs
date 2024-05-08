using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;

public class UIMain : MonoSingleton<UIMain>
{
    [SerializeField, LabelText("��ϷUI����"), Tooltip("��Ϸ��UI����")]
    public List<UIPanel> uiPanels = new List<UIPanel>();
    //0.��Ϸ��ʼ�����
    //1.��Ϸ�е�UI����
    //2.�ƶ������ߵ�UI����
    //3.ѡ����չ̽��С�ӵ�UI����
    //4.�������

    [SerializeField, LabelText("�Զ��رյ���Ϸ���ڽ���"), ReadOnly]
    public UISelectedWindow uISelectedWindow;

    //[SerializeField, LabelText("����ָ�����"), ReadOnly]
    //public UINoviceGuidePanel uINoviceGuidePanel;

    [SerializeField, LabelText("��ǰ�������"),ReadOnly]
    public int curPanelIndex = -1;

    private void Awake()
    {
        this.uISelectedWindow = UIManager.Instance.Show<UISelectedWindow>();
    }

    /// <summary>
    /// �л���ʼ������Ϸ���
    /// </summary>
    /// <param name="ischange"></param>
    public UIPanel ChangeToGamePanel(int index)
    {
        for (int i = 0; i < uiPanels.Count; i++)
        {
            if(i==index)
            {
                this.curPanelIndex = i;
                if (!uiPanels[i].gameObject.activeSelf)
                {
                    uiPanels[i].gameObject.SetActive(true);
                }
            }
            else
            {
                uiPanels[i].gameObject.SetActive(false);
            }
        }
        return uiPanels[index];
    }

}