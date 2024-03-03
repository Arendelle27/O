using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        GameObjectPool.Instance.Init();
        
    }

    void Start()
    {
        RoundManager.Instance.Init();
        PlotManager.Instance.Init();
        BuildingManager.Instance.Init();
        WandererManager.Instance.Init();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
