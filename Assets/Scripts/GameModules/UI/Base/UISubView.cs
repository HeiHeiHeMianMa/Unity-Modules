using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModules.UI
{
    public class UISubView : MonoBehaviour
    {
        protected bool isInit = false;

        private void Awake()
        {
            OnInit();
            OnAddListener();
        }

        private void OnEnable()
        {
            OnOpen();
        }

        private void OnDisable()
        {
            OnClose();
        }

        private void OnDestroy()
        {
            OnRelease();
            OnRemoveListener();
        }

        private void Bind()
        {
            if (isInit) return;
            isInit = true;
        }

        public virtual void OnInit() {
            if (isInit) return;
            Bind();
        }

        public virtual void OnAddListener() { }

        public virtual void OnRemoveListener() { }

        public virtual void OnOpen()
        {
            if (!isInit)
            {
                OnInit();
            }
        }

        public virtual void OnClose() { }

        public virtual void OnRelease() { }
    }
}
