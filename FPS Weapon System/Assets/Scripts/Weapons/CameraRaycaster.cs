using UnityEngine;
using System.Collections;

public class CameraRaycaster : MonoBehaviour {

    public LayerMask hitLayers;

    /// <summary>
    /// Raycast on specified direction.
    /// </summary>
    /// <param name="dir">Direction to raycast on.</param>
    /// <param name="dist">Distance/Length of ray.</param>
    /// <returns>RaycastHit instance contains information about object hit otherwise Null</returns>
    public bool RayCast(Vector3 dir, float dist, out RaycastHit hit)
    {
        Ray ray = new Ray(transform.position, dir);
        return (Physics.Raycast(ray, out hit, dist, hitLayers));
    }

}
