using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCamera : MonoBehaviour
{
    [SerializeField, LabelText("��ʼλ��"), ReadOnly]
    public Vector3 initPos;

    [SerializeField, LabelText("������߶�"), Tooltip("���ֿ���")]
    public float highMax = 7f;
    [SerializeField, LabelText("�����С�߶�"), Tooltip("���ֿ���")]
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
    /// �ؿ�
    /// </summary>
    public  void Restart()
    {
        this.transform.position = this.initPos;
        this.Init();
    }

    /// <summary>
    /// ����
    /// </summary>
    public void ReadArchive()
    {
        this.transform.position =ArchiveManager.archive.CameraPos;
        this.Init();
    }

    /// <summary>
    /// ��������¼�
    /// </summary>
    IDisposable CameraControl;
    //��һ֡λ��
    Vector3 posLastSecond = Vector3.zero;
    /// <summary>
    /// ��ʼ�������
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
    /// �����������
    /// </summary>
    public void StopControl()
    {
        if (CameraControl != null)
        {
            CameraControl.Dispose();
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
                this.transform.position -= new Vector3(speed.x, speed.y, 0) * Time.deltaTime;
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
