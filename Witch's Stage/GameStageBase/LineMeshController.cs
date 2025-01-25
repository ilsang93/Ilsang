using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineMeshController : MonoBehaviour
{
    private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private float thickness = 0.1f;

    void Start()
    {
    }

    public void GenerateLine(Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 direction = (endPoint - startPoint).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * thickness / 2f;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = startPoint - perpendicular; // Bottom-left
        vertices[1] = startPoint + perpendicular; // Top-left
        vertices[2] = endPoint + perpendicular;   // Top-right
        vertices[3] = endPoint - perpendicular;   // Bottom-right

        int[] triangles = new int[6]
        {
            0, 1, 2, // First triangle
            0, 2, 3  // Second triangle
        };

        mesh = new()
        {
            vertices = vertices,
            triangles = triangles
        };

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
        GetComponent<MeshRenderer>().sortingLayerName = "Stage_Node";
        GetComponent<MeshRenderer>().sortingOrder = 5;

        mesh.RecalculateBounds();
    }
}