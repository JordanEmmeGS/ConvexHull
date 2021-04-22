using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConvexHull
{
    private Vector3 interiorPoint = new Vector3();
    private List<Vector3> vertices = new List<Vector3>();
    private List<List<Vector3>> neighbors = new List<List<Vector3>>();
    private List<Vector3[]> faces = new List<Vector3[]>();

    public List<Vector3> Vertices
    {
        get { return vertices; }
    }

    public List<Vector3[]> Faces
    {
        get { return faces; }
    }

    public void UpdateHull(Vector3 point)
    {
        if (vertices.Count < 3)
        {
            vertices.Add(point);
        }

        else if (vertices.Count == 3)
        {
            vertices.Add(point);
            if (!AreCoplanar(vertices))
            {
                InitialTetrahedron();
            }
        }

        else
        {
            if (!IsInHull(point))
            {
                List<Vector3> tangentVertices = new List<Vector3>();
                List<Vector3> invisibleVertices = new List<Vector3>();
                
                for (int i = 0; i < vertices.Count; i++)
                {
                    string visibility = VertexVisibility(vertices[i], point);
                    if (visibility == "tangent")
                    {
                        tangentVertices.Add(vertices[i]);
                    }
                    else if (visibility == "invisible")
                    {
                        invisibleVertices.Add(vertices[i]);
                    }
                }

                for (int i = 0; i < invisibleVertices.Count; i++)
                {
                    DeleteHullVertex(invisibleVertices[i]);
                }

                vertices.Add(point);
                int newPointIndex = vertices.Count - 1;

                neighbors.Add(tangentVertices);

                Vector3 initialVertex = tangentVertices[0];
                Vector3 toTreatVertex = tangentVertices[0];
                tangentVertices.Remove(toTreatVertex);
                
                while (tangentVertices.Count > 0)
                {
                    int vertexIndex = vertices.FindIndex(element => element == toTreatVertex);
                    List<Vector3> vertexNeighbors = neighbors[vertexIndex];
                    
                    for (int i = 0; i < vertexNeighbors.Count; i++)
                    {
                        if (tangentVertices.Contains(vertexNeighbors[i]))
                        {
                            Vector3[] newFace = new Vector3[] {toTreatVertex, vertexNeighbors[i], point};
                            faces.Add(newFace);
                            toTreatVertex = vertexNeighbors[i];
                            tangentVertices.Remove(toTreatVertex);
                            break;
                        }

                        neighbors[vertexIndex].Add(point);
                    }
                }

                Vector3[] finalFace = new Vector3[] {toTreatVertex, initialVertex, point};

            }
        }
    }
    public (List<Vector3>, List<Vector3[]>) FindExtremalPoints(List<Vector3> vertices, List<Vector3[]> edges)
    {
        List<Vector3> extPoints = new List<Vector3>();
        List<Vector3[]> extEdges = new List<Vector3[]>();
        
        if (vertices.Count == 4 && !AreCoplanar(vertices))
        {
            for(int i = 0; i < 4; i++)
            {
                extPoints.Add(vertices[i]);
                /*for (int j = 0; j < 4; j++)
                {
                    if (i != j)
                    {
                        extEdges[i].Add(vertices[j]);
                    }
                }*/
            }
            return (extPoints, extEdges);
        }
        else
        {
            return (extPoints, extEdges);
        }
        
    }


    private void InitialTetrahedron()
    {
        for (int i = 0; i < 4; i++)
        {
            interiorPoint += vertices[i];
            Vector3[] triangle = {vertices[(i + 1) % 4], vertices[(i + 2) % 4], vertices[(i + 3) % 4]};
            faces.Add(triangle);
            List<Vector3> vertexNeighbor = new List<Vector3>
                {vertices[(i + 1) % 4], vertices[(i + 2) % 4], vertices[(i + 3) % 4]};
            neighbors.Add(vertexNeighbor);
        }
        interiorPoint = interiorPoint / 4;
    }

    private bool IsUnderPlane(Vector3 point, Vector3[] triangle)
    {
        Vector3 normal = OutwardNormal(triangle);
        return (Vector3.Dot(point - triangle[0], normal) < 0);
    }

    private bool IsInHull(Vector3 point)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            Vector3[] triangle = faces[i];
            if (!IsUnderPlane(point, triangle))
            {
                return false;
            }
        }
        return true;
    }
    
    private string VertexVisibility(Vector3 vertex, Vector3 source)
    {
        string visibility = "unknown";
        for (int i = 0; i < faces.Count; i++)
        {
            Vector3[] triangle = faces[i];
            if (triangle[0] == vertex || triangle[1] == vertex || triangle[2] == vertex)
            {
                if (!IsUnderPlane(source, triangle))
                {
                    if (visibility == "unknown")
                    {
                        visibility = "invisible";
                    }
                    else if (visibility == "visible")
                    {
                        visibility = "tangent";
                        break;
                    }
                }
                else
                {
                    if (visibility == "unknown")
                    {
                        visibility = "visible";
                    }
                    else if (visibility == "invisible")
                    {
                        visibility = "tangent";
                        break;
                    }
                }
            }
        }
        return visibility;
    }

    private void DeleteHullVertex(Vector3 vertex)
    {
        int vertexIndex = vertices.FindIndex(element => element == vertex);
        
        for (int i = 0; i < neighbors[vertexIndex].Count; i++)
        {
            Vector3 neighbor = neighbors[vertexIndex][i];
            int neighborIndex = vertices.FindIndex(element => element == vertex);
            neighbors[neighborIndex].Remove(vertex);
        }
        
        neighbors.RemoveAt(vertexIndex);
        vertices.Remove(vertex);
        
        for (int i = 0; i < faces.Count; i++)
        {
            Vector3[] triangle = faces[i];
            if (triangle[0] == vertex || triangle[1] == vertex || triangle[2] == vertex)
            {
                faces.RemoveAt(i);
            }
        }
    }

    private Vector3 OutwardNormal(Vector3[] triangle)
    {
        Vector3 v1 = triangle[1] - triangle[0];
        Vector3 v2 = triangle[2] - triangle[0];
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        if (Vector3.Dot(interiorPoint - triangle[0], normal) > 0)
        {
            return -normal;
        }
        else
        {
            return normal;
        }
    }

    private bool AreCoplanar(List<Vector3> points)
    {
        Vector3 col1 = points[1] - points[0];
        Vector3 col2 = points[2] - points[0];
        Vector3 col3 = points[3] - points[0];
        return (col1.x * (col2.y * col3.z - col3.y * col2.z) 
            - col1.y * (col2.x * col3.z - col3.x * col2.z) 
            + col1.z * (col2.x * col3.y - col3.x * col2.y) == 0);
    }
    
    /*public List<Vector2> FindExtremalPoints(List<Vector2> coordinates)
    {
        List<Vector2> extremalPoints = new List<Vector2>();
        
        if (coordinates.Count <= 3)
        {
            extremalPoints = coordinates;
        }
        else
        {
            List<Vector2> topPoints = new List<Vector2>();

            for (int i = 0; i <= coordinates.Count - 1; i++)
            {
                while (topPoints.Count >= 2 && !RightTurn(topPoints[topPoints.Count - 2], topPoints[topPoints.Count - 1], coordinates[i]))
                {

                    topPoints.RemoveAt(topPoints.Count - 1);
                }

                topPoints.Add(coordinates[i]);
            }

            List<Vector2> bottomPoints = new List<Vector2>();

            for (int i = coordinates.Count - 1; i >= 0; i--)
            {
                while (bottomPoints.Count >= 2 && !RightTurn(bottomPoints[bottomPoints.Count - 2], bottomPoints[bottomPoints.Count - 1], coordinates[i]))
                {
                    bottomPoints.RemoveAt(bottomPoints.Count - 1);
                }

                bottomPoints.Add(coordinates[i]);

            }

            topPoints.RemoveAt(topPoints.Count - 1);
            bottomPoints.RemoveAt(bottomPoints.Count - 1);
            
            foreach (Vector2 coordinate in topPoints)
            {
                extremalPoints.Add(coordinate);
            }

            foreach (Vector2 coordinate in bottomPoints)
            {
                extremalPoints.Add(coordinate);
            }
        }

        return extremalPoints;
    }*/
    
    private bool RightTurn(Vector2 v1, Vector2 v2, Vector2 v3)
    
    {
        return ((v3.x - v2.x) * (v2.y - v1.y) - (v3.y - v2.y) * (v2.x - v1.x) < 0);
    }
}
public class SpawnCircle : MonoBehaviour
{
    public GameObject extremalPoint;
    public GameObject userPoint;
    
