using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace GameModule.UI
{
    public partial class UIMessageWindow : UIViewBase
    {   
        [SerializeField] private Button bg_btn;
		[SerializeField] private TextMeshProUGUI TextTitle_txt;
		[SerializeField] private Button ButtonCloses_btn;
		[SerializeField] private TextMeshProUGUI TextCancel_txt;
		[SerializeField] private Button ButtonConfirm_btn;
		[SerializeField] private TextMeshProUGUI TextConfirm_txt;
		[SerializeField] private TextMeshProUGUI TextContent_txt;
    }
}