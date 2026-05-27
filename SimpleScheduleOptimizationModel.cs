using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem
{
    internal class SimpleScheduleOptimizationModel : IScheduleOptimizationModel
    {
        private record Batch(int Type, int Size);

        private ScheduleDto? _scheduleDto;

        private List<Batch> schedule = [];
        private int[][][] T_0l = [];
        private ScheduleConfig _config;

        async Task<bool> IScheduleOptimizationModel.BuildScheduleAsync(ScheduleConfig config)
        {
            _config = config;
            bool success = await Task.Run(BuildSchedule);
            return success;
        }

        ScheduleDto IScheduleOptimizationModel.GetSchedule()
        {
            if (_scheduleDto is null) throw new Exception("Расписание еще не построено");
            return _scheduleDto;
        }

        private bool BuildSchedule()
        {
            int[] ordersLeft = [.. _config.Orders];
            
            int[] jobsCountInOrderTypes = new int[_config.OrderTypesCount];
            for(int orderType = 0;  orderType < _config.OrderTypesCount; orderType++)
            {
                jobsCountInOrderTypes[orderType] = JobsCountInOrderType(orderType);
            }

            int[] orderTypesByJobsCount = [.. Enumerable.Range(0, _config.OrderTypesCount).OrderByDescending(i => jobsCountInOrderTypes[i])];

            while(ordersLeft.Sum() > 0)
            {
                int spaceLeft = _config.BufferSize;
                int[] ordersToMerge = new int[_config.OrderTypesCount];
                foreach(var orderType in orderTypesByJobsCount)
                {
                    if (ordersLeft[orderType] == 0) continue;
                    
                    while (jobsCountInOrderTypes[orderType] <= spaceLeft && ordersLeft[orderType] > 0)
                    {
                        ordersToMerge[orderType]++;
                        ordersLeft[orderType]--;
                        spaceLeft -= jobsCountInOrderTypes[orderType];
                    }
                }
                for(int jobType = 0; jobType < _config.JobTypesCount; jobType++)
                {
                    int jobsOfCurrentBatchCount = 0;
                    for(int orderType = 0; orderType < _config.OrderTypesCount; orderType++)
                    {
                        jobsOfCurrentBatchCount += ordersToMerge[orderType] * _config.OrderTypes[orderType, jobType];
                    }
                    schedule.Add(new(jobType, jobsOfCurrentBatchCount));
                }
            }

            UpdateT_0l();
            BuildScheduleDto();

            return true;
        }

        private void BuildScheduleDto()
        {
            _scheduleDto = [];
            _scheduleDto.MakeSpan = CompletionTimeLastJobOfBatch(_config.DevicesCount - 1, ScheduleSize() - 1);

            for (int device = 0; device < _config.DevicesCount; device++)
            {
                _scheduleDto.Add([]);
                for (int batch = 0; batch < ScheduleSize(); batch++)
                {
                    for (int job = 0; job < BatchSize(batch); job++)
                    {
                        int jobType = BatchType(batch);
                        _scheduleDto[device].Add(new ScheduleElementDto($"T{jobType + 1}", T_0l[device][batch][job], _config.WorkDurations[device, jobType]));
                    }
                }
            }
        }

        private void BuildTestSchedule()
        {
            _scheduleDto = [];
            _scheduleDto.MakeSpan = 33;
            _scheduleDto.Add([]);
            _scheduleDto[0].Add(new("T1", 0, 2));
            _scheduleDto[0].Add(new("T1", 2, 2));
            _scheduleDto[0].Add(new("T1", 4, 2));
            _scheduleDto[0].Add(new("T2", 9, 2));
            _scheduleDto[0].Add(new("T2", 11, 2));
            _scheduleDto[0].Add(new("T3", 18, 1));
            _scheduleDto[0].Add(new("T3", 19, 1));
            _scheduleDto[0].Add(new("T3", 20, 1));
            _scheduleDto.Add([]);
            _scheduleDto[1].Add(new("T1", 6, 1));
            _scheduleDto[1].Add(new("T1", 7, 1));
            _scheduleDto[1].Add(new("T1", 8, 1));
            _scheduleDto[1].Add(new("T2", 13, 3));
            _scheduleDto[1].Add(new("T2", 16, 3));
            _scheduleDto[1].Add(new("T3", 20, 2));
            _scheduleDto[1].Add(new("T3", 22, 2));
            _scheduleDto[1].Add(new("T3", 24, 2));
            _scheduleDto.Add([]);
            _scheduleDto[2].Add(new("T1", 9, 2));
            _scheduleDto[2].Add(new("T1", 11, 2));
            _scheduleDto[2].Add(new("T1", 13, 2));
            _scheduleDto[2].Add(new("T2", 16, 2));
            _scheduleDto[2].Add(new("T2", 19, 2));
            _scheduleDto[2].Add(new("T3", 24, 3));
            _scheduleDto[2].Add(new("T3", 27, 3));
            _scheduleDto[2].Add(new("T3", 30, 3));
        }

        private int JobsCountInOrderType(int orderType)
        {
            int result = 0;
            for (int jobType = 0; jobType < _config.JobTypesCount; jobType++)
            {
                result += _config.OrderTypes[orderType, jobType];
            }
            return result;
        }

        private void UpdateT_0l()
        {
            T_0l = new int[_config.DevicesCount][][];
            for (int device = 0; device < _config.DevicesCount; device++)
            {
                T_0l[device] = new int[ScheduleSize()][];
                for (int batch = 0; batch < ScheduleSize(); batch++)
                {
                    T_0l[device][batch] = new int[BatchSize(batch)];
                }
            }

            CalcT_01();
            for (int device = 1; device < _config.DevicesCount; device++)
            {
                CalcT_0N(device);
            }
        }

        private void CalcT_01()
        {
            int device = 0, batch = 0, job = 0;

            // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на 1 приборе, как наладку
            T_0l[device][batch][job] = ChangeoverDuration(device, batch, batch);

            for (job = 1; job < BatchSize(batch); job++)
            {
                T_0l[device][batch][job] = JobCompletionTime(device, batch, job - 1);
            }


            // Пробегаемся по всем возможным позициям cо второго пакета
            for (batch = 1; batch < ScheduleSize(); batch++)
            {
                job = 0;

                // Момент начала времени выполнения 1 задания в пакете на позиции batch
                T_0l[device][batch][job] = CompletionTimeLastJobOfBatch(device, batch - 1) + ChangeoverDuration(device, batch - 1, batch);

                for (job = 1; job < BatchSize(batch); job++)
                {
                    T_0l[device][batch][job] = JobCompletionTime(device, batch, job - 1);
                }
            }
        }

        private void CalcT_0N(int device)
        {
            int batch = 0, job = 0;

            // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на приборе device, как
            // Максимум, между временем наладки прибора на выполнение 1 задания в 1 пакете
            // и временем окончания выполнения 1 задания в 1 пакете на предыдущем приборе
            T_0l[device][batch][job] = Math.Max(

                // Время наладки прибора на выполнение 1 задания в 1 пакете
                ChangeoverDuration(device, batch, batch),

                // Время окончания выполнения 1 задания в 1 пакете на предыдущем приборе
                JobCompletionTime(device - 1, batch, job)
            );

            // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
            for (job = 1; job < BatchSize(batch); job++)

                // Устанавливаем момент начала времени выполнения текущего задания job, как
                // Максимум, между временем окончания предыдущего задания на текущем приборе и
                // временем окончания текущего задания на предыдущем приборе
                T_0l[device][batch][job] = Math.Max(
                    JobCompletionTime(device, batch, job - 1),
                    JobCompletionTime(device - 1, batch, job)
                );

            // Пробегаемся по всем возможным позициям пакетов
            for (batch = 1; batch < ScheduleSize(); batch++)
            {

                // Инициализируем индекс задания
                job = 0;

                // Устанавливаем момент начала времени выполнения 1 задания в пакете batchIndex на приборе device,
                // как Максимум, между временем окончания выполнения последнего задания в предыдущем пакете вместе с переналадкой 
                // и временем окончания выполнения 1 задания в пакете на в batchIndex на предыдущем приборе
                T_0l[device][batch][job] = Math.Max(

                    CompletionTimeLastJobOfBatch(device, batch - 1) + ChangeoverDuration(device, batch - 1, batch),
                    JobCompletionTime(device - 1, batch, job));

                // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
                for (job = 1; job < BatchSize(batch); job++)

                    // Устанавливаем момент начала времени выполнения текущего задания job, как
                    // Максимум, между временем окончания предыдущего задания на текущем приборе и
                    // временем окончания текущего задания на предыдущем приборе
                    T_0l[device][batch][job] = Math.Max(
                        JobCompletionTime(device, batch, job - 1),
                        JobCompletionTime(device - 1, batch, job)
                    );
            }
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
            return T_0l[device][batch][job] + _config.WorkDurations[device, BatchType(batch)];
        }

        private int ChangeoverDuration(int device, int fromBatch, int toBatch)
        {
            return _config.ChangeoverDurations[device][BatchType(fromBatch), BatchType(toBatch)];
        }


    }
}
