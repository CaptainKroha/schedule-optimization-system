using ScheduleOptimizationSystem.HierarchicalGameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem
{
    internal interface IScheduleOptimizationModel
    {
        /// <summary>
        /// Инициирует процесс построения расписания
        /// </summary>
        /// <param name="config">Конфигурация конвейерной системы и решаемой задачи</param>
        /// <returns>Истина в случае успешного построения, Ложь - в случае неудачи</returns>
        Task<bool> BuildScheduleAsync(ScheduleConfig config);

        /// <summary>
        /// Получение результатов построения расписания
        /// </summary>
        /// <returns>Расписание и его параметры в виде ScheduleDto</returns>
        ScheduleDto GetSchedule();
    }
}
