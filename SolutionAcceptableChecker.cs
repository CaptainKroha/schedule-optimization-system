using System;
using System.Collections.Generic;
using System.Text;
using static ScheduleOptimizationSystem.HierarchicalGameModel.SecondLevel;

namespace ScheduleOptimizationSystem
{
    public class SolutionAcceptableChecker(ScheduleConfig config)
    {

        public bool Check(BatchesSchedule schedule, int[][][] startProcessingTimesMatrixes, List<ExtractedOrder> outOrdersSequence)
        {
            int[] jobsBufferingTimes = CalculateJobsBufferingTimes(schedule, startProcessingTimesMatrixes);

            int jobsCount = jobsBufferingTimes.Length;
            int firstCellIndex = 0, lastCellIndex = config.BufferSize - 1;
            int lastExtractedOrder = 0;

            int orderCount = config.Orders.Sum();

            while (lastExtractedOrder != orderCount)
            {
                int extractedOrdersCount = GetNumberOfExtractedOrdersInPeriod(outOrdersSequence, jobsBufferingTimes[firstCellIndex], jobsBufferingTimes[lastCellIndex]);
                if (extractedOrdersCount == 0) return false;

                int orderExtractBefore = lastExtractedOrder + extractedOrdersCount;

                int releasedCells = 0;
                for (; lastExtractedOrder < orderExtractBefore; lastExtractedOrder++)
                {
                    releasedCells += outOrdersSequence[lastExtractedOrder].JobsCount;
                }

                (firstCellIndex, lastCellIndex) = (lastCellIndex + 1, lastCellIndex + releasedCells);
                lastCellIndex = Math.Min(lastCellIndex, jobsCount - 1);
            }

            return true;
        }

        private int[] CalculateJobsBufferingTimes(BatchesSchedule schedule, int[][][] startProcessingTimesMatrixes)
        {
            int jobsCount = 0;
            for (int batch = 0; batch < schedule.Size; batch++)
            {
                jobsCount += schedule.SizeOf(batch);
            }

            int[] jobsBufferingTimes = new int[jobsCount];
            int jobIndex = 0;
            int lastDevice = config.DevicesCount - 1;

            for (int batch = 0; batch < schedule.Size; batch++)
            {
                for (int job = 0; job < schedule.SizeOf(batch); job++)
                {
                    jobsBufferingTimes[jobIndex++] = startProcessingTimesMatrixes[lastDevice][batch][job] 
                        + config.WorkDurations[lastDevice, schedule.TypeOf(batch)];
                }
            }
            return jobsBufferingTimes;
        }

        private int GetNumberOfExtractedOrdersInPeriod(List<ExtractedOrder> outOrdersSequence, int from, int to)
        {
            int result = 0;

            foreach (var order in outOrdersSequence)
            {
                if (order.ExtractionTime > to)
                {
                    break;
                }
                else if (order.ExtractionTime < from)
                {
                    continue;
                }
                else
                {
                    result++;
                }
            }

            return result;
        }

    }
}
