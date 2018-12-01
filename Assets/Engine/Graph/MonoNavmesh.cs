using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navmesh
{
    [System.Serializable]
    public class Node
    {
        public Vector2 position;
        public float radius;
        [System.Serializable]
        public struct Connection
        {
            public Node otherNode;
            public float cost;
        }
        public List<Connection> connections;
        public Node AddConnection(Node node, float cost)
        {
            var c = new Connection();
            c.otherNode = node;
            c.cost = cost;
            connections.Add(c);
            return this;
        }

        public bool visited;
    }
    public List<Node> nodes;

    public List<Node> FindPath(Node nodeStart, Node nodeEnd)
    {
        foreach (var it in nodes)
            it.visited = false;
        return FindPath(nodeStart, nodeEnd, new List<Node>());
    }
    List<Node> FindPath(Node current, Node nodeEnd, List<Node> path)
    {
        if(current == nodeEnd)
        {
            path.Add(current);
            return path;
        }

        current.connections.Sort(
                delegate (Node.Connection node1, Node.Connection node2)
                {
                    float distance1 = (current.position - node1.otherNode.position).sqrMagnitude * node1.cost * node1.cost;
                    float distance2 = (current.position - node2.otherNode.position).sqrMagnitude * node2.cost * node2.cost;

                    if (distance1 == distance2)
                        return 0;
                    else if (distance1 < distance2)
                        return -1;
                    else
                        return 1;
                }
            );

        foreach(var it in current.connections)
            if(!it.otherNode.visited)
        {
            it.otherNode.visited = true;
            var _path = FindPath(it.otherNode, nodeEnd, path);
            if(_path != null)
            {
                _path.Add(current);
                return _path;
            }
        }

        return null;
    }

    public Node AddNode(Vector2 position, float radius)
    {
        var n = new Node();
        n.position = position;
        n.radius = radius;
        nodes.Add(n);
        return n;
    }


}
