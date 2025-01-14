using UnityEngine;


namespace Apps.Common.Utils
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

                T[] instances = FindObjectsByType<T>(FindObjectsSortMode.None);
                if (instances.Length > 0)
                {
                    _instance = instances[0];
                    for (int i = 1; i < instances.Length; i++)
                    {
                        Debug.LogErrorFormat("Multiple instances of singleton {0} were found. Only the first one was used.", typeof(T).ToString());
                        Destroy(instances[i]);
                    }
                }
                else if (instances.Length == 0)
                {
                    GameObject instanceObj = new GameObject(typeof(T).ToString() + "Singleton");
                    _instance = instanceObj.AddComponent<T>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            T _ = Instance;
        }
    }
}