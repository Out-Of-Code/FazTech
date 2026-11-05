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
    public AnimatronicAiType[] AI_Type_Allowed;
}

public class NodeInfo : MonoBehaviour
{
    public NodePrefab[] NodePrefabs;
    public ConnectedNode[] ConnectedNodes;
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
            if (connection.AI_Type_Allowed == null ||
                connection.AI_Type_Allowed.Length == 0)
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

                var allowedTypes = connection.AI_Type_Allowed;

                // Draw gray if no animatronic types are assigned.
                if (allowedTypes == null || allowedTypes.Length == 0)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(start, end);
                }
                else
                {
                    // Draw one slightly offset line for each allowed type.
                    for (int i = 0; i < allowedTypes.Length; i++)
                    {
                        switch (allowedTypes[i])
                        {
                            case AnimatronicAiType.Freddy:
                                Gizmos.color = new Color(0.6f, 0.3f, 0.1f);
                                break;

                            case AnimatronicAiType.Chica:
                                Gizmos.color = Color.yellow;
                                break;

                            case AnimatronicAiType.Bonnie:
                                Gizmos.color = Color.blue;
                                break;

                            case AnimatronicAiType.Foxy:
                                Gizmos.color = Color.red;
                                break;

                            default:
                                Gizmos.color = Color.gray;
                                break;
                        }

                        // Center the lines around the original connection.
                        float offset = (i - (allowedTypes.Length - 1) / 2f) * 0.08f;
                        Vector3 offsetVector = side * offset;

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
}
