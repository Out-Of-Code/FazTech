
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class WallMapExporter
{
    private const float PixelsPerUnit = 100f;

    [MenuItem("Tools/Export Wall Map")]
    public static void ExportWallMap()
    {
        GameObject walls = GameObject.Find("Walls");

        if (walls == null)
        {
            Debug.LogError("Couldn't find a GameObject named 'Walls'.");
            return;
        }

        Transform[] cubes = walls.GetComponentsInChildren<Transform>();

        if (cubes.Length <= 1)
        {
            Debug.LogError("No wall cubes found under Walls.");
            return;
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        // Find the overall bounds of the map.
        foreach (Transform cube in cubes)
        {
            if (cube == walls)
                continue;

            Vector3 pos = cube.position;
            Vector3 size = cube.localScale;

            minX = Mathf.Min(minX, pos.x - size.x / 2f);
            maxX = Mathf.Max(maxX, pos.x + size.x / 2f);
            minZ = Mathf.Min(minZ, pos.z - size.z / 2f);
            maxZ = Mathf.Max(maxZ, pos.z + size.z / 2f);
        }

        int width = Mathf.CeilToInt((maxX - minX) * PixelsPerUnit);
        int height = Mathf.CeilToInt((maxZ - minZ) * PixelsPerUnit);

        StringBuilder svg = new StringBuilder();

        svg.AppendLine(
            $"<svg xmlns=\"http://www.w3.org/2000/svg\" " +
            $"width=\"{width}\" height=\"{height}\" " +
            $"viewBox=\"0 0 {width} {height}\">"
        );

        // Draw each wall cube.
        foreach (Transform cube in cubes)
        {
            if (cube == walls)
                continue;

            Vector3 pos = cube.position;
            Vector3 size = cube.localScale;

            float x = (pos.x - size.x / 2f - minX) * PixelsPerUnit;
            float y = (maxZ - (pos.z + size.z / 2f)) * PixelsPerUnit;

            float w = size.x * PixelsPerUnit;
            float h = size.z * PixelsPerUnit;

            svg.AppendLine(
                $"<rect x=\"{x}\" y=\"{y}\" " +
                $"width=\"{w}\" height=\"{h}\" fill=\"black\" />"
            );
        }

        svg.AppendLine("</svg>");

        string path = EditorUtility.SaveFilePanel(
            "Export Wall Map",
            Application.dataPath,
            "WallMap.svg",
            "svg"
        );

        if (string.IsNullOrEmpty(path))
            return;

        File.WriteAllText(path, svg.ToString());

        Debug.Log($"Wall map exported to: {path}");
        EditorUtility.RevealInFinder(path);
    }
}
