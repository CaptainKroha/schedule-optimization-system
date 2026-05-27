using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;

namespace ScheduleOptimizationSystem.HierarchicalGameModel
{
    internal class SecondLevel(ScheduleConfig config, ILogger logger)
    {

        private readonly ILogger _logger = logger;
        private readonly StartProcessingTimesCalculator startProcessingTimesCalculator = new(config);

        public record Batch(int Type, int Size);
        public record ExtractedOrder(int Number, int Type, int ExtractionTime, int JobsCount);

        private List<Batch> schedule = [];
        private int[][][] T_0l = [];
        private List<ExtractedOrder> _outOrdersSeq = [];

        public SecondLevelSolution Solution {
            get
            {
                int[,] P_Matrix = new int[config.JobTypesCount, ScheduleSize()];
                for (int batch = 0; batch < ScheduleSize(); batch++)
                    P_Matrix[BatchType(batch), batch] = 1;

                int[,] R_Matrix = new int[config.JobTypesCount, ScheduleSize()];
                for (int batch = 0; batch < ScheduleSize(); batch++)
                    R_Matrix[BatchType(batch), batch] = BatchSize(batch);

                int[][][] T_0l_Copy = new int[config.DevicesCount][][];
                for (int device = 0; device < config.DevicesCount; device++)
                {
                    T_0l_Copy[device] = new int[ScheduleSize()][];
                    for (int batch = 0; batch < ScheduleSize(); batch++)
                    {
                        T_0l_Copy[device][batch] = new int[BatchSize(batch)];
                        Array.Copy(T_0l[device][batch], T_0l_Copy[device][batch], BatchSize(batch));
                    }                    
                }

                int[,] Y_Matrix = new int[config.OrderTypesCount, OrdersCount()];
                for (int extractedOrder = 0; extractedOrder < OrdersCount(); extractedOrder++)
                {
                    Y_Matrix[_outOrdersSeq[extractedOrder].Type, extractedOrder] = 1;
                }

                int[][] T_com_Matrix = new int[config.OrderTypesCount][];
                for (int orderType = 0; orderType < config.OrderTypesCount; orderType++)
                {
                    T_com_Matrix[orderType] = new int[config.Orders[orderType]];
                }

                foreach (var extractedOrder in _outOrdersSeq)
                {
                    T_com_Matrix[extractedOrder.Type][extractedOrder.Number] = extractedOrder.ExtractionTime;
                }

                return new SecondLevelSolution(
                    P_Matrix,
                    R_Matrix,
                    T_0l_Copy,
                    Y_Matrix,
                    T_com_Matrix
                );
            }
        }

        public bool BuildSchedule(FirstLevelSolution data)
        {
            InitScheduleComponents(data);
            List<int> jobTypes = JobTypesInPriority();

            bool solutionAcceptable = true;

            for (int batch = 0; batch < data.M.Max(); batch++)
            {
                foreach (int dataType in jobTypes)
                {
                    if (batch >= data.M[dataType])
                        continue;

                    schedule.Add(new Batch(dataType, data.A[dataType, batch]));
                    if(ScheduleSize() > 1)
                    {
                        solutionAcceptable = OptimizeLocaly(5);
                    }
                }
            }

            if (!solutionAcceptable) return false;
            return SolutionAcceptable();
        }

        private void InitScheduleComponents(FirstLevelSolution data)
        {
            schedule = new(data.M.Sum());
            _outOrdersSeq = new(OrdersCount());
        }

