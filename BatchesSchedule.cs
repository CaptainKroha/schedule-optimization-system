using System;
using System.Collections.Generic;
using System.Text;
using static ScheduleOptimizationSystem.HierarchicalGameModel.SecondLevel;

namespace ScheduleOptimizationSystem
{
    public class BatchesSchedule(int scheduleSize)
    {

        private record Batch(int Type, int Size);

        private readonly Batch[] schedule = new Batch[scheduleSize];
        private int scheduleSize = 0;

        public BatchesSchedule(BatchesSchedule copiedSchedule) : this(copiedSchedule.Size)
        {
            scheduleSize = copiedSchedule.scheduleSize;
            schedule = [.. copiedSchedule.schedule];
        }

        public int Size { get { return scheduleSize; } }

        public void Add(int type, int size)
        {
            schedule[scheduleSize++] = new(type, size);
        }

        public int TypeOf(int batch)
        {
            return schedule[batch].Type;
        }

        public int SizeOf(int batch)
        {
            return schedule[batch].Size;
        }

        public void Switch(int batch1, int batch2)
        {
            (schedule[batch1], schedule[batch2]) = (schedule[batch2], schedule[batch1]);
        }
    }
}
