using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodHub.Core.Runtime
{
    public static class RaycastPlane2D
    {
        private static Plane? _rayPlane;

        public static Plane RayPlane => _rayPlane ??= new Plane(Vector3.back, 0);

        public static Vector3 QueryPlane()
        {
            Ray mouseRay = CameraHelper.GetMouseRay();
            RayPlane.Raycast(mouseRay, out float distance);

            return mouseRay.GetPoint(distance);
        }
    }
}