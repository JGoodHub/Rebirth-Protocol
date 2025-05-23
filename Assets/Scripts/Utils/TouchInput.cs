using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : SceneSingleton<TouchInput>
{
    public struct TouchData
    {
        public Vector2 DownPosition;
        public Vector2 CurrentPosition;
        public Vector2 UpPosition;

        public bool IsDragging;
        public float CurrentDragDistance;

        public bool DownOverUI;
        public bool CurrentlyOverUI;
        public bool UpOverUI;
    }

    #region Events

    public delegate void TouchDelegate(TouchData touchData);

    public static event TouchDelegate OnTouchDown;

    public static event TouchDelegate OnTouchUp;

    public static event TouchDelegate OnTouchClick;

    public static event TouchDelegate OnTouchDragEnter;

    public static event TouchDelegate OnTouchDragStay;

    public static event TouchDelegate OnTouchDragExit;

    #endregion

    public float _relativeDistanceToDrag = 0.04f;
    private float _pixelDistanceToDrag;

    private static TouchData _frameTouchData;

    public static TouchData FrameTouchData => _frameTouchData;

    protected void Awake()
    {
        _pixelDistanceToDrag = Mathf.Sqrt(Mathf.Pow(Screen.width, 2) + Mathf.Pow(Screen.height, 2)) *
                               _relativeDistanceToDrag;
    }

    private void Update()
    {
        _frameTouchData.CurrentPosition = Input.mousePosition;
        _frameTouchData.CurrentlyOverUI = CameraHelper.IsMouseOverUI();

        if (Input.GetMouseButtonDown(0))
        {
            TriggerTouchDown();
            _frameTouchData.CurrentDragDistance = 0;
        }

        if (Input.GetMouseButton(0))
        {
            _frameTouchData.CurrentDragDistance =
                Vector2.Distance(_frameTouchData.DownPosition, _frameTouchData.CurrentPosition);

            if (_frameTouchData.IsDragging || _frameTouchData.CurrentDragDistance >= _pixelDistanceToDrag)
            {
                if (_frameTouchData.IsDragging == false)
                {
                    TriggerTouchDragEnter();
                }

                TriggerTouchDragStay();
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                TriggerTouchUp();

                if (_frameTouchData.CurrentDragDistance < _pixelDistanceToDrag)
                {
                    TriggerTouchClick();
                }
            }

            if (_frameTouchData.IsDragging)
            {
                TriggerTouchDragExit();
            }
        }
    }

    private void TriggerTouchDown()
    {
        _frameTouchData.DownPosition = _frameTouchData.CurrentPosition;
        _frameTouchData.DownOverUI = _frameTouchData.CurrentlyOverUI;

        OnTouchDown?.Invoke(_frameTouchData);
    }

    private void TriggerTouchUp()
    {
        _frameTouchData.UpPosition = _frameTouchData.CurrentPosition;
        _frameTouchData.UpOverUI = _frameTouchData.CurrentlyOverUI;

        OnTouchUp?.Invoke(_frameTouchData);
    }

    private void TriggerTouchClick()
    {
        OnTouchClick?.Invoke(_frameTouchData);
    }

    private void TriggerTouchDragEnter()
    {
        _frameTouchData.IsDragging = true;

        OnTouchDragEnter?.Invoke(_frameTouchData);
    }

    private void TriggerTouchDragStay()
    {
        _frameTouchData.CurrentPosition = Input.mousePosition;
        _frameTouchData.CurrentlyOverUI = EventSystem.current.IsPointerOverGameObject();

        OnTouchDragStay?.Invoke(_frameTouchData);
    }

    private void TriggerTouchDragExit()
    {
        _frameTouchData.IsDragging = false;

        OnTouchDragExit?.Invoke(_frameTouchData);
    }
}