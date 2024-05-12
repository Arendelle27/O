using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoviceGuideTool : Editor
{
    [MenuItem("Tools/储存新手指引窗口的位置")]
    public static void LoadNoviceGuidePosition()
    {
        DataManager.LoadNoviceGuideDefine();

        Scene current = EditorSceneManager.GetActiveScene();
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }


        foreach (var noviceGuideDefine in DataManager.NoviceGuideDefines)
        {
            //string sceneFile = "Assets/Scenes/Main.unity";
            //if (!System.IO.File.Exists(sceneFile))
            //{
            //    Debug.LogWarningFormat("Scene {0} not existed!", sceneFile);
            //    continue;
            //}
            //EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            UINoviceGuideWindow[] uINoviceGuideWindows = GameObject.FindObjectsOfType<UINoviceGuideWindow>();
            foreach (var uINoviceGuideWindow in uINoviceGuideWindows)
            {
                if (!DataManager.NoviceGuideDefines.ContainsKey(uINoviceGuideWindow.id))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("UINoviceGuideWindow:[{0}]中不存在", uINoviceGuideWindow.id), "确定");
                    return;
                }

                NoviceGuideDefine def = DataManager.NoviceGuideDefines[uINoviceGuideWindow.id];

                def.NoviceGuidePosX = uINoviceGuideWindow.transform.position.x;
                def.NoviceGuidePosY = uINoviceGuideWindow.transform.position.y;

                def.NoviceGuideWindowPosX = uINoviceGuideWindow.UINoviceGuideWindowTransform.position.x;
                def.NoviceGuideWindowPosY = uINoviceGuideWindow.UINoviceGuideWindowTransform.position.y;

                def.NoviceGuideArrowPosX = uINoviceGuideWindow.UINoviceGuideArrowTransform.position.x;
                def.NoviceGuideArrowPosY = uINoviceGuideWindow.UINoviceGuideArrowTransform.position.y;
                def.NoviceGuideArrowRotZ = uINoviceGuideWindow.UINoviceGuideArrowTransform.rotation.eulerAngles.z;
            }
        }
        DataManager.SaveNoviceGuidePos();
        EditorUtility.DisplayDialog("提示", "新手指引窗口导出完成", "确定");
    }
}
