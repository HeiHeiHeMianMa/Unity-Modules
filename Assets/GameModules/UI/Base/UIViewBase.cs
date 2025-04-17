using System;
using System.Collections.Generic;
using GameModules;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GameModules
{
    public class UIViewBase : MonoBehaviour
    {
        private UIViewController _controller;
        private Canvas _canvas;

        public UIViewController Controller => _controller;

        public virtual void OnInit(UIViewController controller)
        {
            this._controller = controller;
            _canvas = gameObject.GetOrAddComponent<Canvas>();
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
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = _controller.order;

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
            GameModule.UI.Close(_controller.uiType);
        }

        /// <summary>
        /// 被卸载释放
        /// </summary>
        public virtual void OnRelease() { }
    }
}
