using UnityEngine;
using System.Collections.Generic;

namespace MazeGame {
    public class Cell {

        private cellType cellType;
        private Vector3 position;
        private int weight;
	    public Cell(Vector3 p, cellType c)
	    {
            position = p;
            cellType = c;
            weight = Random.Range(0, BoardManager.rows * BoardManager.columns);
	    }

        public cellType getCellType()
        {
            return cellType;
        }

        public void setCellType(cellType c)
        {
            cellType = c;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public void setPosition(Vector3 v)
        {
            position = v;
        }
        public int getWeight()
        {
            return weight;
        }

        public void setWeight(int w)
        {
            weight = w;
        }

        public List<Vector3> getNeighbors()
        {
            List<Vector3> neighbors = new List<Vector3>();

            if (position.x < BoardManager.columns)
                neighbors.Add(new Vector3(position.x + 1, position.y, 0));
            if (position.y < BoardManager.rows)
                neighbors.Add(new Vector3(position.x, position.y + 1, 0));
            if (position.x > 0)
                neighbors.Add(new Vector3(position.x - 1, position.y, 0));
            if (position.y > 0)
                neighbors.Add(new Vector3(position.x, position.y - 1, 0));

            return neighbors;
        }
    }
}
