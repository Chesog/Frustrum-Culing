using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Frustrum : MonoBehaviour
{
    Camera cam;

    private const int maxFrustrumPlanes = 6;

    Plane[] planes = new Plane[maxFrustrumPlanes];
    GameObject[] drawingPlanes = new GameObject[maxFrustrumPlanes];
    [SerializeField] Vector3 nearTopLeft;
    [SerializeField] Vector3 nearTopRight;
    [SerializeField] Vector3 nearBottomLeft;
    [SerializeField] Vector3 nearBottomRight;

    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farBottomLeft;
    [SerializeField] Vector3 farBottomRight;

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

        //DrawPlanes(nearPos,farPos);
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

        nearTopLeft.x = nearPlaneDistance.x + (cam.transform.up.x * halfCameraHeightNear) - (cam.transform.right.x * CameraHalfWidthNear);
        nearTopLeft.y = nearPlaneDistance.y + (cam.transform.up.y * halfCameraHeightNear) - (cam.transform.right.y * CameraHalfWidthNear);
        nearTopLeft.z = nearPlaneDistance.z + (cam.transform.up.z * halfCameraHeightNear) - (cam.transform.right.z * CameraHalfWidthNear);

        nearTopRight.x = nearPlaneDistance.x + (cam.transform.up.x * halfCameraHeightNear) + (cam.transform.right.x * CameraHalfWidthNear);
        nearTopRight.y = nearPlaneDistance.y + (cam.transform.up.y * halfCameraHeightNear) + (cam.transform.right.y * CameraHalfWidthNear);
        nearTopRight.z = nearPlaneDistance.z + (cam.transform.up.z * halfCameraHeightNear) + (cam.transform.right.z * CameraHalfWidthNear);

        nearBottomLeft.x = nearPlaneDistance.x - (cam.transform.up.x * halfCameraHeightNear) - (cam.transform.right.x * CameraHalfWidthNear);
        nearBottomLeft.y = nearPlaneDistance.y - (cam.transform.up.y * halfCameraHeightNear) - (cam.transform.right.y * CameraHalfWidthNear);
        nearBottomLeft.z = nearPlaneDistance.z - (cam.transform.up.z * halfCameraHeightNear) - (cam.transform.right.z * CameraHalfWidthNear);

        nearBottomRight.x = nearPlaneDistance.x - (cam.transform.up.x * halfCameraHeightNear) + (cam.transform.right.x * CameraHalfWidthNear);
        nearBottomRight.y = nearPlaneDistance.y - (cam.transform.up.y * halfCameraHeightNear) + (cam.transform.right.y * CameraHalfWidthNear);
        nearBottomRight.z = nearPlaneDistance.z - (cam.transform.up.z * halfCameraHeightNear) + (cam.transform.right.z * CameraHalfWidthNear);
    }
    public void SetFarPoints(Vector3 farPos)
    {
        float halfCameraHeightfar = Mathf.Tan((cam.fieldOfView / 2) * Mathf.Deg2Rad) * cam.farClipPlane;
        float CameraHalfWidthFar = (cam.aspect * halfCameraHeightfar);

        Vector3 farPlaneDistance = cam.transform.position + (cam.transform.forward * cam.farClipPlane);

        farTopLeft.x = farPlaneDistance.x + (cam.transform.up.x * halfCameraHeightfar) - (cam.transform.right.x * CameraHalfWidthFar);
        farTopLeft.y = farPlaneDistance.y + (cam.transform.up.y * halfCameraHeightfar) - (cam.transform.right.y * CameraHalfWidthFar);
        farTopLeft.z = farPlaneDistance.z + (cam.transform.up.z * halfCameraHeightfar) - (cam.transform.right.z * CameraHalfWidthFar);

        farTopRight.x = farPlaneDistance.x + (cam.transform.up.x * halfCameraHeightfar) + (cam.transform.right.x * CameraHalfWidthFar);
        farTopRight.y = farPlaneDistance.y + (cam.transform.up.y * halfCameraHeightfar) + (cam.transform.right.y * CameraHalfWidthFar);
        farTopRight.z = farPlaneDistance.z + (cam.transform.up.z * halfCameraHeightfar) + (cam.transform.right.z * CameraHalfWidthFar);

        farBottomLeft.x = farPlaneDistance.x - (cam.transform.up.x * halfCameraHeightfar) - (cam.transform.right.x * CameraHalfWidthFar);
        farBottomLeft.y = farPlaneDistance.y - (cam.transform.up.y * halfCameraHeightfar) - (cam.transform.right.y * CameraHalfWidthFar);
        farBottomLeft.z = farPlaneDistance.z - (cam.transform.up.z * halfCameraHeightfar) - (cam.transform.right.z * CameraHalfWidthFar);

        farBottomRight.x = farPlaneDistance.x - (cam.transform.up.x * halfCameraHeightfar) + (cam.transform.right.x * CameraHalfWidthFar);
        farBottomRight.y = farPlaneDistance.y - (cam.transform.up.y * halfCameraHeightfar) + (cam.transform.right.y * CameraHalfWidthFar);
        farBottomRight.z = farPlaneDistance.z - (cam.transform.up.z * halfCameraHeightfar) + (cam.transform.right.z * CameraHalfWidthFar);
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
    }
    //public void DrawPlane(Vector3 position, Vector3 normal)
    //{
    //    Vector3 v3;
    //    if (normal.normalized != Vector3.forward)
    //        v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
    //    else
    //        v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;
    //    var corner0 = position + v3;
    //    var corner2 = position - v3;
    //    var q = Quaternion.AngleAxis(90.0f, normal);
    //    v3 = q * v3;
    //    var corner1 = position + v3;
    //    var corner3 = position - v3;
    //    Debug.DrawLine(corner0, corner2, Color.green);
    //    Debug.DrawLine(corner1, corner3, Color.green);
    //    Debug.DrawLine(corner0, corner1, Color.green);
    //    Debug.DrawLine(corner1, corner2, Color.green);
    //    Debug.DrawLine(corner2, corner3, Color.green);
    //    Debug.DrawLine(corner3, corner0, Color.green);
    //    Debug.DrawRay(position, normal, Color.red);
    //}
}
