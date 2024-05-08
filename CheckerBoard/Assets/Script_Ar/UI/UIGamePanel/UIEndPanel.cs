using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIEndPanel : UIPanel
{
    [SerializeField, LabelText("�������"), Tooltip("�������")]
    public Transform buttonPanel;

    public Image endImage;


    [SerializeField, LabelText("���¿�ʼ��Ϸ"), Tooltip("���¿�ʼ��Ϸ����")]
    public Button RestartButton;
    [SerializeField, LabelText("����������"), Tooltip("���������水��")]
    public Button ExitButton;
    private void Start()
    {
        this.RestartButton.OnClickAsObservable().Subscribe(_ =>
        {
            //���¿�ʼ��Ϸ
            Main.Instance.ReStart();
        });

        this.ExitButton.OnClickAsObservable().Subscribe(_ =>
        {
            //�˳���Ϸ
            UIMain.Instance.ChangeToGamePanel(0);

        });
    }

    void OnEnable()
    {
        this.showButtonCor=this.StartCoroutine(ShowButton());
    }

    Coroutine showButtonCor;

    IEnumerator ShowButton()
    {
        this.buttonPanel.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        this.buttonPanel.gameObject.SetActive(true);
        StopCoroutine(showButtonCor);
    }

    public void SetInfo(int id)
    {
        string path = string.Format("UI/End/{0}",id);
        this.endImage.sprite = Resources.Load<Sprite>(path);

    }
}
