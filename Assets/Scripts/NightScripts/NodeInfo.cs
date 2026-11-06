using System.Collections.Generic;
using UnityEngine;

public enum AnimatronicAiType
{
    Freddy,
    Chica,
    Bonnie,
    Foxy
}
[System.Serializable]
public class NodePrefab
{
    public GameObject Prefab;
    public AnimatronicAiType Animatronic_AI_Type;
}
[System.Serializable]
public class ConnectedNode
{
    public GameObject Node;
    public ConnectionType Connection;
}

public class NodeInfo : MonoBehaviour
{
    public NodePrefab[] NodePrefabs;
    public List<ConnectedNode> ConnectedNodes = new List<ConnectedNode>();
    public RoomNode[] ExitNode;
    public AnimatronicAPI OccupyingAnimatronic;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw this node.
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.18f);

        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 0.3f,
            gameObject.name
        );

        // Draw every connection from this node.
        foreach (var connection in ConnectedNodes)
        {
            if (connection.Node == null)
                continue;

            // Color the path according to the allowed AI type.
            if (connection.Connection == null)
            {
                Gizmos.color = Color.gray;
            }
            else
            {
                Vector3 start = transform.position;
                Vector3 end = connection.Node.transform.position;
                Vector3 direction = (end - start).normalized;
                // Find a sideways direction to separate the colored paths.
                Vector3 side = Vector3.Cross(direction, Vector3.up).normalized;
                if (side.sqrMagnitude < 0.001f)
                    side = Vector3.Cross(direction, Vector3.right).normalized;

                var allowedTypes = connection.Connection;

                // Draw gray if no animatronic types are assigned.
                if (allowedTypes == null)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(start, end);
                }
                else
                {
                    switch (allowedTypes)
                    {
                        case ConnectionType.Duct:
                            Gizmos.color = Color.grey;
                            break;
                        case ConnectionType.Hallway:
                            Gizmos.color = Color.blue;
                            break;
                        case ConnectionType.Vent:
                            Gizmos.color = Color.green;
                            break;
                        default:
                            Gizmos.color = Color.gray;
                            break;
                    }

                    // Center the lines around the original connection.
                    Vector3 offsetVector = side * 0.08f;

                    Vector3 lineStart = start + offsetVector;
                    Vector3 lineEnd = end + offsetVector;

                    Gizmos.DrawLine(lineStart, lineEnd);

                    // Draw an arrowhead for this animatronic's path.
                    Vector3 arrowPosition = Vector3.Lerp(lineStart, lineEnd, 0.8f);

                    Gizmos.DrawLine(
                        arrowPosition,
                        arrowPosition - direction * 0.25f + side * 0.12f
                    );
                    Gizmos.DrawLine(
                        arrowPosition,
                        arrowPosition - direction * 0.25f - side * 0.12f
                    );
                }
            }
        }
    }
#endif
}
