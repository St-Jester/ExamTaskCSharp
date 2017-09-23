﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeAlgExam
{
    public enum MazeDirection
    {
        North,
        East,
        South,
        West
    }
    //public enum MazeDirection
    //{
    //    North,
    //    East,
    //    South,
    //    West
    //}

    public static class MazeDirections
    {
        static Random r = new Random();
        public const int Count = 4;

        public static MazeDirection RandomValue
        {
            get
            {
                return (MazeDirection)r.Next(0,Count);
            }
        }

        private static MazeDirection[] opposites = {
        MazeDirection.South,
        MazeDirection.West,
        MazeDirection.North,
        MazeDirection.East
    };

        public static MazeDirection GetOpposite(this MazeDirection direction)
        {
            return opposites[(int)direction];
        }

        public static MazeDirection GetNextClockwise(this MazeDirection direction)
        {
            return (MazeDirection)(((int)direction + 1) % Count);
        }

        public static MazeDirection GetNextCounterclockwise(this MazeDirection direction)
        {
            return (MazeDirection)(((int)direction + Count - 1) % Count);
        }

        private static IntVector2[] vectors = {
        new IntVector2(0, 1),
        new IntVector2(1, 0),
        new IntVector2(0, -1),
        new IntVector2(-1, 0)
    };

        public static IntVector2 ToIntVector2(this MazeDirection direction)
        {
            return vectors[(int)direction];
        }
    }
}