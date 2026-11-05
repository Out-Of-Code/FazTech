
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraGizmo : MonoBehaviour
{
    [SerializeField, Min(2)] private int GridResolution = 16;
    [SerializeField, Min(0.1f)] private float MaxDistance = 30f;
    [SerializeField] private Color GizmoColor = Color.cyan;
    [SerializeField] private bool DrawGizmos = true;

    private void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;
        Camera[] cameras = GetComponentsInChildren<Camera>(true);

        foreach (Camera cam in cameras)
        {
            DrawCameraGizmo(cam);
        }
    }

    private void DrawCameraGizmo(Camera cam)
    {
        Gizmos.color = Color.darkGray;
        Gizmos.DrawSphere(cam.transform.position, 0.18f);
        int resolution = Mathf.Max(2, GridResolution);
        Vector3[,] points = new Vector3[resolution + 1, resolution + 1];
        bool[,] hitSomething = new bool[resolution + 1, resolution + 1];

        for (int y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float viewportX = (float)x / resolution;
                float viewportY = (float)y / resolution;

                Ray ray = cam.ViewportPointToRay(
                    new Vector3(viewportX, viewportY, 0f)
                );

                if (Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    MaxDistance,
                    cam.cullingMask,
                    QueryTriggerInteraction.Ignore))
                {
                    points[x, y] = hit.point;
                    hitSomething[x, y] = true;
                }
            }
        }

        Gizmos.color = GizmoColor;

        // Draw a wireframe grid across the visible surfaces.
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                if (!hitSomething[x, y])
                    continue;

                // Horizontal edge.
                if (hitSomething[x + 1, y])
                {
                    Gizmos.DrawLine(
                        points[x, y],
                        points[x + 1, y]
                    );
                }

                // Vertical edge.
                if (hitSomething[x, y + 1])
                {
                    Gizmos.DrawLine(
                        points[x, y],
                        points[x, y + 1]
                    );
                }
            }
        }

        // Complete the outer edges of the grid.
        for (int i = 0; i < resolution; i++)
        {
            if (hitSomething[i, resolution] &&
                hitSomething[i + 1, resolution])
            {
                Gizmos.DrawLine(
                    points[i, resolution],
                    points[i + 1, resolution]
                );
            }

            if (hitSomething[resolution, i] &&
                hitSomething[resolution, i + 1])
            {
                Gizmos.DrawLine(
                    points[resolution, i],
                    points[resolution, i + 1]
                );
            }
        }

        // Draw the four main corner rays and their outline.
        Vector3[] cornerViewportPoints =
        {
            new Vector3(0f, 0f, 0f), // Bottom-left
            new Vector3(0f, 1f, 0f), // Top-left
            new Vector3(1f, 1f, 0f), // Top-right
            new Vector3(1f, 0f, 0f)  // Bottom-right
        };

        Vector3[] corners = new Vector3[4];
        bool[] cornerHits = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            Ray ray = cam.ViewportPointToRay(cornerViewportPoints[i]);

            if (Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    MaxDistance,
                    cam.cullingMask,
                    QueryTriggerInteraction.Ignore))
            {
                corners[i] = hit.point;
                cornerHits[i] = true;
            }
            else
            {
                corners[i] = ray.origin + ray.direction * MaxDistance;
            }

            Gizmos.DrawLine(ray.origin, corners[i]);
        }

        // Connect the four corners into an outline.
        for (int i = 0; i < 4; i++)
        {
            int next = (i + 1) % 4;

            if (cornerHits[i] && cornerHits[next])
            {
                Gizmos.DrawLine(corners[i], corners[next]);
            }
        }

        #if UNITY_EDITOR
        Handles.color = GizmoColor;
        Handles.Label(
            cam.transform.position + Vector3.up * 0.3f,
            cam.name
        );
        #endif
    }
}
