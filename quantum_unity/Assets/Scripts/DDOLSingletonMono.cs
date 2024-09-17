using UnityEngine;

/// <summary>
/// 该单例继承于Mono,并且不会随着场景的切换而销毁
/// 可以用于任何继承了Mono的组件
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class DDOLSingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
