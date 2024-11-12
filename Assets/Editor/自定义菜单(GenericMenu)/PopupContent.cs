    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 自定义弹出窗口 （可以参考Favorites_GroupPopupGUI）
    /// </summary>
    public class PopupContent : PopupWindowContent
    {
        public float width { get; set; }
        public float Height { get; set; }
        public Action<Rect> OnGUIAction { get; set; }
        public Action onopenAction { get; set; }
        public Action oncloseAction { get; set; }
        public override Vector2 GetWindowSize() => new Vector2(x: width, y: Height);
        public override void OnGUI(Rect rect) => OnGUIAction?.Invoke(rect);
        public override void OnOpen() => onopenAction?.Invoke();
        public override void OnClose() => oncloseAction?.Invoke();
    }