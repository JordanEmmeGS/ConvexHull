using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConvexHullConstructor
{
    private Vector3 interiorPoint = new Vector3();
    public (List<Vector3>, List<List<Vector3>>) FindExtremalPoints(List<Vector3> vertices, List<List<Vector3>> edges)
    {
        List<Vector3> extPoints = new List<Vector3>();
        List<List<Vector3>> extEdges = new List<List<Vector3>>();
        
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


    public (List<Vector3>, List<List<Vector3>>) InitialTetrahedron(List<Vector3> vertices)
    {
        List<Vector3> extPoints = new List<Vector3>();
        List<List<Vector3>> extEdges = new List<List<Vector3>>();

        if (vertices.Count == 4 && !AreCoplanar(vertices))
        {
            extPoints = vertices;
            for (int i = 0; i < 4; i++)
            {
                interiorPoint += vertices[i];
                extEdges.Add(new List<Vector3>());
                for (int j = 0; j < 4; j++)
                {
                    if (i != j)
                    {
                        extEdges[i].Add(vertices[j]);
                    }
                }
            }
        }
        interiorPoint = interiorPoint / 4;
        return (extPoints, extEdges);
    }

    private void IsInHull()
    {
        
    }

    private Vector3 GetOutwardNormal(List<Vector3> triangle)
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
    
    private void FindTangent()
    {
        
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
    
    private List<Vector3> pointsCoordinates = new List<Vector3>();
    private List<Vector3> hullVertices = new List<Vector3>();
    private List<List<Vector3>> hullEdges = new List<List<Vector3>>();
    
    private List<GameObject> gameExtPoints = new List<GameObject>();
    
    private ConvexHullConstructor convexHull = new ConvexHullConstructor();
    
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
            SpawnOnClick();
            (hullVertices, hullEdges) = convexHull.FindExtremalPoints(pointsCoordinates, hullEdges);
            UpdateExtremal(hullVertices, hullEdges);
        }
    }

    private void SpawnOnClick()
    {
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));

        GameObject point = Instantiate(userPoint) as GameObject;

        point.transform.position = cursorPos;

        if (pointsCoordinates.Count == 0)
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
        }
    }
    
    private int Lexicographic(Vector2 v1, Vector2 v2)
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
    }
    
    private void UpdateExtremal(List<Vector3> vertices,  List<List<Vector3>> edges) 
    {
        for(int i = 0; i < gameExtPoints.Count; i++)
        {
            Destroy(gameExtPoints[i]);
        }

        gameExtPoints = new List<GameObject>();

        for (int i = 0; i < vertices.Count; i++)
        {
            GameObject point = Instantiate(extremalPoint) as GameObject;
            point.transform.position = vertices[i];
            gameExtPoints.Add(point);
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