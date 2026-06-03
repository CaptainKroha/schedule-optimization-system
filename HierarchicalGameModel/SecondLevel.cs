using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;

namespace ScheduleOptimizationSystem.HierarchicalGameModel
{
    public class SecondLevel(ScheduleConfig config, ILogger logger)
    {

        private readonly ILogger _logger = logger;

        private readonly StartProcessingTimesCalculator startProcessingTimesCalculator = new(config);
        private readonly JobsToOrdersDistributor jobsToOrdersDistributor = new(config);
        private readonly SolutionAcceptableChecker solutionAcceptableChecker = new(config);

        public record struct ExtractedOrder(int Number, int Type, int ExtractionTime, int JobsCount);

        private BatchesSchedule schedule;
        private int[][][] T_0l = [];
        private List<ExtractedOrder> _outOrdersSeq = [];

        public SecondLevelSolution Solution {
            get
            {
                return new SecondLevelSolution(config, schedule, T_0l, _outOrdersSeq);
            }
        }

        public bool BuildSchedule(FirstLevelSolution data)
        {
            schedule = new(data.M.Sum());
            List<int> jobTypes = JobTypesInPriority();

            bool solutionAcceptable = true;

            for (int batch = 0; batch < data.M.Max(); batch++)
            {
                foreach (int dataType in jobTypes)
                {
                    if (batch >= data.M[dataType])
                        continue;

                    schedule.Add(dataType, data.A[dataType, batch]);
                    if(schedule.Size > 1)
                    {
                        solutionAcceptable = OptimizeLocaly(5);
                    }
                }
            }

            if (!solutionAcceptable) return false;

            _outOrdersSeq = jobsToOrdersDistributor.Distribute(schedule, T_0l);
            return solutionAcceptableChecker.Check(schedule, T_0l, _outOrdersSeq);
        }

        protected bool OptimizeLocaly(int swapCount = 999999)
        {

            BatchesSchedule savedSchedule = new(schedule);

            BatchesSchedule? bestSchedule = null;
            int bestF2 = int.MaxValue;

            if (schedule.TypeOf(schedule.Size - 1) != schedule.TypeOf(schedule.Size - 2))
            {
                bestSchedule = new(schedule);
                T_0l = startProcessingTimesCalculator.Calculate(schedule);
                bestF2 = TotalInactionDuration();

            }

            for (int batch = schedule.Size - 1; batch > 0 && (swapCount > 0 || bestF2 == int.MaxValue); batch--, swapCount--)
            {
                schedule.Switch(batch - 1, batch);
                if(batch == 1)
                {
                    if(schedule.TypeOf(batch - 1) == schedule.TypeOf(batch)) continue;  
                }
                else if(schedule.TypeOf(batch - 2) == schedule.TypeOf(batch - 1) || schedule.TypeOf(batch) == schedule.TypeOf(batch - 1))
                {
                    continue;
                }

                T_0l = startProcessingTimesCalculator.Calculate(schedule);
                int newValue = TotalInactionDuration();

                if (newValue < bestF2)
                {
                    bestSchedule = new(schedule);
                    bestF2 = newValue;
                }
            }

            if (bestF2 == int.MaxValue)
            {
                schedule = savedSchedule;
                return false;
            }

            schedule = new(bestSchedule);
            T_0l = startProcessingTimesCalculator.Calculate(schedule);
            return true;
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

            for (int batch = 0; batch < schedule.Size; ++batch)
            {
                result += DeviceInactionDurationBetweenJobsInBatch(device, batch);
            }

            return result;
        }

        private int DeviceInactionDurationBetweenBatches(int device)
        {
            var result = 0;

            for (int batch = 1; batch < schedule.Size; ++batch)
            {
                result += T_0l[device][batch][0] - CompletionTimeLastJobOfBatch(device, batch - 1);
            }

            return result;
        }

        private int DeviceInactionDurationBetweenJobsInBatch(int device, int batch)
        {
            var result = 0;

            for (int job = 1; job < schedule.SizeOf(batch); ++job)
            {
                result += T_0l[device][batch][job] - JobCompletionTime(device, batch, job - 1);
            }

            return result;
        }

        private int CompletionTimeLastJobOfBatch(int device, int batch)
        {
            return JobCompletionTime(device, batch, schedule.SizeOf(batch) - 1);
        }

        private int JobCompletionTime(int device, int batch, int job)
        {
            return T_0l[device][batch][job] + config.WorkDurations[device, schedule.TypeOf(batch)];
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
        
    }
}
