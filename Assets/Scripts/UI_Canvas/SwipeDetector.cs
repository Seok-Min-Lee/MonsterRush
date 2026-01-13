using System;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    public Action ToLeft;
    public Action ToRight;

    [SerializeField] private float minSwipeDistance = 120f;

    private Vector2 startPos;
    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            startPos = t.position;
        }
        else if (t.phase == TouchPhase.Ended)
        {
            CheckSwipe(t.position);
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            CheckSwipe(Input.mousePosition);
        }
    }

    private void CheckSwipe(Vector2 endPos)
    {
        Vector2 delta = endPos - startPos;

        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);
        float angle = Mathf.Atan2(absY, absX) * Mathf.Rad2Deg;

        if (absX < minSwipeDistance || angle > 30)
        {
            return;
        }


        if (delta.x > 0)
        {
            ToLeft?.Invoke();
        }
        else
        {
            ToRight?.Invoke();
        }
    }
}
