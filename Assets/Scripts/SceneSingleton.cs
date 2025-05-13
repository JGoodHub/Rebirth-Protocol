using System;
using UnityEngine;

namespace GoodHub.Core.Runtime
{
    [DefaultExecutionOrder(-50)]
    public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _singleton;

        public static T Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;

                _singleton = FindObjectOfType<T>(true);

                if (_singleton == null)
                    Debug.LogError($"ERROR: No active instance of the Singleton {typeof(T)} found in this scene");

                return _singleton;
            }
        }
    }
}