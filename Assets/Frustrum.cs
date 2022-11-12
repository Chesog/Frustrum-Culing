using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Frustrum : MonoBehaviour
{
    Camera cam;

    private const int maxFrustrumPlanes = 6;
    private const int maxTestObjests = 4;

    Plane[] planes = new Plane[maxFrustrumPlanes];

    [SerializeField] Vector3 nearTopLeft;
    [SerializeField] Vector3 nearTopRight;
    [SerializeField] Vector3 nearBottomLeft;
    [SerializeField] Vector3 nearBottomRight;

    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farBottomLeft;
    [SerializeField] Vector3 farBottomRight;

    [SerializeField] GameObject[] TestObjests = new GameObject[maxTestObjests];

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {

        float halfCameraHeight = cam.farClipPlane * MathF.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float CameraWidth = (halfCameraHeight * 2) * cam.aspect;
        Vector3 frontMultFar = cam.farClipPlane * cam.transform.forward;


        Vector3 nearPos = cam.transform.position;
        nearPos += cam.transform.forward * cam.nearClipPlane;
        planes[0] = new Plane(cam.transform.forward, nearPos);

        Vector3 farPos = cam.transform.position;
        farPos += (cam.transform.forward) * cam.farClipPlane;
        planes[1] = new Plane(cam.transform.forward * -1, farPos);

        planes[2] = new Plane(cam.transform.position, -Vector3.Cross(cam.transform.up, frontMultFar + cam.transform.right * CameraWidth));

        planes[3] = new Plane(cam.transform.position, -Vector3.Cross(frontMultFar - cam.transform.right * CameraWidth, cam.transform.up));

        planes[4] = new Plane(cam.transform.position, -Vector3.Cross(cam.transform.right, frontMultFar - cam.transform.up * halfCameraHeight));

        planes[5] = new Plane(cam.transform.position, -Vector3.Cross(frontMultFar + cam.transform.up * halfCameraHeight, cam.transform.right));

    }


    private void FixedUpdate()
    {
        UpdatePlanes();
    }
    void UpdatePlanes()
    {
        Vector3 frontMultFar = cam.farClipPlane * cam.transform.forward;

        Vector3 nearPos = cam.transform.position;
        nearPos += cam.transform.forward * cam.nearClipPlane;
        planes[0].SetNormalAndPosition(cam.transform.forward, nearPos);


        Vector3 farPos = cam.transform.position;
        farPos += (cam.transform.forward) * cam.farClipPlane;
        planes[1].SetNormalAndPosition(cam.transform.forward * -1, farPos);

        SetNearPoints(nearPos);
        SetFarPoints(farPos);

        planes[2].Set3Points(cam.transform.position, farBottomLeft, farTopLeft);//left
        planes[3].Set3Points(cam.transform.position, farTopRight, farBottomRight);//right
        planes[4].Set3Points(cam.transform.position, farTopLeft, farTopRight);//top
        planes[5].Set3Points(cam.transform.position, farBottomRight, farTopLeft);//bottom

    }
    public void SetNearPoints(Vector3 nearPos)
    {

        float halfCameraHeightNear = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.nearClipPlane;
        float CameraHalfWidthNear = (cam.aspect * halfCameraHeightNear);

        Vector3 nearPlaneDistance = cam.transform.position + (cam.transform.forward * cam.nearClipPlane);

        nearTopLeft = nearPlaneDistance + (cam.transform.up * halfCameraHeightNear) - (cam.transform.right * CameraHalfWidthNear);

        nearTopRight = nearPlaneDistance + (cam.transform.up * halfCameraHeightNear) + (cam.transform.right * CameraHalfWidthNear);

        nearBottomLeft = nearPlaneDistance - (cam.transform.up * halfCameraHeightNear) - (cam.transform.right * CameraHalfWidthNear);

        nearBottomRight = nearPlaneDistance - (cam.transform.up * halfCameraHeightNear) + (cam.transform.right * CameraHalfWidthNear);
    }
    public void SetFarPoints(Vector3 farPos)
    {
        float halfCameraHeightfar = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.farClipPlane;
        float CameraHalfWidthFar = (cam.aspect * halfCameraHeightfar);

        Vector3 farPlaneDistance = cam.transform.position + (cam.transform.forward * cam.farClipPlane);


        farTopLeft = farPlaneDistance + (cam.transform.up * halfCameraHeightfar) - (cam.transform.right * CameraHalfWidthFar);

        farTopRight = farPlaneDistance + (cam.transform.up * halfCameraHeightfar) + (cam.transform.right * CameraHalfWidthFar);

        farBottomLeft = farPlaneDistance - (cam.transform.up * halfCameraHeightfar) - (cam.transform.right * CameraHalfWidthFar);

        farBottomRight = farPlaneDistance - (cam.transform.up * halfCameraHeightfar) + (cam.transform.right * CameraHalfWidthFar);
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //Plano Cercano
        DrawPlane(nearTopRight, nearBottomRight, nearBottomLeft, nearTopLeft);

        //Plano Lejano
        DrawPlane(farTopRight, farBottomRight, farBottomLeft, farTopLeft);

        // Plano Derecho
        DrawPlane(nearTopRight, farTopRight, farBottomRight, nearBottomRight);

        // Plano Izquierdo
        DrawPlane(nearTopLeft, farTopLeft, farBottomLeft, nearBottomLeft);

        // Plano Superior
        DrawPlane(nearTopLeft, farTopLeft, farTopRight, nearTopRight);

        //Plano Inferior
        DrawPlane(nearBottomLeft, farBottomLeft, farBottomRight, nearBottomRight);
    }
    public void DrawPlane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(p1, p3);
        Gizmos.DrawLine(p2, p4);
        Gizmos.color = Color.green;
    }
}
