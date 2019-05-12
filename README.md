# Graphics-Raycast
GPU-based raycaster for Unity which raycasts against MeshRenderers, rather than colliders. 

This method requires reading from the GPU to CPU and is much slower, but still has its uses.

![alt text](https://i.imgur.com/625lQ2s.gif "")

Instructions
------------

Call the static function, like you would with [Physics.Raycast](https://docs.unity3d.com/ScriptReference/Physics.Raycast.html):

`GraphicsRaycast.Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask = -1);`

Optionally, you can draw a gizmo in a similar manner:
`GraphicsRaycast.DrawGizmo(bool hasHit, Vector3 origin, Vector3 direction, RaycastHit hit, float maxDistance = 25f, float size = 1f);`

Limitations
-------
- Not compatible with any Scriptable Render Pipelines
- Does not work with any geometry drawed through `Graphics.DrawMeshInstanced`.
- Does not work with objects using Unlit or Transparent materials.
- Won't work when called through OnSceneGUI (the active render texture apparently cannot be modified during this stage).

License
-------

MIT License (see [LICENSE](LICENSE.md))
