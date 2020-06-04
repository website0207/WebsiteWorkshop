using System;
using UnityEngine;

namespace WebsiteWorkshop.Frameworks
{
    public class SingletonTemplate<T> where T : new()
    {
        private static T _instance;
        private static readonly object objlock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (objlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }

    public class MonoSingletonTemplate<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object _lock = new object();
        /// <summary>
        /// 程序是否正在退出
        /// </summary>
        protected static bool applicationIsQuitting { get; private set; }
        /// <summary>
        /// 是否为全局单例
        /// </summary>
        protected static bool isGlobal = true;

        static MonoSingletonTemplate()
        {
            applicationIsQuitting = false;
        }

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarning("[Singleton] " + typeof(T) +
                                                " already destroyed on application quit." +
                                                " Won't create again - returning null.");
                    }

                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // 先在场景中找寻
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            if (Debug.isDebugBuild)
                            {
                                Debug.LogWarning("[Singleton] " + typeof(T).Name + " should never be more than 1 in scene!");
                            }

                            return _instance;
                        }

                        // 场景中找不到就创建新物体挂载
                        if (_instance == null)
                        {
                            GameObject singletonObj = new GameObject();
                            _instance = singletonObj.AddComponent<T>();
                            singletonObj.name = "(singleton) " + typeof(T);

                            if (isGlobal && Application.isPlaying)
                            {
                                DontDestroyOnLoad(singletonObj);
                            }

                            return _instance;
                        }
                    }

                    return _instance;
                }
            }
        }

        /// <summary>
        /// 当工程运行结束，在退出时，不允许访问单例
        /// </summary>
        public void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
    }


    /// <summary>
    /// Mono生命周期事件
    /// 一些不继承Mono的类如果想在Mono生命周期做一些事，可以往这里添加
    /// </summary>
    public class MonoEvent : MonoSingletonTemplate<MonoEvent>
    {
        public event Action UPDATE;
        public event Action FIXEDUPDATE;
        public event Action ONGUI;
        public event Action LATEUPDATE;

        private void Update()
        {
            UPDATE?.Invoke();
        }

        private void FixedUpdate()
        {
            FIXEDUPDATE?.Invoke();
        }

        private void OnGUI()
        {
            ONGUI?.Invoke();
        }

        private void LateUpdate()
        {
            LATEUPDATE?.Invoke();
        }

    }
}
