using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class CameraPosTool : Editor
{
    [MenuItem("Tools/�������λ�úͷ�Χ")]
    public static void LoadCameraPosition()
    {
        DataManager.LoadCameraDefine();

        Scene current = EditorSceneManager.GetActiveScene();
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("��ʾ", "���ȱ��浱ǰ����", "ȷ��");
            return;
        }

        CameraDefine cameraDefine = DataManager.CameraDefines[0];

        MainCamera[] mainCameras = GameObject.FindObjectsOfType<MainCamera>();
        foreach (var mainCamera in mainCameras)
        {
            switch (mainCamera.id)
            {
                case -1:
                    cameraDefine.InitPosX = mainCamera.transform.position.x;
                    cameraDefine.InitPosY = mainCamera.transform.position.y;
                    cameraDefine.InitPosZ = mainCamera.transform.position.z;
                    break;
                case 0:
                    cameraDefine.ScopeUp = mainCamera.transform.position.y;
                    break;
                case 1:
                    cameraDefine.ScopeDown = mainCamera.transform.position.y;
                    break;
                case 2:
                    cameraDefine.ScopeLeft = mainCamera.transform.position.x;
                    break;
                case 3:
                    cameraDefine.ScopeRight = mainCamera.transform.position.x;
                    break;
            }
        }
        DataManager.CameraDefines[0] = cameraDefine;
        DataManager.SaveCameraDefine();
        EditorUtility.DisplayDialog("��ʾ", "���λ�úͷ�Χ�������", "ȷ��");
    }
}
