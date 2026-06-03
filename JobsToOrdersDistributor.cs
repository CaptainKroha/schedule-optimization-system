using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static ScheduleOptimizationSystem.HierarchicalGameModel.SecondLevel;

namespace ScheduleOptimizationSystem
{
    public class JobsToOrdersDistributor(ScheduleConfig config)
    {
        public List<ExtractedOrder> Distribute(BatchesSchedule schedule, int[][][] startProcessingTimesMatrixes)
        {

            int[][][] completionTimeOrdersComponents = new int[config.OrderTypesCount][][];
            for (int orderType = 0; orderType < config.OrderTypesCount; orderType++)
            {
                completionTimeOrdersComponents[orderType] = new int[config.Orders[orderType]][];
                for (int order = 0; order < config.Orders[orderType]; order++)
                {
                    completionTimeOrdersComponents[orderType][order] = new int[config.JobTypesCount];
                }
            }

            List<int> orderTypesByJobsCount = [.. Enumerable.Range(0, config.OrderTypesCount).OrderBy(JobsCountInOrderType)];

            for (int jobType = 0; jobType < config.JobTypesCount; jobType++)
            {

                Queue<int> batchesToDistribute = new();
                for (int batch = 0; batch < schedule.Size; batch++)
                {
                    if (schedule.TypeOf(batch) == jobType) batchesToDistribute.Enqueue(batch);
                }

                int currentDistributedBatch = batchesToDistribute.Dequeue();
                int jobsOfBatchLeftToDistribute = schedule.SizeOf(currentDistributedBatch);
                int lastDistributedJobOfBatch = -1;

                Queue<int> orderTypesToDistribute = new(orderTypesByJobsCount);

                while (orderTypesToDistribute.Count != 0)
                {
                    int distributedOrderType = orderTypesToDistribute.Dequeue();
                    int jobsOfTypeDistributedToOrder = 0;
                    int currentFillableOrder = 0;

                    while (currentFillableOrder < config.Orders[distributedOrderType])
                    {
                        int jobsOfTypeLeftToFillOrderComponent = config.OrderTypes[distributedOrderType, jobType] - jobsOfTypeDistributedToOrder;

                        if (jobsOfBatchLeftToDistribute >= jobsOfTypeLeftToFillOrderComponent)
                        {
                            jobsOfBatchLeftToDistribute -= jobsOfTypeLeftToFillOrderComponent;
                            lastDistributedJobOfBatch += jobsOfTypeLeftToFillOrderComponent;

                            completionTimeOrdersComponents[distributedOrderType][currentFillableOrder][schedule.TypeOf(currentDistributedBatch)] =
                                startProcessingTimesMatrixes[config.DevicesCount - 1][currentDistributedBatch][lastDistributedJobOfBatch] + config.WorkDurations[config.DevicesCount - 1, schedule.TypeOf(currentDistributedBatch)];

                            currentFillableOrder++;
                            jobsOfTypeDistributedToOrder = 0;
                        }
                        else
                        {
                            jobsOfTypeDistributedToOrder += jobsOfBatchLeftToDistribute;
                            jobsOfBatchLeftToDistribute = 0;
                        }

                        if (jobsOfBatchLeftToDistribute == 0 && batchesToDistribute.Count != 0)
                        {
                            currentDistributedBatch = batchesToDistribute.Dequeue();
                            jobsOfBatchLeftToDistribute = schedule.SizeOf(currentDistributedBatch);
                            lastDistributedJobOfBatch = -1;
                        }
                    }
                }
            }

            List<ExtractedOrder> outOrdersSequence = [];

            for (int orderType = 0; orderType < config.OrderTypesCount; orderType++)
            {
                for (int order = 0; order < config.Orders[orderType]; order++)
                {
                    outOrdersSequence.Add(new(order, orderType, completionTimeOrdersComponents[orderType][order].Max(), JobsCountInOrderType(orderType)));
                }
            }

            outOrdersSequence = [.. outOrdersSequence.OrderBy(i => i.ExtractionTime)];
            return outOrdersSequence;
        }

        private int JobsCountInOrderType(int orderType)
        {
            int result = 0;
            for (int jobType = 0; jobType < config.JobTypesCount; jobType++)
            {
                result += config.OrderTypes[orderType, jobType];
            }
            return result;
        }
    }
}
