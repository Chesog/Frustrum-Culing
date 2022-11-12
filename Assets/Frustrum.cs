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

    struct testObjet
    {
        GameObject testObject;
        Vector3[] aabb;
    }

    [SerializeField] Vector3 nearTopLeft;
    [SerializeField] Vector3 nearTopRight;
    [SerializeField] Vector3 nearBottomLeft;
    [SerializeField] Vector3 nearBottomRight;

    [SerializeField] Vector3 farTopLeft;
    [SerializeField] Vector3 farTopRight;
    [SerializeField] Vector3 farBottomLeft;
    [SerializeField] Vector3 farBottomRight;

    [SerializeField] GameObject[] TestObjests = new GameObject[maxTestObjests];
    [SerializeField] Vector3[] aabb = new Vector3[aabbPoints];
    [SerializeField] Vector3[] aabb1 = new Vector3[aabbPoints];
    [SerializeField] Vector3[] aabb2 = new Vector3[aabbPoints];
    [SerializeField] Vector3[] aabb3 = new Vector3[aabbPoints];
    [SerializeField] Vector3[] aabb4 = new Vector3[aabbPoints];
    [SerializeField] Vector3[] aabb5 = new Vector3[aabbPoints];
    [SerializeField] Vector3[] aabb6 = new Vector3[aabbPoints];
    [SerializeField] Vector3[] aabb7 = new Vector3[aabbPoints];

    [SerializeField] testObjet[] testObjets = new testObjet[maxTestObjests];

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

        for (int i = 0; i < maxTestObjests; i++)
        {
            SetAABB(TestObjests[i],i);
        }
        for (int i = 0; i < maxTestObjests; i++)
        {
            CheckObjetColition(TestObjests[i],i);
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
    public void SetAABB(GameObject currentObject,int number) 
    {

        Vector3 scale = currentObject.transform.localScale / 2;
        Vector3 forward = currentObject.transform.forward;
        Vector3 up = currentObject.transform.up;
        Vector3 right = currentObject.transform.right;

        switch (number)
        {
            case 0:
                for (int i = 0; i < aabbPoints; i++)
                {
                    aabb[i] = currentObject.transform.position;
                }

                aabb[0] += scale.x * right + scale.y * up + scale.z * forward;
                aabb[1] += scale.x * right + scale.y * up + -scale.z * forward;
                aabb[2] += scale.x * right + -scale.y * up + scale.z * forward;
                aabb[3] += scale.x * right + -scale.y * up + -scale.z * forward;
                aabb[4] += -scale.x * right + scale.y * up + scale.z * forward;
                aabb[5] += -scale.x * right + scale.y * up + -scale.z * forward;
                aabb[6] += -scale.x * right + -scale.y * up + scale.z * forward;
                aabb[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
                break;
            case 1:
                for (int i = 0; i < aabbPoints; i++)
                {
                    aabb1[i] = currentObject.transform.position;
                }

                aabb1[0] += scale.x * right + scale.y * up + scale.z * forward;
                aabb1[1] += scale.x * right + scale.y * up + -scale.z * forward;
                aabb1[2] += scale.x * right + -scale.y * up + scale.z * forward;
                aabb1[3] += scale.x * right + -scale.y * up + -scale.z * forward;
                aabb1[4] += -scale.x * right + scale.y * up + scale.z * forward;
                aabb1[5] += -scale.x * right + scale.y * up + -scale.z * forward;
                aabb1[6] += -scale.x * right + -scale.y * up + scale.z * forward;
                aabb1[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
                break;
            case 2:
                for (int i = 0; i < aabbPoints; i++)
                {
                    aabb2[i] = currentObject.transform.position;
                }

                aabb2[0] += scale.x * right + scale.y * up + scale.z * forward;
                aabb2[1] += scale.x * right + scale.y * up + -scale.z * forward;
                aabb2[2] += scale.x * right + -scale.y * up + scale.z * forward;
                aabb2[3] += scale.x * right + -scale.y * up + -scale.z * forward;
                aabb2[4] += -scale.x * right + scale.y * up + scale.z * forward;
                aabb2[5] += -scale.x * right + scale.y * up + -scale.z * forward;
                aabb2[6] += -scale.x * right + -scale.y * up + scale.z * forward;
                aabb2[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
                break;
            case 3:
                for (int i = 0; i < aabbPoints; i++)
                {
                    aabb3[i] = currentObject.transform.position;
                }

                aabb3[0] += scale.x * right + scale.y * up + scale.z * forward;
                aabb3[1] += scale.x * right + scale.y * up + -scale.z * forward;
                aabb3[2] += scale.x * right + -scale.y * up + scale.z * forward;
                aabb3[3] += scale.x * right + -scale.y * up + -scale.z * forward;
                aabb3[4] += -scale.x * right + scale.y * up + scale.z * forward;
                aabb3[5] += -scale.x * right + scale.y * up + -scale.z * forward;
                aabb3[6] += -scale.x * right + -scale.y * up + scale.z * forward;
                aabb3[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
                break;
            case 4:
                for (int i = 0; i < aabbPoints; i++)
                {
                    aabb4[i] = currentObject.transform.position;
                }

                aabb4[0] += scale.x * right + scale.y * up + scale.z * forward;
                aabb4[1] += scale.x * right + scale.y * up + -scale.z * forward;
                aabb4[2] += scale.x * right + -scale.y * up + scale.z * forward;
                aabb4[3] += scale.x * right + -scale.y * up + -scale.z * forward;
                aabb4[4] += -scale.x * right + scale.y * up + scale.z * forward;
                aabb4[5] += -scale.x * right + scale.y * up + -scale.z * forward;
                aabb4[6] += -scale.x * right + -scale.y * up + scale.z * forward;
                aabb4[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
                break;
            case 5:
                for (int i = 0; i < aabbPoints; i++)
                {
                    aabb5[i] = currentObject.transform.position;
                }

                aabb5[0] += scale.x * right + scale.y * up + scale.z * forward;
                aabb5[1] += scale.x * right + scale.y * up + -scale.z * forward;
                aabb5[2] += scale.x * right + -scale.y * up + scale.z * forward;
                aabb5[3] += scale.x * right + -scale.y * up + -scale.z * forward;
                aabb5[4] += -scale.x * right + scale.y * up + scale.z * forward;
                aabb5[5] += -scale.x * right + scale.y * up + -scale.z * forward;
                aabb5[6] += -scale.x * right + -scale.y * up + scale.z * forward;
                aabb5[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
                break;
            case 6:
                for (int i = 0; i < aabbPoints; i++)
                {
                    aabb6[i] = currentObject.transform.position;
                }

                aabb6[0] += scale.x * right + scale.y * up + scale.z * forward;
                aabb6[1] += scale.x * right + scale.y * up + -scale.z * forward;
                aabb6[2] += scale.x * right + -scale.y * up + scale.z * forward;
                aabb6[3] += scale.x * right + -scale.y * up + -scale.z * forward;
                aabb6[4] += -scale.x * right + scale.y * up + scale.z * forward;
                aabb6[5] += -scale.x * right + scale.y * up + -scale.z * forward;
                aabb6[6] += -scale.x * right + -scale.y * up + scale.z * forward;
                aabb6[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
                break;
            case 7:
                for (int i = 0; i < aabbPoints; i++)
                {
                    aabb7[i] = currentObject.transform.position;
                }

                aabb7[0] += scale.x * right + scale.y * up + scale.z * forward;
                aabb7[1] += scale.x * right + scale.y * up + -scale.z * forward;
                aabb7[2] += scale.x * right + -scale.y * up + scale.z * forward;
                aabb7[3] += scale.x * right + -scale.y * up + -scale.z * forward;
                aabb7[4] += -scale.x * right + scale.y * up + scale.z * forward;
                aabb7[5] += -scale.x * right + scale.y * up + -scale.z * forward;
                aabb7[6] += -scale.x * right + -scale.y * up + scale.z * forward;
                aabb7[7] += -scale.x * right + -scale.y * up + -scale.z * forward;
                break;
        }
    }
    public void CheckObjetColition(GameObject currentObject,int number) 
    {
        bool isInside = false;

        for (int i = 0; i < aabbPoints; i++)
        {
            int counter = maxFrustrumPlanes;

            for (int j = 0; j < maxFrustrumPlanes; j++)
            {
                switch (number)
                {
                    case 0:
                        if (planes[j].GetSide(aabb[i]))
                        {
                            counter--;
                        }
                        break;
                    case 1:
                        if (planes[j].GetSide(aabb1[i]))
                        {
                            counter--;
                        }
                        break;
                    case 2:
                        if (planes[j].GetSide(aabb2[i]))
                        {
                            counter--;
                        }
                        break;
                    case 3:
                        if (planes[j].GetSide(aabb3[i]))
                        {
                            counter--;
                        }
                        break;
                    case 4:
                        if (planes[j].GetSide(aabb4[i]))
                        {
                            counter--;
                        }
                        break;
                    case 5:
                        if (planes[j].GetSide(aabb5[i]))
                        {
                            counter--;
                        }
                        break;
                    case 6:
                        if (planes[j].GetSide(aabb6[i]))
                        {
                            counter--;
                        }
                        break;
                    case 7:
                        if (planes[j].GetSide(aabb7[i]))
                        {
                            counter--;
                        }
                        break;
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
            if (!currentObject.activeSelf)
            {
                currentObject.SetActive(true);
            }
        }
        else
        {
            if (currentObject.activeSelf)
            {
                Debug.Log("Está afuera");
                currentObject.SetActive(false);
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

        DrawAABB();
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
    public void DrawAABB() 
    {

        Gizmos.color = Color.blue;

        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(aabb[i], 0.2f);
        }
        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(aabb1[i], 0.2f);
        }
        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(aabb2[i], 0.2f);
        }
        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(aabb3[i], 0.2f);
        }
        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(aabb4[i], 0.2f);
        }
        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(aabb5[i], 0.2f);
        }
        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(aabb6[i], 0.2f);
        }
        for (int i = 0; i < aabbPoints; i++)
        {
            Gizmos.DrawSphere(aabb7[i], 0.2f);
        }
        Gizmos.color = Color.green;
    }
}
