using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoviceGuideTool : Editor
{
    [MenuItem("Tools/��������ָ�����ڵ�λ��")]
    public static void LoadNoviceGuidePosition()
    {
        DataManager.LoadNoviceGuideDefine();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScnee = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("��ʾ", "���ȱ��浱ǰ����", "ȷ��");
            return;
        }

        List<UINoviceGuideWindow> allUINoviceGuideWindow = new List<UINoviceGuideWindow>();

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
                    EditorUtility.DisplayDialog("����", string.Format("UINoviceGuideWindow:[{0}]�в�����", uINoviceGuideWindow.id), "ȷ��");
                    return;
                }

                NoviceGuideDefine def = DataManager.NoviceGuideDefines[uINoviceGuideWindow.id];

                def.NoviceGuideWindowPosX = uINoviceGuideWindow.transform.position.x;
                def.NoviceGuideWindowPosY = uINoviceGuideWindow.transform.position.y;

                def.NoviceGuideArrowPosX = uINoviceGuideWindow.UINoviceGuideArrowTransform.position.x;
                def.NoviceGuideArrowPosY = uINoviceGuideWindow.UINoviceGuideArrowTransform.position.y;
                def.NoviceGuideArrowRotZ = uINoviceGuideWindow.UINoviceGuideArrowTransform.rotation.eulerAngles.z;
            }
        }
        DataManager.SaveTNoviceGuidePos();
        EditorUtility.DisplayDialog("��ʾ", "����ָ�����ڵ������", "ȷ��");
    }
}
