using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodHub.Core.Runtime
{
    public class CameraDragController2D : SceneSingleton<CameraDragController2D>
    {
        private Vector3 _dragStartPosition;
        private Vector3 _dragPosition;

        [SerializeField] private bool _lockToBounds;
        [SerializeField] private Bounds _cameraBoundary;

        public bool DragEnabled = true;

        private void Awake()
        {
            _dragPosition = transform.position;
        }

        private void OnEnable()
        {
            TouchInput.OnTouchDown += OnTouchDown;
            TouchInput.OnTouchDragStay += OnTouchDragStay;
        }

        private void OnDisable()
        {
            TouchInput.OnTouchDown -= OnTouchDown;
            TouchInput.OnTouchDragStay -= OnTouchDragStay;
        }

        private void OnTouchDown(TouchInput.TouchData touchData)
        {
            if (CameraHelper.IsMouseOverUI() || DragEnabled == false || touchData.DownOverUI)
                return;

            Vector3 planeIntersectionPoint = RaycastPlane2D.QueryPlane();
            _dragStartPosition = planeIntersectionPoint;
        }

        private void OnTouchDragStay(TouchInput.TouchData touchData)
        {
            if (CameraHelper.IsMouseOverUI() || DragEnabled == false || touchData.DownOverUI)
                return;

            Vector3 planeIntersectionPoint = RaycastPlane2D.QueryPlane();
            Vector3 dragDirection = _dragStartPosition - planeIntersectionPoint;

            _dragPosition = transform.position + dragDirection;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _dragPosition, 0.5f);

            if (_lockToBounds)
            {
                float clampedX = Mathf.Clamp(transform.position.x, _cameraBoundary.min.x, _cameraBoundary.max.x);
                float clampedY = Mathf.Clamp(transform.position.y, _cameraBoundary.min.y, _cameraBoundary.max.y);
                transform.position = new Vector3(clampedX, clampedY);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_cameraBoundary.center, _cameraBoundary.size);
        }
    }
}