using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem.HierarchicalGameModel
{
    internal record FirstLevelSolution(
        int[] M,
        int[,] A
    );
    internal record SecondLevelSolution(
        int[,] P,
        int[,] R,
        int[][][] T_0l,
        int[,] Y,
        int[][] T
    );

    internal record struct ScheduleBuildingResult(
        bool Success,
        int Makespan,
        FirstLevelSolution? FlSolution,
        SecondLevelSolution? SlSolution
    );
}
