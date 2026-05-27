using System;
using System.Collections.Generic;
using System.Text;
using static ScheduleOptimizationSystem.HierarchicalGameModel.SecondLevel;

namespace ScheduleOptimizationSystem
{
    internal class StartProcessingTimesCalculator(ScheduleConfig config)
    {

        private int[][][] SPTMatrixes = new int[config.DevicesCount][][];

        public int[][][] Calculate(in List<Batch> schedule)
        {
            for (int device = 0; device < config.DevicesCount; device++)
            {
                SPTMatrixes[device] = new int[schedule.Count][];
                for (int batch = 0; batch < schedule.Count; batch++)
                {
                    SPTMatrixes[device][batch] = new int[schedule[batch].Size];
                }
            }

            CalcT_01(schedule);
            for (int device = 1; device < config.DevicesCount; device++)
            {
                CalcT_0N(device, schedule);
            }

            return SPTMatrixes;
        }

        private void CalcT_01(List<Batch> schedule)
        {
            int device = 0, batch = 0, job = 0;

            int[][] firstDeviceSPTMatrix = SPTMatrixes[0];

            // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на 1 приборе, как наладку
            firstDeviceSPTMatrix[batch][job] = config.ChangeoverDurations[device][schedule[batch].Type, schedule[batch].Type];

            for (job = 1; job < schedule[batch].Size; job++)
            {
                firstDeviceSPTMatrix[batch][job] = JobCompletionTime(device, batch, job - 1, schedule[batch].Type);
            }


            // Пробегаемся по всем возможным позициям cо второго пакета
            for (batch = 1; batch < schedule.Count; batch++)
            {
                job = 0;

                // Момент начала времени выполнения 1 задания в пакете на позиции batch
                firstDeviceSPTMatrix[batch][job] = JobCompletionTime(device, batch - 1, schedule[batch - 1].Size - 1, schedule[batch - 1].Type)
                    + config.ChangeoverDurations[device][schedule[batch - 1].Type, schedule[batch].Type];

                for (job = 1; job < schedule[batch].Size; job++)
                {
                    firstDeviceSPTMatrix[batch][job] = JobCompletionTime(device, batch, job - 1, schedule[batch].Type);
                }
            }
        }

        private void CalcT_0N(int device, List<Batch> schedule)
        {
            int batch = 0, job = 0;
            int[][] deviceSPTMatrix = SPTMatrixes[device];

            // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на приборе device, как
            // Максимум, между временем наладки прибора на выполнение 1 задания в 1 пакете
            // и временем окончания выполнения 1 задания в 1 пакете на предыдущем приборе
            deviceSPTMatrix[batch][job] = Math.Max(

                // Время наладки прибора на выполнение 1 задания в 1 пакете
                config.ChangeoverDurations[device][schedule[batch].Type, schedule[batch].Type],

                // Время окончания выполнения 1 задания в 1 пакете на предыдущем приборе
                JobCompletionTime(device - 1, batch, job, schedule[batch].Type)
            );

            // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
            for (job = 1; job < schedule[batch].Size; job++)

                // Устанавливаем момент начала времени выполнения текущего задания job, как
                // Максимум, между временем окончания предыдущего задания на текущем приборе и
                // временем окончания текущего задания на предыдущем приборе
                deviceSPTMatrix[batch][job] = Math.Max(
                    JobCompletionTime(device, batch, job - 1, schedule[batch].Type),
                    JobCompletionTime(device - 1, batch, job, schedule[batch].Type)
                );

            for (batch = 1; batch < schedule.Count; batch++)
            {
                job = 0;

                // Устанавливаем момент начала времени выполнения 1 задания в пакете batch на приборе device,
                // как Максимум, между временем окончания выполнения последнего задания в предыдущем пакете вместе с переналадкой 
                // и временем окончания выполнения 1 задания в пакете на в batch на предыдущем приборе
                deviceSPTMatrix[batch][job] = Math.Max(
                    JobCompletionTime(device, batch - 1, schedule[batch - 1].Size - 1, schedule[batch - 1].Type) + config.ChangeoverDurations[device][schedule[batch - 1].Type, schedule[batch].Type],
                    JobCompletionTime(device - 1, batch, job, schedule[batch].Type)
                );

                // Пробегаемся по всем возможным заданиям пакета в позиции batch
                for (job = 1; job < schedule[batch].Size; job++)

                    // Устанавливаем момент начала времени выполнения текущего задания job, как
                    // Максимум, между временем окончания предыдущего задания на текущем приборе и
                    // временем окончания текущего задания на предыдущем приборе
                    deviceSPTMatrix[batch][job] = Math.Max(
                        JobCompletionTime(device, batch, job - 1, schedule[batch].Type),
                        JobCompletionTime(device - 1, batch, job, schedule[batch].Type)
                    );
            }
        }

        private int JobCompletionTime(int device, int batch, int job, int type)
        {
            return SPTMatrixes[device][batch][job] + config.WorkDurations[device, type];
        }

    }
}
