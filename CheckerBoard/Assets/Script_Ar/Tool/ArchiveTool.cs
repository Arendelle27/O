using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ArchiveTool
{
    public static readonly string DataPath = Application.dataPath + "/Data/";

    /// <summary>
    /// 保存到本地
    /// </summary>
    /// <param name="saveFileName"></param>
    /// <param name="json"></param>
    public static void SaveByJson(string saveFileName,string json)
    {
        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }

        var path = Path.Combine(DataPath, saveFileName);

        try
        {
            //if (!File.Exists(path))
            //{
            //    File.Create(path);
            //}
            File.WriteAllText(path, json);
            #if UNITY_EDITOR
            Debug.Log($"保存成功{path}");
            #endif
        }
        catch (System.Exception exception)
        {
            #if UNITY_EDITOR
            Debug.LogError($"保存失败{path}.\n{exception}");
            #endif
        }
    }

        /// <summary>
    /// 读取
    /// </summary>
    /// <param name="saveFileName"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static String LoadByJson(string saveFileName)
    {
        var path=Path.Combine(DataPath, saveFileName);

        try
        {
            String json=File.ReadAllText(path);

            #if UNITY_EDITOR
            Debug.Log($"读取成功{path}");
            #endif
            return json;
        }
        catch (System.Exception exception)
        {
            #if UNITY_EDITOR
            Debug.LogError($"读取失败{path}.\n{exception}");
            #endif
            return default;
        }
    }
}
