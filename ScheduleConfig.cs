using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleOptimizationSystem
{
    public record ScheduleConfig(
        int DevicesCount,
        int JobTypesCount,
        int OrderTypesCount,
        int BufferSize,
        int[,] WorkDurations,
        int[][,] ChangeoverDurations,
        int[,] OrderTypes,
        int[] Orders
    );
}
