using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pets : MonoBehaviour
{
    //姓名
    public string Name;
    //编号
    public int Id;
    //年龄
    public int age = 1;
    //宠物颜色
    public Color Color_App;
    //宠物位置
    public Vector3 Pet_Position;
    //宠物睡眠曲线
    public AnimationCurve Pet_Sleep;
    //宠物guid
    public double Pet_GUID;

    public Pet_FlagsEnum PetFlagsEnum;
    // Start is called before the first frame update
    void Start()
    {
        //Pet_Skill.allowSwitchOff
    }

    // Update is called once per frame
    void Update()
    {

    }
}
public enum Pet_FlagsEnum
{
    [Header("未知")]
    none = 0,
    [Header("睡觉")]
    sleep = 1 << 0,
    [Header("吃饭")]
    rice = 1 << 1,
    [Header("睡觉和吃饭")]
    sleepandrice = sleep | rice,
    [Header("泡妞")]
    xxoo = 1 << 2,
    [Header("全干")]
    Everthing = ~0,
}

