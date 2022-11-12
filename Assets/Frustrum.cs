using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Frustrum : MonoBehaviour
{
    Camera cam;

    private const int maxFrustrumPlanes = 6;
    private const int maxTestObjests = 4;
    private const int aabbPoints = 8;

    Plane[] planes = new Plane[maxFrustrumPlanes];
    [SerializeField] GameObject[] TestObjests = new GameObject[maxTestObjests];

    public struct testObject
    {
        public GameObject gameObject;
        public Vector3[] aabb;
    }

    [SerializeField] Vector3 nearTopLeft;
    [SerializeField] Vector3 nearTopRight;
    [SerializeField] Vector3 nearBottomLeft;
    [SerializeField] Vector3 nearBottomRight;

    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farBottomLeft;
    [SerializeField] Vector3 farBottomRight;

    [SerializeField] Vector3[] aabb = new Vector3[aabbPoints];

    [SerializeField] testObject[] testObjets = new testObject[maxTestObjests];

    bool rancio = true;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        for (int i = 0; i < maxTestObjests; i++)
        {
            testObjets[i].gameObject = TestObjests[i];
            testObjets[i].aabb = new Vector3[aabbPoints];
        }


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
        planes[5].Set3Points(cam.transform.position, farBottomRight, farBottomLeft);//bottom

        for (int i = 2; i < maxFrustrumPlanes; i++)
        {
            planes[i].Flip();
        }

        for (int i = 0; i < maxTestObjests; i++)
        {
            SetAABB(ref testObjets[i]);
        }
        for (int i = 0; i < maxTestObjests; i++)
        {
            CheckObjetColition(testObjets[i]);
        }

        //if (rancio)
        //{
        //    for (int i = 0; i < 6; ++i)
        //    {
        //        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //        p.name = "Plane " + i.ToString();
        //        p.transform.position = -planes[i].normal * planes[i].distance;
        //        p.transform.rotation = Quaternion.FromToRotation(Vector3.up, planes[i].normal);
        //    }
        //    rancio = false;
        //}

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
    public void SetAABB(ref testObject currentObject) 
    {

        Vector3 scale = currentObject.gameObject.transform.localScale / 2;
        Vector3 forward = currentObject.gameObject.transform.forward;
        Vector3 up = currentObject.gameObject.transform.up;
        Vector3 right = currentObject.gameObject.transform.right;

        for (int i = 0; i < aabbPoints; i++)
        {
            currentObject.aabb[i] = currentObject.gameObject.transform.position;
        }

        currentObject.aabb[0] += scale.x * right + scale.y * up + scale.z * forward;
        currentObject.aabb[1] += scale.x * right + scale.y * up + -scale.z * forward;
        currentObject.aabb[2] += scale.x * right + -scale.y * up + scale.z * forward;
        currentObject.aabb[3] += scale.x * right + -scale.y * up + -scale.z * forward;
        currentObject.aabb[4] += -scale.x * right + scale.y * up + scale.z * forward;
        currentObject.aabb[5] += -scale.x * right + scale.y * up + -scale.z * forward;
        currentObject.aabb[6] += -scale.x * right + -scale.y * up + scale.z * forward;
        currentObject.aabb[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
    }
    public void CheckObjetColition(testObject currentObject) 
    {
        bool isInside = false;

        for (int i = 0; i < aabbPoints; i++)
        {
            int counter = maxFrustrumPlanes;

            for (int j = 0; j < maxFrustrumPlanes; j++)
            {
                if (planes[j].GetSide(currentObject.aabb[i]))
                {
                    counter--;
                }
            }

            if (counter == 0)
            {
                Debug.Log("Está adentro");
                isInside = true;
                break;
            }
        }

        if (isInside)
        {
            if (!currentObject.gameObject.activeSelf)
            {
                currentObject.gameObject.SetActive(true);
            }
        }
        else
        {
            if (currentObject.gameObject.activeSelf)
            {
                Debug.Log("Está afuera");
                currentObject.gameObject.SetActive(false);
            }
        }
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
        for (int i = 0; i < maxTestObjests; i++)
        {
            DrawAABB(ref testObjets[i]);
        }
       // Gizmos.DrawLine();
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
    public void DrawAABB(ref testObject currentObject) 
    {

        Gizmos.color = Color.blue;

        for (int i = 0; i < maxTestObjests; i++)
        {
            for (int j = 0; i < aabbPoints; i++)
            {
                Gizmos.DrawSphere(currentObject.aabb[i], 0.2f);
            }
        }
        Gizmos.color = Color.green;
    }

    public void DrawPlane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;
        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;
        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }
}