    /*private List<Vector3> pointsCoordinates = new List<Vector3>();
    private List<Vector3> hullVertices = new List<Vector3>();
    private List<Vector3[]> hullFaces = new List<Vector3[]>();*/
    
    private List<GameObject> extremalSpheres = new List<GameObject>();
    
    private ConvexHull convexHull = new ConvexHull();
    
    void Start()
    {
        /*LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.04f;*/
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = SpawnOnClick();
            convexHull.UpdateHull(point);
            UpdateExtremalSpheresRender();
        }
    }

    private Vector3 SpawnOnClick()
    {
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));

        GameObject point = Instantiate(userPoint) as GameObject;

        point.transform.position = cursorPos;

        return (cursorPos);

        /*if (pointsCoordinates.Count == 0)
        {
            pointsCoordinates.Add(cursorPos);
        }

        else
        {
            int j = 0;

            while ((j < pointsCoordinates.Count) && Lexicographic(cursorPos, pointsCoordinates[j]) == 1)
            {
                j++;
            }

            pointsCoordinates.Insert(j, cursorPos);
        }*/
    }
    
    /*private int Lexicographic(Vector2 v1, Vector2 v2)
    {
        if (v1.x > v2.x)
        {
            return 1;
        }
        else if (v1.x == v2.x)
        {
            if (v1.y > v2.y)
            {
                return 1;
            }
            else if (v1.y == v2.y)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return -1;
        }
    }*/
    
    private void UpdateExtremalSpheresRender() 
    {
        for(int i = 0; i < extremalSpheres.Count; i++)
        {
            Destroy(extremalSpheres[i]);
        }

        extremalSpheres = new List<GameObject>();

        for (int i = 0; i < convexHull.Vertices.Count; i++)
        {
            GameObject point = Instantiate(extremalPoint) as GameObject;
            point.transform.position = convexHull.Vertices[i];
            extremalSpheres.Add(point);
        }

        /*if (coordinates.Count >= 2)
        {

            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            
            lineRenderer.positionCount = coordinates.Count + 1;

            for (int i = 0; i < coordinates.Count; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(coordinates[i].x, coordinates[i].y, 0.0f));
            }
            lineRenderer.SetPosition(coordinates.Count, new Vector3(coordinates[0].x, coordinates[0].y, 0.0f));
        }*/
    }
}