        protected bool OptimizeLocaly(int swapCount = 999999)
        {

            List<Batch> savedSchedule = [.. schedule];

            List<Batch> bestSchedule = [];
            int bestF2 = int.MaxValue;

            int scheduleSize = ScheduleSize();

            if (schedule[scheduleSize - 1].Type != schedule[scheduleSize - 2].Type)
            {
                bestSchedule = [.. schedule];
                T_0l = startProcessingTimesCalculator.Calculate(schedule);
                bestF2 = TotalInactionDuration();

            }

            for (int batch = ScheduleSize() - 1; batch > 0 && (swapCount > 0 || bestF2 == int.MaxValue); batch--, swapCount--)
            {

                (schedule[batch - 1], schedule[batch]) = (schedule[batch], schedule[batch - 1]);
                if(batch == 1)
                {
                    if(schedule[batch - 1].Type == schedule[batch].Type) continue;  
                }
                else if(schedule[batch - 2].Type == schedule[batch - 1].Type || schedule[batch - 1].Type == schedule[batch].Type)
                {
                    continue;
                }

                T_0l = startProcessingTimesCalculator.Calculate(schedule);
                int newValue = TotalInactionDuration();

                if (newValue < bestF2)
                {
                    bestSchedule = [.. schedule];
                    bestF2 = newValue;
                }
            }

            if (bestF2 == int.MaxValue)
            {
                schedule = savedSchedule;
                return false;
            }

            schedule = bestSchedule;
            T_0l = startProcessingTimesCalculator.Calculate(schedule);
            return true;
        }

        private bool SolutionAcceptable()
        {
            int[] jobsBufferingTimes = CalculateJobsBufferingTimes();
            DistributeOrders();

            int jobsCount = jobsBufferingTimes.Length;
            int firstCellIndex = 0, lastCellIndex = config.BufferSize - 1;
            int lastExtractedOrder = 0;

            while (lastExtractedOrder != OrdersCount())
            {
                int extractedOrdersCount = GetNumberOfExtractedOrdersInPeriod(jobsBufferingTimes[firstCellIndex], jobsBufferingTimes[lastCellIndex]);
                if (extractedOrdersCount == 0) return false;

                int orderExtractBefore = lastExtractedOrder + extractedOrdersCount;

                int releasedCells = 0;
                for (; lastExtractedOrder < orderExtractBefore; lastExtractedOrder++)
                {
                    releasedCells += _outOrdersSeq[lastExtractedOrder].JobsCount;
                }

                (firstCellIndex, lastCellIndex) = (lastCellIndex + 1, lastCellIndex + releasedCells);
                lastCellIndex = Math.Min(lastCellIndex, jobsCount - 1);
            }

            return true;
        }

        private int[] CalculateJobsBufferingTimes()
        {
            int jobsCount = 0;
            foreach (var batch in schedule)
            {
                jobsCount += batch.Size;
            }

            int[] jobsBufferingTimes = new int[jobsCount];
            int jobIndex = 0;
            int lastDevice = config.DevicesCount - 1;
            for (int batch = 0; batch < ScheduleSize(); batch++)
            {
                for (int job = 0; job < BatchSize(batch); job++)
                {
                    jobsBufferingTimes[jobIndex++] = JobCompletionTime(lastDevice, batch, job);
                }
            }
            return jobsBufferingTimes;
        }

