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
    [SerializeField] public bool showPoints;
    public struct testObject
    {
        public GameObject gameObject;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public Vector3[] aabb;
        public Vector3 v3Extents;
        public Vector3 scale;
    }

    [SerializeField] Vector3 nearTopLeft;
    [SerializeField] Vector3 nearTopRight;
    [SerializeField] Vector3 nearBottomLeft;
    [SerializeField] Vector3 nearBottomRight;

    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farBottomLeft;
    [SerializeField] Vector3 farBottomRight;

    [SerializeField] testObject[] testObjets = new testObject[maxTestObjests];

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        for (int i = 0; i < maxTestObjests; i++)
        {
            testObjets[i].gameObject = TestObjests[i];
            testObjets[i].meshRenderer = TestObjests[i].GetComponent<MeshRenderer>();
            testObjets[i].meshFilter = TestObjests[i].GetComponent<MeshFilter>();
            testObjets[i].aabb = new Vector3[aabbPoints];
            testObjets[i].v3Extents = testObjets[i].meshRenderer.bounds.extents;
            testObjets[i].scale = testObjets[i].meshRenderer.bounds.size;
        }

        for (int i = 0; i < maxFrustrumPlanes; i++)
        {
            planes[i] = new Plane();
        }
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
            CheckObjetColition(testObjets[i]);
        }

        for (int i = 0; i < maxTestObjests; i++)
        {
            SetAABB(ref testObjets[i]);
        }
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


        if (currentObject.scale != currentObject.gameObject.transform.localScale)
        {
            Quaternion rotation = currentObject.gameObject.transform.rotation;
            currentObject.gameObject.transform.rotation = Quaternion.identity;
            currentObject.v3Extents = currentObject.meshRenderer.bounds.extents;
            currentObject.scale = currentObject.gameObject.transform.localScale;
            currentObject.gameObject.transform.rotation = rotation;
        }

        Vector3 center = currentObject.meshRenderer.bounds.center;
        Vector3 size = currentObject.v3Extents;

        currentObject.aabb[0] = new Vector3(center.x - size.x, center.y + size.y, center.z - size.z);  // Front top left corner
        currentObject.aabb[1] = new Vector3(center.x + size.x, center.y + size.y, center.z - size.z);  // Front top right corner
        currentObject.aabb[2] = new Vector3(center.x - size.x, center.y - size.y, center.z - size.z);  // Front bottom left corner
        currentObject.aabb[3] = new Vector3(center.x + size.x, center.y - size.y, center.z - size.z);  // Front bottom right corner
        currentObject.aabb[4] = new Vector3(center.x - size.x, center.y + size.y, center.z + size.z);  // Back top left corner
        currentObject.aabb[5] = new Vector3(center.x + size.x, center.y + size.y, center.z + size.z);  // Back top right corner
        currentObject.aabb[6] = new Vector3(center.x - size.x, center.y - size.y, center.z + size.z);  // Back bottom left corner
        currentObject.aabb[7] = new Vector3(center.x + size.x, center.y - size.y, center.z + size.z);  // Back bottom right corner


        // tranformar las Posiciones en puntos en el espacio
        currentObject.aabb[0] = transform.TransformPoint(currentObject.aabb[0]);
        currentObject.aabb[1] = transform.TransformPoint(currentObject.aabb[1]);
        currentObject.aabb[2] = transform.TransformPoint(currentObject.aabb[2]);
        currentObject.aabb[3] = transform.TransformPoint(currentObject.aabb[3]);
        currentObject.aabb[4] = transform.TransformPoint(currentObject.aabb[4]);
        currentObject.aabb[5] = transform.TransformPoint(currentObject.aabb[5]);
        currentObject.aabb[6] = transform.TransformPoint(currentObject.aabb[6]);
        currentObject.aabb[7] = transform.TransformPoint(currentObject.aabb[7]);

        // Roto el punto en la direccion que rota el objeto (Punto a rotar , pivot en el que rota , angulo en cada eje)
        currentObject.aabb[0] = RotatePointAroundPivot(currentObject.aabb[0], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[1] = RotatePointAroundPivot(currentObject.aabb[1], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[2] = RotatePointAroundPivot(currentObject.aabb[2], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[3] = RotatePointAroundPivot(currentObject.aabb[3], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[4] = RotatePointAroundPivot(currentObject.aabb[4], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[5] = RotatePointAroundPivot(currentObject.aabb[5], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[6] = RotatePointAroundPivot(currentObject.aabb[6], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);
        currentObject.aabb[7] = RotatePointAroundPivot(currentObject.aabb[7], currentObject.gameObject.transform.position, currentObject.gameObject.transform.rotation.eulerAngles);


    }
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
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
            for (int i = 0; i < currentObject.meshFilter.mesh.vertices.Length; i++)
            {
                int counter = maxFrustrumPlanes;

                for (int j = 0; j < maxFrustrumPlanes; j++)
                {
                    if (planes[j].GetSide(currentObject.gameObject.transform.TransformPoint(currentObject.meshFilter.mesh.vertices[i])))
                    {
                        counter--;
                    }
                }

                if (counter == 0)
                {
                    Debug.Log("Está adentro vert ");
                    currentObject.gameObject.SetActive(true);
                    break;
                }
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
        if (!Application.isPlaying)
        {
            return;
        }
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

        if (showPoints)
        {
            for (int i = 0; i < maxTestObjests; i++)
            {
                if (testObjets[i].gameObject.active)
                {
                    DrawVert(testObjets[i]);
                }
            }
        }
    }
    public void DrawPlane(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(p1, p3);
        //Gizmos.DrawLine(p2, p4);
        Gizmos.color = Color.green;
    }
    public void DrawAABB(ref testObject currentObject)
    {

        Gizmos.color = Color.blue;


        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(currentObject.aabb[i], 0.05f);
        }


        // Draw the AABB Box 
        Gizmos.DrawLine(currentObject.aabb[0], currentObject.aabb[1]);
        Gizmos.DrawLine(currentObject.aabb[1], currentObject.aabb[3]);
        Gizmos.DrawLine(currentObject.aabb[3], currentObject.aabb[2]);
        Gizmos.DrawLine(currentObject.aabb[2], currentObject.aabb[0]);
        Gizmos.DrawLine(currentObject.aabb[0], currentObject.aabb[4]);
        Gizmos.DrawLine(currentObject.aabb[4], currentObject.aabb[5]);
        Gizmos.DrawLine(currentObject.aabb[5], currentObject.aabb[7]);
        Gizmos.DrawLine(currentObject.aabb[7], currentObject.aabb[6]);
        Gizmos.DrawLine(currentObject.aabb[6], currentObject.aabb[4]);
        Gizmos.DrawLine(currentObject.aabb[7], currentObject.aabb[3]);
        Gizmos.DrawLine(currentObject.aabb[6], currentObject.aabb[2]);
        Gizmos.DrawLine(currentObject.aabb[5], currentObject.aabb[1]);

        Gizmos.color = Color.green;



    }
    public void DrawVert(testObject currentObject)
    {

        Gizmos.color = Color.red;

        MeshFilter mesh = currentObject.gameObject.GetComponent<MeshFilter>();

        for (int i = 0; i < mesh.mesh.vertices.Length; i++)
        {
            Gizmos.DrawSphere(currentObject.gameObject.transform.TransformPoint(mesh.mesh.vertices[i]), 0.05f);
        }

        Gizmos.color = Color.green;
    }
}