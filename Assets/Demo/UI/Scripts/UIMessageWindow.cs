using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using GameModule.Pool;

namespace GameModule.UI
{
    public enum MessageBoxType
    {
        TwoButton,
        OneButton,
    }

    public class MessageBoxData
    {
        const string DefaultConfirmName = "确认";
        const string DefaultCancelName = "取消";

        public string title;
        public string content;
        public Action confirm;
        public Action cancel;
        public string confirmName;
        public string cancelName;
        public MessageBoxType type;

        public MessageBoxData Set(string title, string content, Action confirm, Action cancel = null
            , string confirmName = DefaultConfirmName, string cancelName = DefaultCancelName)
        {
            this.title = title;
            this.content = content;
            this.confirm = confirm;
            this.cancel = cancel;
            this.confirmName = confirmName;
            this.cancelName = cancelName;
            this.type = MessageBoxType.TwoButton;
            return this;
        }

        public MessageBoxData SetOneButton(string title, string content, Action confirm, Action cancel = null
            , string confirmName = DefaultConfirmName)
        {
            this.title = title;
            this.content = content;
            this.confirm = confirm;
            this.cancel = cancel;
            this.confirmName = confirmName;
            this.cancelName = null;
            this.type = MessageBoxType.OneButton;
            return this;
        }
    }

    public partial class UIMessageWindow : UIViewBase
    {
        MessageBoxData data;

        public override void OnInit(UIViewController controller)
        {
            base.OnInit(controller);

            ButtonCloses_btn.AddClick(() =>
            {
                data.cancel?.Invoke();
                Module.UI.Close(Controller.uiType);
            });
            bg_btn.AddClick(() =>
            {
                data.cancel?.Invoke();
                Module.UI.Close(Controller.uiType);
            });
            ButtonConfirm_btn.AddClick(() =>
            {
                data.confirm?.Invoke();
                Module.UI.Close(Controller.uiType);
            });
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            data = userData as MessageBoxData;

            TextTitle_txt.text = data.title;
            TextTitle_txt.text = data.content;
            TextTitle_txt.text = data.confirmName;
            TextTitle_txt.text = data.cancelName;

            LayoutRebuilder.ForceRebuildLayoutImmediate(TextContent_txt.rectTransform);

            ButtonCloses_btn.gameObject.SetActive(data.type == MessageBoxType.TwoButton);
        }

        public override void OnAddListener()
        {
            base.OnAddListener();
        }

        public override void OnRemoveListener()
        {
            base.OnRemoveListener();
        }

        public override void OnClose()
        {
            base.OnClose();
            PublicPool<MessageBoxData>.Release(data);
        }
    }
}
