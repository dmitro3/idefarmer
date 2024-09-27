using UnityEngine;

namespace Project_Data.Scripts
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T s_instance = null;
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<T>();
                }
                return s_instance;
            }
        }

        public virtual void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this as T;
            }
        }

        public virtual void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }
    }
}