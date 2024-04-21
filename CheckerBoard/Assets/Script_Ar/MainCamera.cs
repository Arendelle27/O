using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCamera : MonoBehaviour
{
    [SerializeField, LabelText("初始位置"), ReadOnly]
    public Vector3 initPos;

    [SerializeField, LabelText("相机最大高度"), Tooltip("音乐开关")]
    public float highMax = 7f;
    [SerializeField, LabelText("相机最小高度"), Tooltip("音乐开关")]
    public float highMin = 1.5f;
    public void Awake()
    {
        this.initPos = this.transform.position;
    }

    void Init()
    {
        this.StartControl();
    }

    /// <summary>
    /// 重开
    /// </summary>
    public  void Restart()
    {
        this.transform.position = this.initPos;
        this.Init();
    }

    /// <summary>
    /// 读档
    /// </summary>
    public void ReadArchive()
    {
        this.transform.position =ArchiveManager.archive.CameraPos;
        this.Init();
    }

    /// <summary>
    /// 相机控制事件
    /// </summary>
    IDisposable CameraControl;
    //上一帧位置
    Vector3 posLastSecond = Vector3.zero;
    /// <summary>
    /// 开始控制相机
    /// </summary>
    public void StartControl()
    {

        CameraControl = Observable
        .EveryUpdate()
        .Subscribe(_ =>
        {
            this.CameraMove();
            this.CameraZoom();
        });
    }

    /// <summary>
    /// 结束控制相机
    /// </summary>
    public void StopControl()
    {
        if (CameraControl != null)
        {
            CameraControl.Dispose();
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
                this.transform.position -= new Vector3(speed.x, speed.y, 0) * Time.deltaTime;
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
        float f = Mathf.Lerp(50, 200, zoom);
        Vector3 v3=Vector3.zero;
        if (zoom>0f)
        {
            v3 = this.transform.position + transform.forward * -f * Time.deltaTime;
        }
        else if(zoom<0f)
        {
            v3 = this.transform.position + transform.forward * f * Time.deltaTime;
        }

        if (v3.z <= -highMax|| v3.z >= -highMin)
        {
            v3 = this.transform.position;
        }
        this.transform.position = v3;
    }
}
