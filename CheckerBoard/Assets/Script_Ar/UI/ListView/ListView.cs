using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UILIST
{
    public class ListView:MonoBehaviour
    {
        public UnityAction<ListViewItem> onItemSelected;

        [SerializeField, LabelText("内容位置"), Tooltip("该列表父物体")]
        public Transform content;
        public class ListViewItem : MonoBehaviour, IPointerClickHandler
        {
            [SerializeField, LabelText("背景图像"), Tooltip("放入背景图像")]
            public Image background;
            [SerializeField, LabelText("未被选择时的背景"), Tooltip("放入未被选择时的背景")]
            public Sprite normalBg;
            [SerializeField, LabelText("被选择时的背景"), Tooltip("被选择时的背景")]
            public Sprite selectedBg;

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set
                {
                    selected = value;
                    onSelected(selected);
                }
            }

            public virtual void OnEnable()
            {
                this.background.sprite = normalBg;
                this.Selected = false;
            }

            public virtual void onSelected(bool selected)
            {
                this.background.sprite = selected ? selectedBg : normalBg;
            }

            public ListView owner;

            public void OnPointerClick(PointerEventData eventData)
            {
                if (!this.selected)
                {
                    this.Selected = true;
                }
                //if (owner != null && owner.SelectedItem != this)
                if (owner != null)
                {
                    owner.SelectedItem = this;
                }
            }
        }

        List<ListViewItem> items = new List<ListViewItem>();

        private ListViewItem selectedItem = null;
        public ListViewItem SelectedItem
        {
            get { return selectedItem; }
            private set
            {
                if (selectedItem != null && selectedItem != value)
                {
                    selectedItem.Selected = false;
                }
                selectedItem = value;
                if (onItemSelected != null)
                    onItemSelected.Invoke((ListViewItem)value);
            }
        }

        private void Start()
        {
            this.content=this.content??this.transform;//如果content为空则默认为自己
        }

        public void AddItem(ListViewItem item)
        {
            item.owner = this;
            this.items.Add(item);
        }

        public void RemoveAll()
        {
            foreach (var it in items)
            {
                Destroy(it.gameObject);
            }
            items.Clear();
        }
    }
}

