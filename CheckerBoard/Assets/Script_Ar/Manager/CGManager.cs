using MANAGER;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CGManager : Singleton<CGManager>
{
    //CG动画图像列表
    public Dictionary<int, List<string>> CGlist=new Dictionary<int, List<string>>();

    public void ReStart()
    {
        List<string> list = new List<string>(3) { "开场cg01", "开场cg02", "开场cg03" };
        CGlist.Add(0, list);//0为开场动画

        list = new List<string>(3) { "追逐cg01", "追逐cg02" };
    }


    public IEnumerator PlayCG(int id)
    {
        UICGWindow uicg = UIManager.Instance.Show<UICGWindow>();
        int i = 0;
        switch (id)
        {
            case 0:
                while (this.CGlist[id].Count > i)
                {
                    string path = string.Format("CG/开场/{0}", CGlist[id][i]);
                    uicg.sprite = Resources.Load<Sprite>(path);
                    i++;
                    yield return new WaitForSeconds(1.5f);
                }
                ChatManager.Instance.RoundStart(1);//开场动画播放完毕，开始第一回合
                break;
            case 1:
                while (this.CGlist[id].Count > i)
                {
                    string path = string.Format("CG/追逐/{0}", CGlist[id][i]);
                    uicg.sprite = Resources.Load<Sprite>(path);
                    i++;
                    yield return new WaitForSeconds(1.5f);
                }
                break;
        }
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

