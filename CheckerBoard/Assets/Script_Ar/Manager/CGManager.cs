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
    //CG动画图像列表
    public Dictionary<CG_Type, List<string>> CGlist=new Dictionary<CG_Type, List<string>>();

    //点击位置去下一张
    IDisposable onClickToNext;

    public void ReStart()
    {
        List<string> list = new List<string>(3) { "开场cg01", "开场cg02", "开场cg03" };
        CGlist.Add(CG_Type.醒来, list);//0为开场动画

        list = new List<string>(2) { "老爹的遗书cg01", "老爹的遗书cg02" };
        CGlist.Add(CG_Type.老爹的遗书, list);//0为开场动画

        list = new List<string>(1) { "档案cg01" };
        CGlist.Add(CG_Type.档案, list);//0为开场动画

        list = new List<string>(2) { "追逐cg01", "追逐cg02"};
        CGlist.Add(CG_Type.追逐, list);//0为开场动画

    }

    /// <summary>
    /// 播放CG
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
        //    case "醒来":
        //        ChatManager.Instance.RoundStart(1);//开场动画播放完毕，开始第一回合
        //        break;
        //    case "老爹的遗书":
        //        ChatManager.Instance.ChatConditionUnlock(3, 1);
        //        break;
        //    case "档案":
        //        ChatManager.Instance.ChatConditionUnlock(3, 2);
        //        break;
        //    case "追逐":
        //        ChatManager.Instance.ChatConditionUnlock(3, 3);
        //        break;
        //}
        //ChatManager.Instance.RoundStart(1);//开场动画播放完毕，开始第一回合
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

