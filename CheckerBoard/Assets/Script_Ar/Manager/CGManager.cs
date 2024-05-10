using MANAGER;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class CGManager : MonoSingleton<CGManager>
{
    //CG����ͼ���б�
    public Dictionary<CG_Type, List<string>> CGlist=new Dictionary<CG_Type, List<string>>();

    //���λ��ȥ��һ��
    IDisposable onClickToNext;

    public void ReStart()
    {
        List<string> list = new List<string>(3) { "����cg01", "����cg02", "����cg03" };
        CGlist.Add(CG_Type.����, list);//0Ϊ��������

        list = new List<string>(2) { "�ϵ�������cg01", "�ϵ�������cg02" };
        CGlist.Add(CG_Type.�ϵ�������, list);//0Ϊ��������

        list = new List<string>(1) { "����cg01" };
        CGlist.Add(CG_Type.����, list);//0Ϊ��������

        list = new List<string>(2) { "׷��cg01", "׷��cg02"};
        CGlist.Add(CG_Type.׷��, list);//0Ϊ��������

    }

    /// <summary>
    /// ����CG
    /// </summary>
    /// <param name="cgName"></param>
    public void PlayCG(CG_Type cgName)
    {
        UICGWindow uicg = UIManager.Instance.Show<UICGWindow>();
        int i = 0;
        string path = string.Format("CG/{0}/{1}", cgName.ToString(), CGlist[cgName][i]);
        uicg.sprite = Resources.Load<Sprite>(path);
        this.onClickToNext = Observable
          .EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
        {
            i++;
            if (this.CGlist[cgName].Count <= i)
            {
                this.onClickToNext.Dispose();
                ChatManager.Instance.ChatConditionUnlock(4, (int)cgName);
                uicg.OnCloseClick();
                return;
            }
            path = string.Format("CG/{0}/{1}", cgName.ToString(), CGlist[cgName][i]);
            uicg.sprite = Resources.Load<Sprite>(path);

        });


        //if (playCGCor != null)
        //{
        //    StopCoroutine(playCGCor);
        //}
        //playCGCor = StartCoroutine(PlayCGCor(cgName));
    }

    Coroutine playCGCor;


    IEnumerator PlayCGCor(CG_Type cgName)
    {
        UICGWindow uicg = UIManager.Instance.Show<UICGWindow>();
        int i = 0;
        while (this.CGlist[cgName].Count > i)
        {
            string path = string.Format("CG/{0}/{1}", cgName.ToString(), CGlist[cgName][i]);
            uicg.sprite = Resources.Load<Sprite>(path);
            i++;
            yield return new WaitForSeconds(1.5f);
        }
        //switch(cgName)
        //{
        //    case "����":
        //        ChatManager.Instance.RoundStart(1);//��������������ϣ���ʼ��һ�غ�
        //        break;
        //    case "�ϵ�������":
        //        ChatManager.Instance.ChatConditionUnlock(3, 1);
        //        break;
        //    case "����":
        //        ChatManager.Instance.ChatConditionUnlock(3, 2);
        //        break;
        //    case "׷��":
        //        ChatManager.Instance.ChatConditionUnlock(3, 3);
        //        break;
        //}
        //ChatManager.Instance.RoundStart(1);//��������������ϣ���ʼ��һ�غ�
        ChatManager.Instance.ChatConditionUnlock(4, (int)cgName);
        uicg.OnCloseClick();

    }


    //public void Load()
    //{
    //    CGDefines = new List<CGDefine>();
    //    CGDefineDic = new Dictionary<int, CGDefine>();
    //    string json = File.ReadAllText(PathConfig.GetDataTxtPath("CGDefine.txt"));
    //    CGDefines = JsonConvert.DeserializeObject<List<CGDefine>>(json);
    //    foreach (var item in CGDefines)
    //    {
    //        CGDefineDic.Add(item.TID, item);
    //    }
    //}

    //public CGDefine GetCGDefine(int tid)
    //{
    //    if (CGDefineDic.ContainsKey(tid))
    //    {
    //        return CGDefineDic[tid];
    //    }
    //    return null;
    //}

}

