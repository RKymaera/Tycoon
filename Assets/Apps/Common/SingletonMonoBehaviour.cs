using UnityEngine;


namespace Apps.Common
{
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
{
    // This is a generic Singleton Monobehaviour class that can be used to ensure that only one static instance of a Monobehaviour exists in the scene

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            GameObject instanceObj = new GameObject(typeof(T).ToString() + "Singleton");
            _instance = instanceObj.AddComponent<T>();
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        T _ = Instance;
    }
}
}