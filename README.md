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

For it to work in a build, ensure the GraphicRaycastShader.shader file is in the "Always included shaders" list under [Graphics Settings](https://docs.unity3d.com/Manual/class-GraphicsSettings.html). Alternatively, move it into a folder named "Resources".

Limitations
-------
- Not compatible with any Scriptable Render Pipelines (uses SetReplacementShader)
- Does not work with any geometry drawn through `Graphics.DrawMeshInstanced`, as these are outside of the regular render loop.
- Does not work with objects using Unlit or Transparent materials, or other shaders not writing to the depth texture.
- Won't work when called through OnSceneGUI (the active render texture apparently cannot be modified during this stage).

**Future improvements**
- Add override function to raycast from a camera's center. This could simply read the depth- and normals buffer and return the result, and would also work with anything rendered manually through the Graphics API. Very useful for a Depth of Field auto-focus function.

License
-------

MIT License (see [LICENSE](LICENSE.md))
