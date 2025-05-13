using UnityEngine;

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

            _singleton = FindAnyObjectByType<T>(FindObjectsInactive.Include);

            if (_singleton == null)
                Debug.LogError($"ERROR: No active instance of the Singleton {typeof(T)} found in this scene");

            return _singleton;
        }
    }
}