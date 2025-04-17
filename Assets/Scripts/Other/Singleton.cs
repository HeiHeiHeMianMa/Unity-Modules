using UnityEngine;

public abstract class Singleton<T> where T : new()
{
    private static T serviceContext;
    private readonly static object lockObj = new object();

    /// <summary>
    /// 禁止外部进行实例化
    /// </summary>
    protected Singleton()
    {
        OnInitialize();
    }

    /// <summary>
    /// 获取唯一实例，双锁定防止多线程并发时重复创建实例
    /// </summary>
    /// <returns></returns>
    public static T Instance
    {
        get
        {
            if (serviceContext == null)
            {
                lock (lockObj)
                {
                    if (serviceContext == null)
                    {
                        serviceContext = new T();
                    }
                }
            }
            return serviceContext;
        }
    }
    public virtual void OnInitialize() { }

    public void Init() { }
}
