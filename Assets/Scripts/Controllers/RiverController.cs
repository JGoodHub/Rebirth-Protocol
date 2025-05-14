using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class RiverController : SceneSingleton<RiverController>
{
    public enum RiverTileType
    {
        TopLeft,
        TopMiddle,
        TopRight,
        MiddleLeft,
        Center,
        MiddleRight,
        BottomLeft,
        BottomMiddle,
        BottomRight,
    }

    [Serializable]
    public class RiverCurve
    {
        public Transform start;
        public Transform end;
        public Vector2 controlPoint1Offset = Vector2.up * 2f;
        public Vector2 controlPoint2Offset = Vector2.down * 2f;

        public Vector2 P0 => start.position;
        public Vector2 P1 => (Vector2)start.position + controlPoint1Offset;
        public Vector2 P2 => (Vector2)end.position + controlPoint2Offset;
        public Vector2 P3 => end.position;
    }

    [Serializable]
    public class RiverTilePair
    {
        public RiverTileType Type;
        public Sprite Sprite;
    }

    [SerializeField]
    private List<RiverCurve> _riverCurves = new List<RiverCurve>();

    [SerializeField]
    private List<RiverTilePair> _riverTilePairs = new List<RiverTilePair>();

    [SerializeField]
    private GameObject _riverTilePrefab;

    [SerializeField]
    private Transform _riverTilesParent;

    public void Initialise()
    {
        RiverCurve riverA = _riverCurves[Random.Range(0, _riverCurves.Count)];
        RiverCurve riverB = _riverCurves[Random.Range(0, _riverCurves.Count)];

        while (riverB == riverA)
        {
            riverB = _riverCurves[Random.Range(0, _riverCurves.Count)];
        }

        SetupRiver(riverA);
        SetupRiver(riverB);
    }

    private void SetupRiver(RiverCurve riverCurve)
    {
        List<Vector2Int> rasterizedRiverCurve = RasterizeRiverCurve(riverCurve);

        foreach (Vector2Int rasterizedPoint in rasterizedRiverCurve)
        {
            Instantiate(_riverTilePrefab, new Vector3(rasterizedPoint.x, rasterizedPoint.y), Quaternion.identity,
                _riverTilesParent).GetComponent<SpriteRenderer>().sprite = _riverTilePairs[4].Sprite;

            ObstaclesController.Singleton.RegisterObstacle(rasterizedPoint);
        }
    }

    private List<Vector2Int> RasterizeRiverCurve(RiverCurve curve)
    {
        List<Vector2Int> rasterCurvePoints = new List<Vector2Int>();

        for (int i = 0; i <= 250; i++)
        {
            float t = i / 250f;

            Vector3 pointOnCurve = GetPointOnCurve(curve.P0, curve.P1, curve.P2, curve.P3, t);
            Vector2Int rasterizedPointOnCurve =
                new Vector2Int(Mathf.RoundToInt(pointOnCurve.x), Mathf.RoundToInt(pointOnCurve.y));

            if (rasterCurvePoints.Contains(rasterizedPointOnCurve))
                continue;

            rasterCurvePoints.Add(rasterizedPointOnCurve);
        }

        return rasterCurvePoints;
    }

    public static Vector2 GetPointOnCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        t = Mathf.Clamp01(t);

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        foreach (RiverCurve riverCurve in _riverCurves)
        {
            if (riverCurve.start == null || riverCurve.end == null)
                continue;

            Vector2 p0 = riverCurve.start.position;
            Vector2 p1 = p0 + riverCurve.controlPoint1Offset;
            Vector2 p3 = riverCurve.end.position;
            Vector2 p2 = p3 + riverCurve.controlPoint2Offset;

            Handles.DrawBezier(p0, p3, p1, p2, Color.cyan, null, 2f);

            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p3, p2);

            List<Vector2Int> rasterizedRiverCurve = RasterizeRiverCurve(riverCurve);
            foreach (Vector2Int rasterizedPoint in rasterizedRiverCurve)
            {
                Gizmos.DrawSphere(new Vector3(rasterizedPoint.x, rasterizedPoint.y), 0.3f);
            }
        }
#endif
    }
}