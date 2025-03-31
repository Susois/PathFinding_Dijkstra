using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private MapGenerator _mapGenerator;

    private void Awake()
    {
        _mapGenerator = FindObjectOfType<MapGenerator>();
    }

    Queue<Tile> Dijkstra(Tile start, Tile goal)
    {
        Dictionary<Tile, Tile> NextTileToGoal = new Dictionary<Tile, Tile>();//Determines for each tile where you need to go to reach the goal. Key=Tile, Value=Direction to Goal
        Dictionary<Tile, int> costToReachTile = new Dictionary<Tile, int>();//Total Movement Cost to reach the tile

        PriorityQueue<Tile> frontier = new PriorityQueue<Tile>();
        frontier.Enqueue(start, 0);
        costToReachTile[start] = 0;

        while (frontier.Count > 0)
        {
            Tile curTile = frontier.Dequeue();
            if (curTile == goal)
                break;

            foreach (Tile neighbor in _mapGenerator.Neighbors(curTile))
            {
                int newCost = costToReachTile[curTile] + neighbor._Cost;
                if (costToReachTile.ContainsKey(neighbor) == false || newCost < costToReachTile[neighbor])
                {
                    if (neighbor._TileType != Tile.TileType.Wall)
                    {
                        costToReachTile[neighbor] = newCost;
                        int priority = newCost;
                        frontier.Enqueue(neighbor, priority);
                        NextTileToGoal[neighbor] = curTile;
                        neighbor._Text = costToReachTile[neighbor].ToString();
                    }
                }
            }
        }

        //Get the Path

        //check if tile is reachable
        Stack<Tile> pathStack = new Stack<Tile>();
        Tile pathTile = goal;
        while (pathTile != start)
        {
            pathStack.Push(pathTile);
            pathTile = NextTileToGoal[pathTile];
        }
        pathStack.Push(start); // Đưa start vào stack để đảm bảo có đủ đường đi

        // Chuyển từ Stack sang Queue để có thứ tự từ start đến goal
        Queue<Tile> path = new Queue<Tile>();
        while (pathStack.Count > 0)
        {
            path.Enqueue(pathStack.Pop());
        }

        return path;
    }

    public Queue<Tile> FindPath(Tile end, Tile start)
    {
        switch (_currentAlgorithm)
        {
            case Algorithm.Dijkstra:
                return Dijkstra(end, start);
        }

        return null;
    }


    int Distance(Tile t1, Tile t2)
    {
        return Mathf.Abs(t1._X - t2._X) + Mathf.Abs(t1._Y - t2._Y);
    }

    #region unimportant
    public enum Algorithm
    {
        Dijkstra = 0
    }

    public Algorithm _currentAlgorithm;
    private void Start()
    {
        TMPro.TMP_Dropdown dropDown = FindObjectOfType<TMPro.TMP_Dropdown>();
        dropDown.onValueChanged.AddListener(OnAlgorithmChanged);
        dropDown.value = PlayerPrefs.GetInt("currentAlgorithm");
    }


    public void OnAlgorithmChanged(int algorithmID)
    {
        _currentAlgorithm = (Algorithm)algorithmID;
        FindObjectOfType<PathTester>().RepaintMap();
        PlayerPrefs.SetInt("currentAlgorithm", (int)algorithmID);
        PlayerPrefs.Save();
    }
    #endregion
    
}
