using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ScheduleOptimizationSystem
{
    public class ScheduleDto : List<List<ScheduleElementDto>>
    {
        public int MakeSpan;

        public int MaxTime
        {
            get
            {
                ScheduleElementDto lastElement = this.Last().Last();
                return lastElement.Start + lastElement.Duration;
            }
        }

    };
    

    public record ScheduleElementDto(
        string Title,
        int Start,
        int Duration
    );
}
