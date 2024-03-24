using System;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        class UIElement
        {
            public string Resources;
            public bool Cache;
            public GameObject Instance;
        }

        private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

        public UIManager()
        {
            this.UIResources.Add(typeof(UIBuildingWindow), new UIElement() { Resources = PathConfig.GetUIPrefabPath("UIBuildingWindow"), Cache = true });

            this.UIResources.Add(typeof(UIScoreWindow), new UIElement() { Resources = PathConfig.GetUIPrefabPath("UIScoreWindow"), Cache = true });

            this.UIResources.Add(typeof(UIUpgradeWindow), new UIElement() { Resources = PathConfig.GetUIPrefabPath("UIUpgradeWindow"), Cache = true });

            this.UIResources.Add(typeof(UISettingWindow), new UIElement() { Resources = PathConfig.GetUIPrefabPath("UISettingWindow"), Cache = true });

            this.UIResources.Add(typeof(UIEventWindow), new UIElement() { Resources = PathConfig.GetUIPrefabPath("UIEventWindow"), Cache = true });

        }

        ~UIManager()
        {

        }

        public T Show<T>()
        {
            Type type = typeof(T);
            if (this.UIResources.ContainsKey(type))
            {
                UIElement info = this.UIResources[type];
                if (info.Instance != null)
                {
                    info.Instance.SetActive(true);
                }
                else
                {
                    UnityEngine.Object prefab = Resources.Load(info.Resources);
                    if (prefab == null)
                    {
                        return default(T);
                    }
                    info.Instance = (GameObject)GameObject.Instantiate(prefab);
                }
                return info.Instance.GetComponent<T>();
            }
            return default(T);
        }

        public void Close(Type type)
        {
            if (this.UIResources.ContainsKey(type))
            {
                UIElement info = this.UIResources[type];
                if (info.Cache)
                {
                    info.Instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(info.Instance);
                    info.Instance = null;
                }
            }
        }

        public void Close<T>()
        {
            this.Close(typeof(T));
        }
    }

}
