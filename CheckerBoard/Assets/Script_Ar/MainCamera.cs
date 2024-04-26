using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCamera : MonoBehaviour
{
    [SerializeField, LabelText("主相机"), ReadOnly]
    Camera mainCamera;

    [SerializeField, LabelText("相机Id"), Tooltip("标记相机Id")]
    public int id;

    //[SerializeField, LabelText("初始位置"), ReadOnly]
    //public Vector3 initPos;
    [SerializeField, LabelText("相位置和范围"), ReadOnly]
    public CameraDefine cameraDefine;

    [SerializeField, LabelText("相机最大高度"), Tooltip("音乐开关")]
    public float fieldOfViewMax = 60f;
    //public float highMax = 7f;
    [SerializeField, LabelText("相机最小高度"), Tooltip("音乐开关")]
    public float fieldOfViewMin = 10f;
    //public float highMin = 1.5f;
    public void Awake()
    {
        this.mainCamera = this.GetComponent<Camera>();
    }

    void Init()
    {
        if(cameraDefine==null)
        {
            cameraDefine = DataManager.CameraDefines[0];
        }
        this.StartControl();
    }

    /// <summary>
    /// 重开
    /// </summary>
    public  void Restart()
    {
        this.Init();
        this.transform.position = new Vector3(cameraDefine.InitPosX, cameraDefine.InitPosY, cameraDefine.InitPosZ);
    }

    /// <summary>
    /// 读档
    /// </summary>
    public void ReadArchive()
    {
        this.Init();
        this.transform.position =ArchiveManager.archive.CameraPos;
    }

    /// <summary>
    /// 相机控制事件
    /// </summary>
    IDisposable CameraMoveControl;
    IDisposable CameraZoomControl;

    //上一帧位置
    Vector3 posLastSecond = Vector3.zero;
    /// <summary>
    /// 开始控制相机
    /// </summary>
    public void StartControl()
    {
        CameraMoveControl = Observable
        .EveryFixedUpdate()
        .Subscribe(_ =>
        {
            this.CameraMove();
        });

        CameraZoomControl = Observable
        .EveryUpdate()
        .Subscribe(_ =>
        {
            this.CameraZoom();
        });
    }

    /// <summary>
    /// 结束控制相机
    /// </summary>
    public void StopControl()
    {
        if (CameraMoveControl != null)
        {
            CameraMoveControl.Dispose();
        }
        if (CameraZoomControl != null)
        {
            CameraZoomControl.Dispose();
        }
    }

    /// <summary>
    /// 相机位置移动
    /// </summary>
    void CameraMove()
    {
        if (Input.GetMouseButton(2))
        {
            //if (EventSystem.current.IsPointerOverGameObject())
            //    return;
            Vector3 mousePos = Input.mousePosition;
            Vector3 speed = mousePos - posLastSecond;
            if (speed.magnitude < 90f)
            {
                Vector3 pos= this.transform.position - new Vector3(speed.x, speed.y, 0) * Time.fixedDeltaTime;
                if (pos.x < cameraDefine.ScopeLeft || pos.x > cameraDefine.ScopeRight)
                {
                    pos.x = this.transform.position.x;
                }
                if(pos.y < cameraDefine.ScopeDown || pos.y > cameraDefine.ScopeUp)
                {
                    pos.y = this.transform.position.y;
                }
                this.transform.position = pos;
            }

            posLastSecond = mousePos;
        }
    }

    /// <summary>
    /// 相机视角缩放
    /// </summary>
    void CameraZoom()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if(zoom==0f)
        {
            return;
        }
        float f = Mathf.Lerp(400, 600, zoom);
        float fOP = 0f;
        if (zoom>0f)
        {
            //v3 = this.transform.position + transform.forward * -f * Time.deltaTime;
            fOP=this.mainCamera.fieldOfView+(-f)*Time.deltaTime;
        }
        else if(zoom<0f)
        {
            //v3 = this.transform.position + transform.forward * f * Time.deltaTime;
            fOP = this.mainCamera.fieldOfView + f * Time.deltaTime;
        }

        if (fOP >= fieldOfViewMax|| fOP <= fieldOfViewMin)
        {
            fOP = this.mainCamera.fieldOfView;
        }
        this.mainCamera.fieldOfView = fOP;
    }
}
