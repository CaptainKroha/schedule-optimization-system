using System;
using System.Collections.Generic;
using System.Text;
using static ScheduleOptimizationSystem.HierarchicalGameModel.SecondLevel;

namespace ScheduleOptimizationSystem.HierarchicalGameModel
{
    public record FirstLevelSolution
    {
        public int[] M;
        public int[,] A;

        public FirstLevelSolution(ScheduleConfig config, List<List<int>> A_Matrix)
        {
            M = new int[config.JobTypesCount];
            int maxBatchesCount = 0;
            for (int jobType = 0; jobType < config.JobTypesCount; jobType++)
            {
                int jobTypeBatchesCount = A_Matrix[jobType].Count;
                maxBatchesCount = Math.Max(maxBatchesCount, jobTypeBatchesCount);
                M[jobType] = jobTypeBatchesCount;
            }

            A = new int[config.JobTypesCount, maxBatchesCount];
            for (int jobType = 0; jobType < config.JobTypesCount; jobType++)
            {
                for (int batch = 0; batch < M[jobType]; batch++)
                {
                    A[jobType, batch] = A_Matrix[jobType][batch];
                }
            }

        }
    
    };
    public record SecondLevelSolution
    {

        public int[,] P;
        public int[,] R;
        public int[][][] T_0l;
        public int[,] Y;
        public int[][] T;


        public SecondLevelSolution(ScheduleConfig config, BatchesSchedule schedule, int[][][] startProcessingTimesMatrixes, List<ExtractedOrder> outOrdersSequence)
        {
            P = new int[config.JobTypesCount, schedule.Size];
            for (int batch = 0; batch < schedule.Size; batch++)
                P[schedule.TypeOf(batch), batch] = 1;

            R = new int[config.JobTypesCount, schedule.Size];
            for (int batch = 0; batch < schedule.Size; batch++)
                R[schedule.TypeOf(batch), batch] = schedule.SizeOf(batch);

            T_0l = new int[config.DevicesCount][][];
            for (int device = 0; device < config.DevicesCount; device++)
            {
                T_0l[device] = new int[schedule.Size][];
                for (int batch = 0; batch < schedule.Size; batch++)
                {
                    T_0l[device][batch] = new int[schedule.SizeOf(batch)];
                    Array.Copy(startProcessingTimesMatrixes[device][batch], T_0l[device][batch], schedule.SizeOf(batch));
                }
            }

            Y = new int[config.OrderTypesCount, config.Orders.Sum()];
            for (int extractedOrder = 0; extractedOrder < config.Orders.Sum(); extractedOrder++)
            {
                Y[outOrdersSequence[extractedOrder].Type, extractedOrder] = 1;
            }

            T = new int[config.OrderTypesCount][];
            for (int orderType = 0; orderType < config.OrderTypesCount; orderType++)
            {
                T[orderType] = new int[config.Orders[orderType]];
            }

            foreach (var extractedOrder in outOrdersSequence)
            {
                T[extractedOrder.Type][extractedOrder.Number] = extractedOrder.ExtractionTime;
            }
        }
    };

    internal record struct ScheduleBuildingResult(
        bool Success,
        int Makespan,
        FirstLevelSolution? FlSolution,
        SecondLevelSolution? SlSolution
    );
}
