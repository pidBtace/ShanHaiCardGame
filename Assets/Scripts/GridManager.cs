using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : Manager<GridManager>
{
    public GameObject terrainGrid;

    public int laneCount = 4;

    protected Graph graph;
    protected Dictionary<Team, int> startPositionPerTeam;
    
    List<Tile> allTiles = new List<Tile>();
    protected void Awake()
    {
        base.Awake();
        allTiles = terrainGrid.GetComponentsInChildren<Tile>().ToList();
        
        InitializeGraph();
        startPositionPerTeam = new Dictionary<Team, int>();
        startPositionPerTeam.Add(Team.Team1, 0);
        startPositionPerTeam.Add(Team.Team2, graph.Nodes.Count -1);
    }

    public Node GetFreeNode(Team forTeam)
    {
        for (int lane = 0; lane < laneCount; lane++)
        {
            Node n = GetFreeNode(forTeam, lane);
            if (n != null)
                return n;
        }
        return null;
    }

    public Node GetFreeNode(Team forTeam, int lane)
    {
        int startIndex = startPositionPerTeam[forTeam];
        int direction = startIndex == 0 ? 1 : -1;
        int currentIndex = startIndex;

        while (currentIndex >= 0 && currentIndex < graph.Nodes.Count)
        {
            Node node = graph.Nodes[currentIndex];
            if (node.lane == lane && !node.IsOccupied)
                return node;

            currentIndex += direction;
        }

        return null;
    }

    public List<Node> GetPath(Node from, Node to)
    {
        return graph.GetShortestPath(from, to);
    }

    public List<Node> GetNodesCloseTo(Node to)
    {
        return graph.Neighbors(to);
    }

    public Node GetNodeForTile(Tile t)
    {
        var allNodes = graph.Nodes;

        for (int i = 0; i < allNodes.Count; i++)
        {
            if (t.transform.GetSiblingIndex() == allNodes[i].index)
            {
                return allNodes[i];
            }
        }

        return null;
    }
    
    private void InitializeGraph()
    {
        graph = new Graph();

        for (int i = 0; i < allTiles.Count; i++)
        {
            Vector3 place = allTiles[i].transform.position;
            int lane = i % laneCount;
            graph.AddNode(place, lane);
        }

        var allNodes = graph.Nodes;
        foreach (Node from in allNodes)
        {
            foreach (Node to in allNodes)
            {
                if (Vector3.Distance(from.worldPosition, to.worldPosition) < 1f && from != to)
                {
                    graph.AddEdge(from, to);
                }
            }
        }
    }

    public int fromIndex = 0;
    public int toIndex = 0;

    private void OnDrawGizmos()
    {
        if (graph == null)
            return;

        var allEdges = graph.Edges;
        if (allEdges == null)
            return;

        foreach(Edge e in allEdges)
        {
            Debug.DrawLine(e.from.worldPosition, e.to.worldPosition, Color.black, 100);
        }

        var allNodes = graph.Nodes;
        if (allNodes == null)
            return;

        foreach (Node n in allNodes)
        {
            Gizmos.color = n.IsOccupied ? Color.red : Color.green;
            Gizmos.DrawSphere(n.worldPosition, 0.1f);
            
        }

        if (fromIndex >= allNodes.Count || toIndex >= allNodes.Count)
            return;

        List<Node> path = graph.GetShortestPath(allNodes[fromIndex], allNodes[toIndex]);
        if (path.Count > 1)
        {
            for (int i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i - 1].worldPosition, path[i].worldPosition, Color.red, 10);
            }
        }
    }
}
