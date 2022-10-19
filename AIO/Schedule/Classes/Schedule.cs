using System;
using System.Collections.Generic;

namespace AIO
{
    public class Schedule
    {
        public int Month { get; set; }
        public List<_Event> Event { get; set; }
    }

    public class _Event
    {
        public string Event_Name { get; set; }
        public int Start_Date { get; set; }
        public int End_Date { get; set; }
    }
}
