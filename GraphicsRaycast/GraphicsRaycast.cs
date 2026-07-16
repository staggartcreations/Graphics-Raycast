using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class GraphicsRaycast
{

    private static GameObject RayCamObj;
    private static Camera _RayCam;
    private static Camera RayCam
    {
        get
        {
            if (_RayCam == null) _RayCam = CreateRayCam();
            return _RayCam;
        }
    }

    private static Shader shader { get { return Shader.Find("Hidden/Raycast"); } }
    private static RenderTexture _RT;
    private static RenderTexture RT
    {
        get
        {
            if (_RT == null) _RT = CreateRenderTexture();

            return _RT;
        }
    }
    public static Color outputColor;

    private static Camera CreateRayCam()
    {
        Camera rayCam;

        if (!RayCamObj)
        {
            RayCamObj = GameObject.Find("Graphics Raycast camera");
        }
        if (!RayCamObj)
        {
            RayCamObj = new GameObject();
            rayCam = RayCamObj.AddComponent<Camera>();
        }
        else
        {
            rayCam = RayCamObj.GetComponent<Camera>();
        }

        RayCamObj.hideFlags = HideFlags.HideAndDontSave;

        rayCam.SetReplacementShader(shader, "");
        rayCam.renderingPath = RenderingPath.Forward;
        rayCam.name = "Graphics Raycast camera";

        rayCam.clearFlags = CameraClearFlags.SolidColor;
        rayCam.backgroundColor = Color.clear;

        rayCam.fieldOfView = 1;
        rayCam.nearClipPlane = 0.01f;
        rayCam.depth = -100f;

        rayCam.targetTexture = RT;

        rayCam.useOcclusionCulling = false;
        rayCam.allowHDR = false;
        rayCam.allowMSAA = false;

        rayCam.enabled = false;

        return rayCam;
    }

    private static RenderTexture CreateRenderTexture()
    {
        RenderTexture rt = new RenderTexture(1, 1, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        rt.name = "Graphics Raycast RenderTexture";
        rt.useMipMap = false;
        rt.autoGenerateMips = false;
        rt.filterMode = FilterMode.Point;
        rt.hideFlags = HideFlags.DontSave;

        return rt;
    }

    public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask = -1)
    {
        bool hasHit = false;
        maxDistance = Mathf.Clamp(maxDistance, 0f, 1000f);

        RayCam.transform.position = origin;
        RayCam.transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
        RayCam.cullingMask = layerMask;
        RayCam.farClipPlane = maxDistance;
        RayCam.Render();
        RenderTexture.active = RayCam.targetTexture;

        //CPU texture
        Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;
        tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
        outputColor = tex.GetPixel(0, 0);

        //Consider a miss when the depth equals the far clip plane
        hasHit = (outputColor.a > 0);

        hit = new RaycastHit();

        //Returned distance is in 0-1 range and needs scaling
        float depth = outputColor.a * maxDistance;

        //Set position at distance along forward direction
        hit.point = origin + (direction * depth);

        //Grab initial unscaled normal
        hit.normal = new Vector3(outputColor.r, outputColor.g, outputColor.b);

        //Scale values back to -1 to 1
        hit.normal = Vector3.Scale(hit.normal, Vector3.one * 2f);
        hit.normal = hit.normal - Vector3.one;

        RenderTexture.active = null;
        outputColor.a = 1f; //Useful so gizmos don't turn transparent

        return hasHit;
    }

    /// <summary>
    /// Objects used for raycasting are not saved by default, call this to destroy them anyway. Do not call every frame.
    /// </summary>
    public static void Cleanup()
    {
        if (RayCam) UnityEngine.GameObject.DestroyImmediate(RayCam.gameObject);
    }

    public static void DrawGizmo(bool hasHit, Vector3 origin, Vector3 direction, RaycastHit hit, float maxDistance = 25f, float size = 1f)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawSolidDisc(origin, direction, 0.05f * size);
        UnityEditor.Handles.DrawDottedLine(origin, origin + (direction * maxDistance), 5f);

        if (!hasHit) return;

        UnityEditor.Handles.DrawAAPolyLine(Texture2D.whiteTexture, 3 * size, new Vector3[] { origin, hit.point });

        UnityEditor.Handles.color = new Color(hit.normal.x, hit.normal.y, hit.normal.z);
        UnityEditor.Handles.DrawAAPolyLine(Texture2D.whiteTexture, 3 * size, new Vector3[] { hit.point, hit.point + (hit.normal * 0.66f * size) });
        UnityEditor.Handles.DrawSolidDisc(hit.point, hit.normal, 0.05f * size);

        if(hit.normal != Vector3.zero)
        UnityEditor.Handles.ArrowHandleCap(0, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up), 0.66f * size, EventType.Repaint);
#endif
    }
}
