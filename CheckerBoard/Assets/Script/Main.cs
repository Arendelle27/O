using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        EntityPool.Instance.Init();
    }

    void Start()
    {

        PlotManager.Instance.Init();
        WandererManager.Instance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
