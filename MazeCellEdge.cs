using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeAlgExam
{
    public abstract class MazeCellEdge
    {
        public bool isPlaced = false;
        public bool isPickUp = false;
        public bool isExit = false;
        public MazeCell cell, otherCell;

        public MazeDirection direction;

        public virtual void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
        {
            this.cell = cell;
            this.otherCell = otherCell;
            this.direction = direction;
            cell.SetEdge(direction, this);
        }
    }
}
