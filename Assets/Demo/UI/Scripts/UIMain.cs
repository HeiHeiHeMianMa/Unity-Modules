using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;
using GameModules;

namespace GameModules
{
    public partial class UIMain : UIViewBase
    {
        public override void OnInit(UIViewController controller)
        {
            base.OnInit(controller);

            ButtonStart_btn.AddClick(() =>
            {
                GameModule.UI.Open(UIType.UIMessageWindow, PublicPool<MessageBoxData>.Get().Set("提示", "弹窗。", () =>
                {
                    Debug.Log("确认");
                }));
            });
            ButtonSetting_btn.AddClick(() =>
            {
                GameModule.UI.Open(UIType.UITestB);
            });
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            Debug.Log("OnOpen");
        }

        public override void OnResume()
        {
            base.OnResume();
            Debug.Log("OnResume");
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
        }
    }
}
