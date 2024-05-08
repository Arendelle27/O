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
    [SerializeField, LabelText("按键面板"), Tooltip("按键面板")]
    public Transform buttonPanel;

    public Image endImage;


    [SerializeField, LabelText("重新开始游戏"), Tooltip("重新开始游戏按键")]
    public Button RestartButton;
    [SerializeField, LabelText("返回主界面"), Tooltip("返回主界面按键")]
    public Button ExitButton;
    private void Start()
    {
        this.RestartButton.OnClickAsObservable().Subscribe(_ =>
        {
            //重新开始游戏
            Main.Instance.ReStart();
        });

        this.ExitButton.OnClickAsObservable().Subscribe(_ =>
        {
            //退出游戏
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
