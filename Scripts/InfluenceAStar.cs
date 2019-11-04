using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class InfluenceAStar
{
    private static List<Node> _openList = new List<Node>();
    private static List<Node> _closedList = new List<Node>();
    private static Node _current;
    private static List<Node> _adjacencies;
    private static readonly int MAX_SEARCHES = 5000;
    public static float MapInfluenceMultiplier = 5f; 
    
    
    
    public static Vector2Int[] GetPath(Vector2Int gridStart, Vector2Int gridEnd, float[][] gridValue, Vector3[][] gridPosition)
    {
        List<Vector2Int> finalPath = new List<Vector2Int>();
        _openList.Clear();
        _closedList.Clear();
        List<Node> path = new List<Node>();
        Node start = new Node(gridStart);
        _openList.Add(start);

        int i = 0;
        while (_openList.Count > 0 && !_closedList.Exists(x => x.Position == gridEnd) && i < MAX_SEARCHES)
        {
            _current = _openList[0];
            _openList.Remove(_current);
            _closedList.Add(_current);
            _adjacencies = GetAdjacentNodes(_current, gridValue, gridPosition);

            foreach (Node n in _adjacencies)
            {
                if(_closedList.Contains(n) || _openList.Contains(n))
                    continue;;
                
                n.Parent = _current;
                n.DistanceToGoal = ManhattanDistance(n.Position, gridEnd);
                
                float mapCost = 1 - gridValue[n.Position.x][n.Position.y]* MapInfluenceMultiplier;

                if (mapCost < 0f)
                    n.Cost = 1 + n.Parent.Cost;
                else
                {
                    n.Cost = 1 + n.Parent.Cost + mapCost;
                    Debug.Log(mapCost);
                }
                    
                
                _openList.Add(n);
                _openList = _openList.OrderBy(node => node.F).ToList();
                i++;
                if (i > MAX_SEARCHES)
                {
                    Debug.Log("Max Search in Astar!");
                    break;
                }
            }
        }

        if (!_closedList.Exists(x => x.Position == gridEnd))
        {
            Debug.Log("Could not find a Path!");
            return new Vector2Int[0];
        }

        Node currentNode = _closedList[_closedList.IndexOf(_current)];
        
        while (currentNode.Parent != null && currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        finalPath = new List<Vector2Int>();

        foreach (Node x in path)
        {
            finalPath.Add(x.Position);
        }

        finalPath.Reverse();
        return finalPath.ToArray();
    }
    
    private static List<Node> GetAdjacentNodes(Node n, float[][] gridValue, Vector3[][] gridPosition)
    {
        List<Node> temp = new List<Node>();

        if(n.Position.y + 1 <= gridValue[n.Position.x].Length)
        {
            if(Math.Abs(gridValue[n.Position.x][n.Position.y + 1]) > 0.1f)
                temp.Add(new Node(new Vector2Int(n.Position.x, n.Position.y + 1)));
        }
        if(n.Position.y - 1 >= 0)
        {
            if(Math.Abs(gridValue[n.Position.x][n.Position.y - 1]) > 0.1f)
                temp.Add(new Node(new Vector2Int(n.Position.x, n.Position.y - 1)));
        }
        if(n.Position.x - 1 >= 0)
        {
            if(Math.Abs(gridValue[n.Position.x - 1][n.Position.y]) > 0.1f)
                temp.Add(new Node(new Vector2Int(n.Position.x - 1, n.Position.y)));
        }
        if(n.Position.x + 1 <= gridValue[n.Position.y].Length)
        {
            if(Math.Abs(gridValue[n.Position.x + 1][n.Position.y]) > 0.1f)
                temp.Add(new Node(new Vector2Int(n.Position.x + 1, n.Position.y)));
        }
 
        return temp;
    }
    private static int ManhattanDistance(Vector2Int start, Vector2Int end)
    {
        checked {
            return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);
        }
    }
}

public class Node
{
    public Vector2Int Position;
    public int DistanceToGoal;
    public float Cost;
    public Node Parent;
    public float F
    {
        get
        {
            if (DistanceToGoal != -1 && Cost != -1)
                return DistanceToGoal + Cost;
            else
                return -1;
        }
    }

    public Node(Vector2Int start)
    {
        Position = start;
        Cost = 1f;
        DistanceToGoal = -1;
        Parent = null;
    }

}


