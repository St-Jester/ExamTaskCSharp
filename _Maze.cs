using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeAlgExam
{
    public class _Maze
    {
        string passageS = " ", wallS = "#", pickUps = "$", exit = "N";
            
        Random r = new Random();

        public IntVector2 size = new IntVector2(10, 10);

        private MazeCell[,] cells;
        private float Pprobability = 0.01f;
        public IntVector2 RandomCoordinates
        {
            get
            {
                return new IntVector2(r.Next(0, size.x), r.Next(0, size.z));
            }
        }

        public bool ContainsCoordinates(IntVector2 coordinate)
        {
            return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
        }
        public MazeCell GetCell(IntVector2 coordinates)
        {
            return cells[coordinates.x, coordinates.z];
        }
        public void Generate()
        {
            cells = new MazeCell[size.x, size.z];
            List<MazeCell> activeCells = new List<MazeCell>();
            DoFirstGenerationStep(activeCells);
            while (activeCells.Count > 0)
            {

                DoNextGenerationStep(activeCells);
            }
            GenerateExit();
            showMaze();
        }
        private void GenerateExit()
        {
            do
            {
                MazeCell c = GetCell(RandomCoordinates);
                MazeCellEdge edge = c.GetEdge((MazeDirection)r.Next(0, 3));
                if (edge is MazePassage)
                {
                    edge.isExit = true;
                    
                    break;
                }
                else
                    continue;
            }
            while (true);
            
            
        }
        private void DoFirstGenerationStep(List<MazeCell> activeCells)
        {
            MazeCell newCell = CreateCell(RandomCoordinates);

            activeCells.Add(newCell);
        }
        private void DoNextGenerationStep(List<MazeCell> activeCells)
        {
            int currentIndex = activeCells.Count - 1;
            MazeCell currentCell = activeCells[currentIndex];
            if (currentCell.IsFullyInitialized)
            {
                activeCells.RemoveAt(currentIndex);
                return;
            }
            MazeDirection direction = currentCell.RandomUninitializedDirection;
            IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
            if (ContainsCoordinates(coordinates))
            {
                MazeCell neighbor = GetCell(coordinates);
                if (neighbor == null)
                {
                    neighbor = CreateCell(coordinates);
                    CreatePassage(currentCell, neighbor, direction);
                    
                    activeCells.Add(neighbor);
                }
                else
                {
                    CreateWall(currentCell, neighbor, direction);
                }
            }
            else
            {
                CreateWall(currentCell, null, direction);
            }
        }

        private MazeCell CreateCell(IntVector2 coordinates)
        {
            MazeCell newCell = new MazeCell();
            cells[coordinates.x, coordinates.z] = newCell;
            newCell.coordinates = coordinates;

            return newCell;

        }
        private void CreatePickUp(MazeCell cell, MazeCell otherCell, MazeDirection direction)
        {
            cell.GetEdge(direction).isPickUp = true;
            otherCell.GetEdge(direction.GetOpposite()).isPickUp = true;
        }
        private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
        {
            MazePassage passage = new MazePassage();
            //Instantiate ' ' on a console screen
            passage.Initialize(cell, otherCell, direction);
            passage.Initialize(otherCell, cell, direction.GetOpposite());
            if (r.NextDouble() < Pprobability)
            {
                CreatePickUp(cell, otherCell, direction);

            }

        }

        private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
        {
            MazeWall wall = new MazeWall();
            ////Instantiate '@' on a console screen
            wall.Initialize(cell, otherCell, direction);

            if (otherCell != null)
            {
                wall.Initialize(otherCell, cell, direction.GetOpposite());
            }

        }
        //private void CreateOuterWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
        //{
        //    MazeWall wall = new MazeWall();

        //    wall.Initialize(cell, otherCell, direction);
        //    if (otherCell != null)
        //    {
        //        wall.Initialize(otherCell, cell, direction.GetOpposite());
        //    }
        //    ConsoleOperations.WriteAt("@", cell.coordinates.x + 1, cell.coordinates.z + 1);
        //}
        private void SetDiagonalWalls(int i, int j)
        {
            ConsoleOperations.WriteAt(wallS, i-1, j-1);
            ConsoleOperations.WriteAt(wallS, i - 1, j + 1);
            ConsoleOperations.WriteAt(wallS, i + 1, j - 1);
            ConsoleOperations.WriteAt(wallS, i + 1, j + 1);


        }
        public void showMaze()
        {
            ConsoleOperations.WriteAt(wallS, 0, 0);
            ConsoleOperations.WriteAt(wallS, 0, 2 * size.z + 1);
            ConsoleOperations.WriteAt(wallS, 2 * size.x , 0);
            ConsoleOperations.WriteAt(wallS, 2 * size.x , 2 * size.z );

            for (int i = 0, c_i = 1; i <size.x; i++, c_i++)
            {
                for (int j = 0, c_j = 2*size.z; j < size.z; j++,c_j--)
                {
                    ConsoleOperations.WriteAt(passageS, c_i, c_j);
                    SetDiagonalWalls(c_i, c_j);
                    for (int z = 0; z < MazeDirections.Count; z++)
                    {
                        #region Switch
                        switch (z)
                        {
                            case 0:
                                if (cells[i, j].GetEdge((MazeDirection)z) is MazePassage
                                    && !cells[i, j].GetEdge((MazeDirection)z).isPlaced)
                                {
                                    if (cells[i, j].GetEdge((MazeDirection)z).isPickUp)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        ConsoleOperations.WriteAt(pickUps, c_i, c_j - 1);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else if(cells[i, j].GetEdge((MazeDirection)z).isExit)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        ConsoleOperations.WriteAt(exit, c_i, c_j - 1);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else
                                    ConsoleOperations.WriteAt(passageS, c_i, c_j - 1);
                                    
                                }
                                else
                                if (!cells[i, j].GetEdge((MazeDirection)z).isPlaced)
                                {

                                    ConsoleOperations.WriteAt(wallS, c_i, c_j - 1);
                                }
                                cells[i, j].GetEdge((MazeDirection)z).isPlaced = true;
                                cells[i, j].GetEdge(((MazeDirection)z).GetOpposite()).isPlaced = true;
                                break;
                            case 1:
                                if (cells[i, j].GetEdge((MazeDirection)z) is MazePassage 
                                    && !cells[i, j].GetEdge((MazeDirection)z).isPlaced)
                                {
                                    if (cells[i, j].GetEdge((MazeDirection)z).isPickUp)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;

                                        ConsoleOperations.WriteAt(pickUps, c_i + 1, c_j);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else if (cells[i, j].GetEdge((MazeDirection)z).isExit)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        ConsoleOperations.WriteAt(exit, c_i + 1, c_j);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else
                                    ConsoleOperations.WriteAt(passageS, c_i + 1, c_j);
                                    
                                }
                                else
                                if (!cells[i, j].GetEdge((MazeDirection)z).isPlaced)
                                {
                                    ConsoleOperations.WriteAt(wallS, c_i + 1, c_j);
                                }
                                cells[i, j].GetEdge((MazeDirection)z).isPlaced = true;
                                cells[i, j].GetEdge(((MazeDirection)z).GetOpposite()).isPlaced = true;

                                break;
                            case 2:
                                if (cells[i, j].GetEdge((MazeDirection)z) is MazePassage
                                    && !cells[i, j].GetEdge((MazeDirection)z).isPlaced)
                                {
                                    if (cells[i, j].GetEdge((MazeDirection)z).isPickUp)
                                    {

                                        Console.ForegroundColor = ConsoleColor.Yellow;

                                        ConsoleOperations.WriteAt(pickUps, c_i, c_j + 1);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else if (cells[i, j].GetEdge((MazeDirection)z).isExit)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        ConsoleOperations.WriteAt(exit, c_i, c_j + 1);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else
                                        ConsoleOperations.WriteAt(passageS, c_i, c_j + 1);
                                }
                                else
                                if(!cells[i, j].GetEdge((MazeDirection)z).isPlaced)
                                {
                                    ConsoleOperations.WriteAt(wallS, c_i, c_j + 1);
                                }
                                cells[i, j].GetEdge((MazeDirection)z).isPlaced = true;
                                cells[i, j].GetEdge(((MazeDirection)z).GetOpposite()).isPlaced = true;

                                break;
                            case 3:
                                if (cells[i, j].GetEdge((MazeDirection)z) is MazePassage
                                    && !cells[i, j].GetEdge((MazeDirection)z).isPlaced)
                                {
                                    if (cells[i, j].GetEdge((MazeDirection)z).isPickUp)
                                    {

                                        Console.ForegroundColor = ConsoleColor.Yellow;

                                        ConsoleOperations.WriteAt(pickUps, c_i - 1, c_j);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else if (cells[i, j].GetEdge((MazeDirection)z).isExit)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        ConsoleOperations.WriteAt(exit, c_i - 1, c_j);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else
                                        ConsoleOperations.WriteAt(passageS, c_i - 1, c_j);
                                }
                                else
                                if (!cells[i, j].GetEdge((MazeDirection)z).isPlaced)
                                {
                                    ConsoleOperations.WriteAt(wallS, c_i - 1, c_j);
                                }
                                cells[i, j].GetEdge((MazeDirection)z).isPlaced = true;
                                cells[i, j].GetEdge(((MazeDirection)z).GetOpposite()).isPlaced = true;

                                break;

                        }
                        #endregion
                    }
                    c_j--;
                }
                c_i++;
            }

        }
    }
}
//ставить через один.step = 2
//все через 2