        private void DistributeOrders()
        {
            int[][][] completionTimeOrdersComponents = new int[config.OrderTypesCount][][];
            for(int orderType = 0; orderType < config.OrderTypesCount; orderType++)
            {
                completionTimeOrdersComponents[orderType] = new int[config.Orders[orderType]][];
                for(int order = 0; order < config.Orders[orderType]; order++)
                {
                    completionTimeOrdersComponents[orderType][order] = new int[config.JobTypesCount];
                }
            }

            List<int> orderTypesByJobsCount = [.. Enumerable.Range(0, config.OrderTypesCount).OrderBy(JobsCountInOrderType)];

            for (int jobType = 0; jobType < config.JobTypesCount; jobType++)
            {
                
                Queue<int> batchesToDistribute = new(from batch in schedule where batch.Type == jobType select schedule.IndexOf(batch));

                int currentDistributedBatch = batchesToDistribute.Dequeue();
                int jobsOfBatchLeftToDistribute = BatchSize(currentDistributedBatch);
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

                            completionTimeOrdersComponents[distributedOrderType][currentFillableOrder][BatchType(currentDistributedBatch)] = JobCompletionTime(config.DevicesCount - 1, currentDistributedBatch, lastDistributedJobOfBatch);

                            currentFillableOrder++;
                            jobsOfTypeDistributedToOrder = 0;
                        }
                        else
                        {
                            jobsOfTypeDistributedToOrder += jobsOfBatchLeftToDistribute;
                            jobsOfBatchLeftToDistribute = 0;
                        }

                        if(jobsOfBatchLeftToDistribute == 0 && batchesToDistribute.Count != 0)
                        {
                            currentDistributedBatch = batchesToDistribute.Dequeue();
                            jobsOfBatchLeftToDistribute = BatchSize(currentDistributedBatch);
                            lastDistributedJobOfBatch = -1;
                        }
                    }
                }
            }

            for(int orderType = 0; orderType < config.OrderTypesCount; orderType++)
            {
                for(int order = 0; order < config.Orders[orderType]; order++)
                {
                    _outOrdersSeq.Add(new(order, orderType, completionTimeOrdersComponents[orderType][order].Max(), JobsCountInOrderType(orderType)));
                }
            }

            _outOrdersSeq = [.. _outOrdersSeq.OrderBy(i => i.ExtractionTime)];
        }

        private int GetNumberOfExtractedOrdersInPeriod(int from, int to)
        {
            int result = 0;

            foreach(var order in _outOrdersSeq)
            {
                if(order.ExtractionTime > to)
                {
                    break;
                }
                else if(order.ExtractionTime < from)
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

        private int TotalInactionDuration()
        {
            var result = 0;
            for (int device = 0; device < config.DevicesCount; device++)
            {
                result += DeviceInactionDuration(device);
            }
            return result;
        }

        private int DeviceInactionDuration(int device)
        {
            int result = T_0l[device][0][0];

            result += DeviceInactionDurationBetweenBatches(device);

            for (int batch = 0; batch < ScheduleSize(); ++batch)
            {
                result += DeviceInactionDurationBetweenJobsInBatch(device, batch);
            }

            return result;
        }

        private int DeviceInactionDurationBetweenBatches(int device)
        {
            var result = 0;

            for (int batch = 1; batch < ScheduleSize(); ++batch)
            {
                result += T_0l[device][batch][0] - CompletionTimeLastJobOfBatch(device, batch - 1);
            }

            return result;
        }

        private int DeviceInactionDurationBetweenJobsInBatch(int device, int batch)
        {
            var result = 0;

            for (int job = 1; job < BatchSize(batch); ++job)
            {
                result += T_0l[device][batch][job] - JobCompletionTime(device, batch, job - 1);
            }

            return result;
        }

        private int ScheduleSize()
        {
            return schedule.Count;
        }

        private int BatchType(int batch)
        {
            return schedule[batch].Type;
        }

        private int BatchSize(int batch)
        {
            return schedule[batch].Size;
        }

        private int CompletionTimeLastJobOfBatch(int device, int batch)
        {
            return JobCompletionTime(device, batch, BatchSize(batch) - 1);
        }

        private int JobCompletionTime(int device, int batch, int job)
        {
            return T_0l[device][batch][job] + config.WorkDurations[device, BatchType(batch)];
        }

        private int ChangeoverDuration(int device, int fromBatch, int toBatch)
        {
            return config.ChangeoverDurations[device][BatchType(fromBatch), BatchType(toBatch)];
        }

        private List<int> JobTypesInPriority()
        {
            Dictionary<int, double> m = new(config.JobTypesCount);

            for (int dataType = 0; dataType < config.JobTypesCount; dataType++)
            {
                double sum = 0;
                for (int device = 1; device < config.DevicesCount; device++)
                    sum +=
                        (double)config.WorkDurations[device, dataType] /
                        (double)config.WorkDurations[device - 1, dataType];
                m.Add(dataType, sum);
            }

            List<int> dataTypes = new(config.JobTypesCount);

            while (m.Count != 0)
            {
                int myDataType = m.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                dataTypes.Add(myDataType);
                m.Remove(myDataType);
            }

            return dataTypes;
        }
    
        private int JobsCountInOrderType(int orderType)
        {
            int result = 0;
            for(int jobType = 0; jobType < config.JobTypesCount; jobType++)
            {
                result += config.OrderTypes[orderType, jobType];
            }
            return result;
        }
    
        private int OrdersCount()
        {
            return config.Orders.Sum();
        }
    }
}
