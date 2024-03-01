using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ENTITY
{
    public class Entity : MonoBehaviour
    {
        internal SpriteRenderer SR;

        internal Collider2D Co;

        public virtual void Awake()
        {
            SpriteRenderer SR = GetComponent<SpriteRenderer>();
            if (SR != null)
            {
                this.SR = SR;
            }

            Collider2D C2D = GetComponent<Collider2D>();
            if (C2D != null)
            {
                this.Co = C2D;
            }
        }

        public void LookToCamera()
        {
            if(this.transform.rotation!=Camera.main.transform.rotation)
            {
                this.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
}
