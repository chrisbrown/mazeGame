﻿using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

namespace MazeGame
{
    public enum cellType{outerWall, innerWall, tile, goal};
    public class BoardManager : MonoBehaviour
    {
        public const int columns = 32;                                         //Number of columns in our game board.
        public const int rows = 32;                                            //Number of rows in our game board.
        private Dictionary<Vector3, Cell> cells = new Dictionary<Vector3,Cell>(); //Set of cells for the maze
        public GameObject exit;                                         //Prefab to spawn for exit.
        public GameObject[] floorTiles;                                 //Array of floor prefabs.
        public GameObject[] wallTiles;                                  //Array of wall prefabs.
        public GameObject cam;
        public GameObject player;

        private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
        private List<Vector3> gridPositions = new List<Vector3>();   //A list of possible locations to place tiles.
        public Vector3 goalPosition;

        //Clears our list gridPositions and prepares it to generate a new board.
        void InitialiseList()
        {
            //Clear our list gridPositions.
            gridPositions.Clear();

            //Loop through x axis (columns).
            for (int x = 1; x < columns - 1; x++)
            {
                //Within each column, loop through y axis (rows).
                for (int y = 1; y < rows - 1; y++)
                {
                    //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }

        Cell getLowestWeight(Cell[] c)
        {
            Cell lowest = c[0];
            foreach (Cell cell in c)
            {
                if (cell.getWeight() < lowest.getWeight())
                    lowest = cell;
            }
            return lowest;
        }

        int numTileNeighbors(Cell c)
        {
            int count = 0;
            foreach (Vector3 v in c.getNeighbors()) {
                if (cells[v].getCellType() == cellType.tile)
                    count++;
            }
            return count;
        }

        void MazeInit()
        {
            HashSet<Cell> frontier = new HashSet<Cell>();
            Cell start = cells[new Vector3(Random.Range(1, columns), Random.Range(1, rows), 0)];
            Debug.Log(start.getPosition());
            frontier.Add(start);
            while (frontier.Count > 0)
            {
                //Pick a random element of the set
                Cell[] currFrontier = new Cell[frontier.Count];
                frontier.CopyTo(currFrontier);
                Cell curr = getLowestWeight(currFrontier);
                //Remove the current cell from the frontier
                frontier.Remove(curr);

                curr.beenVisited = true;
                //Set the active cell to an inner tile
                if (curr.getCellType() != cellType.outerWall)
                {
                    if (numTileNeighbors(curr) <= 1)
                    {
                        curr.setCellType(cellType.tile);
                        foreach (Vector3 nnv in curr.getNeighbors())
                        {
                            Cell next = cells[nnv];
                            if (next.getCellType() == cellType.innerWall && next.beenVisited == false)
                                frontier.Add(next);
                        }
                    }
                }
                //Add neighbors with 1 or fewer tile neighbors to the frontier
                
            }

        }

        void BoardInit()
        {
            cells.Clear();
            for (int x = 0; x <= columns; x++)
            {
                //Loop along y axis, starting from -1 to place floor or outerwall tiles.
                for (int y = 0; y <= rows; y++)
                {
                    Vector3 pos = new Vector3(x, y, 0f);
                    if (x == 0f || (int)x == columns || y == 0f || (int)y == rows)
                    {
                        cells.Add(pos, new Cell(pos, cellType.outerWall));
                    }
                    else
                    {
                        cells.Add(pos, new Cell(pos, cellType.innerWall));
                    }
                }
            }
            goalPosition = new Vector3(Random.Range((int)(columns / 2), columns), Random.Range((int)(rows / 2), rows), 0f);
            cells[goalPosition] = new Cell(goalPosition, cellType.goal);
        }

        //Sets up the outer walls and floor (background) of the game board.
        void BoardSetup()
        {
            //Instantiate Board and set boardHolder to its transform.
            boardHolder = new GameObject("Board").transform;
            //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
            for (int x = 0; x <= columns; x++)
            {
                //Loop along y axis, starting from -1 to place floor or outerwall tiles.
                for (int y = 0; y <= rows; y++)
                {        
                    //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                    GameObject toInstantiate = floorTiles[3];
                    Vector3 pos = new Vector3(x, y, 0f);
                    //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                    switch (cells[pos].getCellType()) {
                        case cellType.outerWall:
                            toInstantiate = wallTiles[0];
                            break;
                        case cellType.innerWall:
                            toInstantiate = wallTiles[1];
                            break;
                        case cellType.goal:
                            toInstantiate = exit;
                            break;
                        case cellType.tile:
                            toInstantiate = floorTiles[2];
                            break;
                        default:
                            break;
                    }
                    //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                    GameObject instance =
                        Instantiate(toInstantiate, pos, Quaternion.identity) as GameObject;

                    //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                    instance.transform.SetParent(boardHolder);
                }
            }
        }

        void spawnPlayer()
        {
            Dictionary<Vector3, Cell>.ValueCollection vals = cells.Values;
            List<Cell> tiles = new List<Cell>();
            foreach (Cell c in vals)
            {
                if (c.getCellType() == cellType.tile)
                {
                    tiles.Add(c);
                }
            }

            Vector3 pos = tiles[Random.Range(0, tiles.Count)].getPosition();

            GameObject p = Instantiate(player, pos, Quaternion.identity) as GameObject;
            GameObject camera = Instantiate(cam, new Vector3(p.transform.position.x, p.transform.position.y, -10), Quaternion.identity) as GameObject;
            camera.transform.SetParent(p.transform);

        }

        //SetupScene initializes our level and calls the previous functions to lay out the game board
        public void SetupScene()
        {
            //Creates the list of cells.
            BoardInit();

            //Creates the maze
            MazeInit();

            //Creates the outer walls and floor.
            BoardSetup();

            //Reset our list of gridpositions.
            InitialiseList();

            //Spawn the player.
            spawnPlayer();

            //Instantiate the exit tile in the upper right hand corner of our game board
            Instantiate(exit, goalPosition, Quaternion.identity);
        }
    }
}