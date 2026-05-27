using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem.HierarchicalGameModel
{
    internal class HierarchicalGameScheduleOptimizationModel(ILogger logger) : IScheduleOptimizationModel
    {
        private ScheduleDto? _scheduleDto;

        private readonly ILogger logger = logger; 

        async Task<bool> IScheduleOptimizationModel.BuildScheduleAsync(ScheduleConfig config)
        {
            FirstLevel firstLevel = new(config, logger);
            ScheduleBuildingResult result = await Task.Run(firstLevel.BuildSchedule);
            if (!result.Success) return false;
            BuildScheduleDto(result, config);
            return true;
        }

        ScheduleDto IScheduleOptimizationModel.GetSchedule()
        {
            if (_scheduleDto is null) throw new Exception("Расписание еще не построено");
            return _scheduleDto;
        }

        private void BuildScheduleDto(ScheduleBuildingResult result, ScheduleConfig config)
        {
            _scheduleDto = [];

            if (result.FlSolution is null) return;
            if (result.SlSolution is null) return;

            _scheduleDto.MakeSpan = result.Makespan;

            int scheduleSize = result.FlSolution.M.Sum();
            
            for(int device = 0; device < config.DevicesCount; device++)
            {
                _scheduleDto.Add([]);
                for (int batch = 0; batch < scheduleSize; batch++)
                {
                    for(int job = 0; job < BatchSize(batch, result.SlSolution.R); job++)
                    {
                        int jobType = BatchType(batch, result.SlSolution.P);
                        _scheduleDto[device].Add(new ScheduleElementDto($"T{jobType + 1}", result.SlSolution.T_0l[device][batch][job], config.WorkDurations[device, jobType]));   
                    }
                }
            }
        }

        private int BatchType(int batch, int[,] P)
        {
            for(int jobType = 0; jobType < P.GetLength(0); jobType++)
            {
                if (P[jobType, batch] == 1) return jobType;
            }
            return 0;
        }

        private int BatchSize(int batch, int[,] R)
        {
            for (int jobType = 0; jobType < R.GetLength(0); jobType++)
            {
                if (R[jobType, batch] != 0) return R[jobType, batch];
            }
            return 0;
        }

    }
}
