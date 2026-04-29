using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

//泛型单例模式
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    //只允许可读
    public static T Instance
    {
        get { return instance; }
    }
    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;
    }
    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        if(instance == this)
        {
            instance = null; 
        }
    }
}
