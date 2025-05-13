using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class CameraHelper
{
    private static Camera _camera;

    public static Camera Camera => _camera ??= Camera.main;

    static CameraHelper()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private static void OnActiveSceneChanged(Scene old, Scene @new)
    {
        _camera = Camera.main;
    }

    public static Ray GetMouseRay()
    {
        return Camera.ScreenPointToRay(Input.mousePosition);
    }

    public static bool IsMouseOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult raycastResult in raycastResults)
        {
            if (raycastResult.gameObject.layer == 5)
                return true;
        }

        return false;
    }
}