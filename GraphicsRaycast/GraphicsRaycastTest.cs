using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GraphicsRaycastTest : MonoBehaviour
{
    public float maxDistance = 10f;
    public bool hitSomething;

    private RaycastHit hit;

    void Update()
    {
        hitSomething = GraphicsRaycast.Raycast(this.transform.position, this.transform.forward, out hit, maxDistance);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        GraphicsRaycast.DrawGizmo(hitSomething, this.transform.position, this.transform.forward, hit, maxDistance, 1f);
#endif
    }
}
