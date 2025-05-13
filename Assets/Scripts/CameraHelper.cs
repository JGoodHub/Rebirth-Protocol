using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GoodHub.Core.Runtime
{
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

        public static bool GetMouseRaycast(out RaycastHit rayHit, float distance)
        {
            return Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out rayHit, distance);
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

        public static Vector3 GetTouchWorldPosition(Vector2 touchPosition)
        {
            Plane plane = new Plane(Vector3.up, 0f);
            Ray ray = Camera.ScreenPointToRay(touchPosition);

            plane.Raycast(ray, out float distanceToPlane);
            return ray.origin + (ray.direction * distanceToPlane);
        }

        public static Vector2Int GetTouchSnappedPosition(Vector2 touchPosition)
        {
            Vector3 touchWorldPosition = GetTouchWorldPosition(touchPosition);
            return new Vector2Int(Mathf.RoundToInt(touchWorldPosition.x), Mathf.RoundToInt(touchWorldPosition.z));
        }

        public static Vector3Int GetTouchSnappedPosition3D(Vector2 touchPosition)
        {
            Vector3 touchWorldPosition = GetTouchWorldPosition(touchPosition);
            return new Vector3Int(Mathf.RoundToInt(touchWorldPosition.x), 0, Mathf.RoundToInt(touchWorldPosition.z));
        }
    }
}