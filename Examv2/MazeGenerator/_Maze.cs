using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator
{
    public struct Frog
    {
       public IntVector2 frogCoordinates;
       public MazeDirection frogEdgeDirection;
    }
    public class _Maze
    {
        Frog mazeFrog = new Frog();
        string passageS = " ", wallS = "#", pickUps = "$", exit = "N", frog = "@";
            
        Random r = new Random();

        public IntVector2 size;

        private MazeCell[,] cells;
        private float Pprobability = 0.1f;
        public bool Frogs;
        public _Maze()
        {
            size = new IntVector2(10, 10);
            Frogs = true;
        }

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

            if (Frogs)
                GenerateFrogs();

            showMaze();
            startMove();
        }

        public void startMove()
        {
            MazeCell frogCell, nextFrogCell;
            MazeDirection dir;
            int cnt = 0;
            do
            {
                ++cnt;
                frogCell = GetCell(mazeFrog.frogCoordinates);
                dir = GetRandomPassage(frogCell);
                if (frogCell.GetEdge(dir).isExit)
                {
                    break;
                }
                
                nextFrogCell = GetCell(mazeFrog.frogCoordinates + dir.ToIntVector2());

                frogCell.GetEdge(mazeFrog.frogEdgeDirection).isFrog = false;
                nextFrogCell.GetEdge(mazeFrog.frogEdgeDirection.GetOpposite()).isFrog = true;

                mazeFrog.frogCoordinates = nextFrogCell.coordinates;
                mazeFrog.frogEdgeDirection = mazeFrog.frogEdgeDirection.GetOpposite();

                FrogPainter(frogCell.coordinates, nextFrogCell.coordinates);

            } while (cnt<=100);
            
        }

        private void FrogPainter(IntVector2 previousCell, IntVector2 nextCell)
        {
            System.Threading.Thread.Sleep(100);

            IntVector2 prevRealPos = previousCell + mazeFrog.frogEdgeDirection.GetOpposite().ToIntVector2();
            IntVector2 nextRealPos = nextCell + mazeFrog.frogEdgeDirection.ToIntVector2();
            ConsoleOperations.WriteAt(pickUps, 2 * prevRealPos.x + 1, 2 * size.z - 2*prevRealPos.z + 1);
            ConsoleOperations.WriteAt(frog,2*nextRealPos.x+1,2 * size.z - 2*nextRealPos.z + 1);
        }

        private MazeDirection GetRandomPassage(MazeCell cell)
        {
                Random r = new Random();
                
                for (int i = 0; i < MazeDirections.Count; i++)
                {
                if (cell.GetEdge((MazeDirection)i) is MazePassage)
                    {
                        return (MazeDirection)i;
                    }
                
                }
            throw new Exception("No Passage");
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
        #region GeneratorSteps
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
        #endregion


       
        private MazeCell CreateCell(IntVector2 coordinates)
        {
            MazeCell newCell = new MazeCell();
            cells[coordinates.x, coordinates.z] = newCell;
            newCell.coordinates = coordinates;

            return newCell;

        }
 #region Generate InMaze Items
        private void GenerateFrogs()
        {
            MazeCell c = cells[RandomCoordinates.x, RandomCoordinates.z];

            MazeDirection xdir = GetRandomPassage(c);

            c.GetEdge(xdir).isFrog = true;
            //
            IntVector2 nextcellCoord = c.coordinates + xdir.ToIntVector2();
            GetCell(nextcellCoord).GetEdge(xdir.GetOpposite()).isFrog = true;

            mazeFrog.frogCoordinates = c.coordinates;
            mazeFrog.frogEdgeDirection = xdir;
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
        #endregion
        

        private void SetDiagonalWalls(int i, int j)
        {
            ConsoleOperations.WriteAt(wallS, i-1, j-1);
            ConsoleOperations.WriteAt(wallS, i - 1, j + 1);
            ConsoleOperations.WriteAt(wallS, i + 1, j - 1);
            ConsoleOperations.WriteAt(wallS, i + 1, j + 1);
        }
        public void showMaze()
        {
           // ConsoleOperations.WriteAt(wallS, 0, 0);
            ConsoleOperations.WriteAt(wallS, 0, 2 * size.z + 1);
            //ConsoleOperations.WriteAt(wallS, 2 * size.x , 0);
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
                                    else if (cells[i, j].GetEdge((MazeDirection)z).isExit)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        ConsoleOperations.WriteAt(exit, c_i, c_j - 1);
                                        Console.ForegroundColor = ConsoleColor.Gray;

                                    }
                                    else
                                        if (cells[i, j].GetEdge((MazeDirection)z).isFrog)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        ConsoleOperations.WriteAt(frog, c_i, c_j - 1);
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
                                        if (cells[i, j].GetEdge((MazeDirection)z).isFrog)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        ConsoleOperations.WriteAt(frog, c_i + 1, c_j);
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
                                        if (cells[i, j].GetEdge((MazeDirection)z).isFrog)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        ConsoleOperations.WriteAt(frog, c_i, c_j + 1);
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
                                        if (cells[i, j].GetEdge((MazeDirection)z).isFrog)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        ConsoleOperations.WriteAt(frog, c_i - 1, c_j);
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