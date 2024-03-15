using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ArchiveTool
{
        /// <summary>
    /// 保存到本地
    /// </summary>
    /// <param name="saveFileName"></param>
    /// <param name="json"></param>
    public static void SaveByJson(string saveFileName,string json)
    {
        var path = Path.Combine(Application.streamingAssetsPath, saveFileName);

        try
        {

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
        var path=Path.Combine(Application.streamingAssetsPath, saveFileName);

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
