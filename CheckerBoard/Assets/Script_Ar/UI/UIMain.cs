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
    //1.��Ϸ��ʼ�����
    //2.��Ϸ�е�UI����
    //3.�ƶ������ߵ�UI����
    //4.ѡ����չ̽��С�ӵ�UI����
    //5.�������

    [SerializeField, LabelText("�Զ��رյ���Ϸ���ڽ���"), ReadOnly]
    public UISelectedWindow uISelectedWindow;

    [SerializeField, LabelText("����ָ�����"), ReadOnly]
    public UINoviceGuidePanel uINoviceGuidePanel;

    [SerializeField, LabelText("��ǰ�������"),ReadOnly]
    public int curPanelIndex = -1;

    private void Awake()
    {
        this.uISelectedWindow = UIManager.Instance.Show<UISelectedWindow>();
        this.uINoviceGuidePanel = UIManager.Instance.Show<UINoviceGuidePanel>();
    }

    /// <summary>
    /// �л���ʼ������Ϸ���
    /// </summary>
    /// <param name="ischange"></param>
    public void ChangeToGamePanel(int index)
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
                //else
                //{
                //    uiPanels[i].gameObject.SetActive(false);
                //}
            }
            else
            {
                uiPanels[i].gameObject.SetActive(false);
            }
        }
    }

}