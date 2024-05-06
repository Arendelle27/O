using MANAGER;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CGManager : Singleton<CGManager>
{
    //CG����ͼ���б�
    public Dictionary<int, List<string>> CGlist=new Dictionary<int, List<string>>();

    public void ReStart()
    {
        List<string> list = new List<string>(3) { "����cg01", "����cg02", "����cg03" };
        CGlist.Add(0, list);//0Ϊ��������

        list = new List<string>(3) { "׷��cg01", "׷��cg02" };
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
                    string path = string.Format("CG/����/{0}", CGlist[id][i]);
                    uicg.sprite = Resources.Load<Sprite>(path);
                    i++;
                    yield return new WaitForSeconds(1.5f);
                }
                ChatManager.Instance.RoundStart(1);//��������������ϣ���ʼ��һ�غ�
                break;
            case 1:
                while (this.CGlist[id].Count > i)
                {
                    string path = string.Format("CG/׷��/{0}", CGlist[id][i]);
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

