using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch 
{
    public static BFSResult BFSGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints, bool skipObstacle = true)
    {
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();
        
        nodesToVisitQueue.Enqueue(startPoint);
        costSoFar.Add(startPoint, 0);
        visitedNodes.Add(startPoint, null);

        while (nodesToVisitQueue.Count > 0)
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();  // 다음 이동해야할 Hex 타일을 꺼내옴
            
            foreach (var neighbourPosition in hexGrid.GetNeighboursFor(currentNode)) // 꺼내온 Hex 타일 기준으로 인접 Hex 노드 좌표 가져옴
            {
                if (skipObstacle)
                {
                    if (hexGrid.GetTileAt(neighbourPosition).IsObstacle()) // 만약 인접 Hex 노드가 장애물 또는 Water인 경우 Pass 
                    {
                        continue;
                    }
                }

                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost; // 인접 Hex 노드의 거리 비용을 가져옴
                int currentCost = costSoFar[currentNode];                     //  현재 위치까지 Hex 노드의 거리 비용을 가져옴
                int newCost = currentCost + nodeCost;                        // 현재 위치에서 neighbourPosition 노드 위치까지 비용을 newCost에 저장

                if (newCost <= movementPoints) // 계산된 거리비용이 현재 이동할수 있는 거리비용보다 작거나 같은경우 (즉 이동가능한 비용)
                {
                    if (!visitedNodes.ContainsKey(neighbourPosition)) // 인접 Hex 노드에 방문하지 않은 경우
                    {
                        visitedNodes[neighbourPosition] = currentNode; // 현재 인접한 Hex 노드위치에 현재 Hex 노드위치를 넣음
                        costSoFar[neighbourPosition] = newCost;       // 비용도 현재까지의 거리비용을 넣음
                        nodesToVisitQueue.Enqueue(neighbourPosition); // 현재 위치를 기록하기 위해 현재 Hex 노드위치 좌표를 Queue에 넣음
                    }
                    else if (costSoFar[neighbourPosition] > newCost) // 새로 발견한 Hex 노드의 거리비용이 더 작은경우 
                    {
                        costSoFar[neighbourPosition] = newCost; // 지금까지의 거리 비용을 더 작은 비용으로 업데이트
                        visitedNodes[neighbourPosition] = currentNode; // 마지막으로 알아봤던 인접 Hex 노드위치를 더 거리비용이 작은 현재 Hex 노드를 넣음
                    }
                }
            }
        }

        return new BFSResult { VisitedNodesDictionary = visitedNodes };
    }

    public static List<Vector3Int> GeneratePathBFS(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> visitedNodesDictionary)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        while (visitedNodesDictionary[current] != null)
        {
            path.Add(visitedNodesDictionary[current].Value);
            current = visitedNodesDictionary[current].Value;
        }

        path.Reverse();
        return path.Skip(1).ToList();
    }
}

public struct BFSResult
{
    public Dictionary<Vector3Int, Vector3Int?> VisitedNodesDictionary;

    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if (VisitedNodesDictionary.ContainsKey(destination) == false)
        {
            return new List<Vector3Int>();
        }

        return GraphSearch.GeneratePathBFS(destination, VisitedNodesDictionary);
    }

    public bool IsHexPositionInRange(Vector3Int position)
    {
        return VisitedNodesDictionary.ContainsKey(position);
    }

    public IEnumerable<Vector3Int> GetRangePositions() => VisitedNodesDictionary.Keys;
}