using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GameModule.UI
{
    public class UIViewBase : MonoBehaviour
    {
        private UIViewController controller;
        private Canvas canvas;

        public UIViewController Controller => controller;

        public virtual void OnInit(UIViewController controller)
        {
            this.controller = controller;
            canvas = gameObject.GetOrAddComponent<Canvas>();
            gameObject.GetOrAddComponent<CanvasScaler>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        /// <summary>
        /// 事件监听
        /// </summary>
        public virtual void OnAddListener() { }

        /// <summary>
        /// 事件移除
        /// </summary>
        public virtual void OnRemoveListener() { }

        /// <summary>
        /// 打开
        /// </summary>
        public virtual void OnOpen(object userData)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = controller.order;

            OnAddListener();
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public virtual void OnResume()
        {
        }

        /// <summary>
        /// 被覆盖
        /// </summary>
        public virtual void OnPause()
        {
        }

        /// <summary>
        /// 被关闭
        /// </summary>
        public virtual void OnClose()
        {
            OnRemoveListener();
        }

        /// <summary>
        /// 取消按钮响应
        /// </summary>
        public virtual void OnCancel()
        {
            Module.UI.Close(controller.uiType);
        }

        /// <summary>
        /// 被卸载释放
        /// </summary>
        public virtual void OnRelease() { }
    }
}
