using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem
{
    public interface ILogger
    {
        void Print(string message);
        void Print(string message, int[] array);
        void Print(string message, List<List<int>> matrix);
        void Print(string message, int[,] matrix);
    }
}
