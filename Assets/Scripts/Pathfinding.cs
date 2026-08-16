using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int width;
    int height;
    float windAngleSizeMultiplier;
    float noiseFactor;
    public float timeFactor;
    public int windStrength;
    public int distanceCost;
    public int textureDetail;
    public MapCreator creator;
    public List<Vector2> vertices;
    public List<FollowPath> airplanes;
    Vector2Int startPos;
    Vector2Int endPos;
    void Start()
    {
        width = creator.width; height = creator.height; windAngleSizeMultiplier = creator.windAngleSizeMultiplier; noiseFactor = creator.noiseFactor;
        startPos = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        endPos = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        StartCoroutine( Process());
    }

    // Update is called once per frame
    void Update()
    {
    }
    public IEnumerator Process()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Starting pathfinding");
        int index = 0;
        for (int i = 0; i < creator.airports.Count; i++)
        {
            for (int j = 0; j < creator.airports.Count; j++)
            {
                if (i == j) { continue; }
                path = Pathfind(creator.airports[i], creator.airports[j]);
                vertices = SmoothPath(path);
                ShowPath(vertices, index);
                airplanes[index].NewPath(vertices);
                index++;
            }
        }
    }
    // A* pathfinding algorithm
    public List<Vector2Int> Pathfind(Vector2Int startPos, Vector2Int endPos)
    {
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        Dictionary<Vector2Int, Node> nodeLookup = new Dictionary<Vector2Int, Node>();
        Node start = new Node(startPos);
        start.sethCost(endPos, distanceCost);
        nodeLookup.Add(startPos, start);
        open.Add(start);
        while (open.Count > 0)
        {
            Node current = open[0];
            foreach(Node n in open)
            {
                if (n.fCost<current.fCost||(n.fCost==current.fCost&&n.hCost<current.hCost))
                {
                    current = n;
                }
            }
            open.Remove(current);
            closed.Add(current);
            if (current.position == endPos)
            {
                Debug.Log("Path found");
                Node end = current;
                return Retrace(start, end);
            }
            
            int[] deltaX = new int[4] { -1, 0, 0, 1 };
            int[] deltaY =  new int[4] { 0, -1, 1, 0 };
            for(int i = 0; i < 4; i++)
            {
                int newX = deltaX[i]+current.position.x;
                int newY = deltaY[i]+current.position.y;
                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    Node node;
                    try { node = nodeLookup[new Vector2Int(newX, newY)]; }
                    catch (KeyNotFoundException)
                    {
                        float angle = Mathf.Atan2(deltaY[i], deltaX[i]);
                        node = new Node(new Vector2Int(newX, newY));
                        nodeLookup.Add(node.position, node);
                    }
                    if (node.passable && !closed.Contains(node))
                    {
                        float angle = Mathf.Atan2(node.position.y - current.position.y,node.position.x - current.position.x);

                        int weight = GetWeightAtPoint(node.position, angle);
                        int newGCost = current.gCost + distanceCost + weight;

                        if (!open.Contains(node) || newGCost < node.gCost)
                        {
                            node.parent = current;
                            node.gCost = newGCost;
                            node.sethCost(endPos, distanceCost);

                            if (!open.Contains(node))
                            {
                                open.Add(node);
                            }
                        }
                    }
                }
            }
        }
        
        Debug.LogWarning("No path found");
        return null;
    }
    // theta* inspired smoothing
    public List<Vector2> SmoothPath(List<Vector2Int> path)
    {
        List<Vector2> smoothPath = new List<Vector2>();
        for (int i = 0; i < path.Count; i++)
        {
            smoothPath.Add( new Vector2(path[i].x, path[i].y));
        }
        return thetaStar(smoothPath);
    }
    private List<Vector2Int> Retrace(Node start, Node end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = end;
        while (currentNode != start)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Add(start.position);
        path.Reverse();
        return path;
    }
    private List<Vector2> thetaStar(List<Vector2> nodes)
    {
        int origin = 0;
        List<Vector2> result = new List<Vector2>();
        result.Add(nodes[0]);
        if(nodes.Count<3)
        {
            result.Add(nodes[1]);
            return result;
        }
        for (int i = 2; i < nodes.Count; i++)
        {
            float sl = GetWeightOfStraightLine(nodes[origin], nodes[i]);    
            float pa = GetWeightOfPath(nodes, origin, i);
           
            if (sl <= pa)
            {
                if (i == nodes.Count - 1)
                {
                    result.Add(nodes[i]);
                    return result;
                }
                continue;
            }
            else
            {
                result.Add(nodes[i-1]);
                origin = i-1;
            }
            if (i == nodes.Count - 1)
            {
                result.Add(nodes[i]);
                return result;
            }
        }
        return result;
    }
    private float GetWeightOfStraightLine(Vector2 start, Vector2 end)
    {
        float angle = Mathf.Atan2(end.y - start.y, end.x - start.x);
        float distance = Vector2.Distance(start, end);
        float weight = 0;
        for (int i = 1; i < distance; i++)
        {
            Vector2 pos = Vector2.Lerp(start, end, i / distance);
            weight +=GetWeightAtPoint(pos, angle)*1;
            weight += distanceCost;
        }
        float dist = distance - (int)distance;
        weight += GetWeightAtPoint(end,angle)*dist;
        weight += dist*distanceCost;
        return weight;
    }
    private float GetWeightOfPath(List<Vector2> nodes, int start, int end)
    {
        float weight = 0;
        for (int i = start+1; i <= end; i++)
        {
            float angle = Mathf.Atan2(nodes[i].y - nodes[i-1].y, nodes[i].x - nodes[i-1].x);
            weight +=(distanceCost + GetWeightAtPoint(nodes[i],angle));
        }
        return weight;
    }
    private int GetWeightAtPoint(Vector2 pos, float angle)
    {
        return GetNoise(pos)+ GetWind(pos, angle);
    }
    public int weatherCost;
    private int GetNoise(Vector2 pos)
    {
        float val = (WeatherField.GetWeatherAtPoint(pos.x / noiseFactor, pos.y / noiseFactor, width, height,timeFactor) * weatherCost);
        return (int)val;
    }
    private int GetWind(Vector2 pos, float angle)
    {
        float val = (WindField.GetWindAtPoint(pos.x / noiseFactor, pos.y / noiseFactor, width, height, timeFactor) * windStrength);
        float windAngle = WindField.GetWindAngleAtPoint(pos.x / noiseFactor*windAngleSizeMultiplier, pos.y / noiseFactor*windAngleSizeMultiplier, width, height, timeFactor);
        float angleDiff = Mathf.Abs(Mathf.Atan2(Mathf.Sin(windAngle - angle),Mathf.Cos(windAngle - angle)));
        int ret = (int)(((angleDiff / Mathf.PI *2) -1)*val);
        return ret;
    }
    List<Vector2Int> path;
    public Material weatherMaterial;
    public List<LineRenderer> lines;

    void ShowPath(List<Vector2> path,int index)
    {
        LineRenderer line = lines[index];
        line.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            line.SetPosition(i, new Vector3(path[i].x, path[i].y,0));
        }
    }
    public class Node
    {
        public Vector2Int position;
        public int gCost; // Cost from the start node
        public int hCost; // Heuristic cost to the end node
        public bool passable; // Can be moved through
        public Node parent;
        public float fCost => gCost + hCost; // Total cost
        public Node(Vector2Int position, bool passable=true)
        {
            this.position = position;
            gCost = 0;
            hCost = 0;
            parent = null;
            this.passable = passable;
        }
        public void setCosts(Vector2Int end,Node parent, int distanceCost, int weight)
        {
            this.parent = parent;
            sethCost(end, distanceCost);
            setgCost(distanceCost, weight);
        }
        public void sethCost(Vector2Int end, int distanceCost)
        {
            int deltaX = Math.Abs(end.x - position.x);
            int deltaY = Math.Abs(end.y - position.y);
            hCost = distanceCost * (deltaX + deltaY);
        }
        public void setgCost(int distanceCost, int weight)
        {
            int deltaX = Math.Abs(parent.position.x - position.x);
            int deltaY = Math.Abs(parent.position.y - position.y);
            gCost = weight+ parent.gCost + distanceCost;
        }
    }
}
