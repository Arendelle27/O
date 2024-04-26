using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCamera : MonoBehaviour
{
    [SerializeField, LabelText("�����"), ReadOnly]
    Camera mainCamera;

    [SerializeField, LabelText("���Id"), Tooltip("������Id")]
    public int id;

    //[SerializeField, LabelText("��ʼλ��"), ReadOnly]
    //public Vector3 initPos;
    [SerializeField, LabelText("��λ�úͷ�Χ"), ReadOnly]
    public CameraDefine cameraDefine;

    [SerializeField, LabelText("������߶�"), Tooltip("���ֿ���")]
    public float fieldOfViewMax = 60f;
    //public float highMax = 7f;
    [SerializeField, LabelText("�����С�߶�"), Tooltip("���ֿ���")]
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
    /// �ؿ�
    /// </summary>
    public  void Restart()
    {
        this.Init();
        this.transform.position = new Vector3(cameraDefine.InitPosX, cameraDefine.InitPosY, cameraDefine.InitPosZ);
    }

    /// <summary>
    /// ����
    /// </summary>
    public void ReadArchive()
    {
        this.Init();
        this.transform.position =ArchiveManager.archive.CameraPos;
    }

    /// <summary>
    /// ��������¼�
    /// </summary>
    IDisposable CameraMoveControl;
    IDisposable CameraZoomControl;

    //��һ֡λ��
    Vector3 posLastSecond = Vector3.zero;
    /// <summary>
    /// ��ʼ�������
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
    /// �����������
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
    /// ���λ���ƶ�
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
    /// ����ӽ�����
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
