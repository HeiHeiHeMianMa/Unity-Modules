using System;
using System.Collections.Generic;
using GameModules;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace GameModules.UI
{
    public partial class UITestB : UIViewBase
    {
        public override void OnInit(UIViewController controller)
        {
            base.OnInit(controller);

            ButtonClose_btn.AddClick(() =>
            {
                Module.UI.Close(Controller.uiType);
            });
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
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
