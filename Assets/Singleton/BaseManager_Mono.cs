using UnityEngine;
/// <summary>
/// 单例基类，继承mono
/// </summary>
public class BaseManager_Mono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(T).Name;
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }
    // Makes this object a persistent singleton unless the singleton already exists in which case
    // this object is destroyed
    // 使这个对象成为持久的单例对象，除非这个单例对象已经存在 销毁此对象
    protected void DontDestroy()
    {
        if (this == Instance)
        {
            MonoBehaviour.DontDestroyOnLoad(Instance.gameObject);
        }
        else
        {
            MonoBehaviour.Destroy(this);
        }
    }
}