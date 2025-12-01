using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskPocket.DAL.Models.Enum
{
   public enum TaskFilter
    {
            All,
            Completed,
            Pending,
            Overdue,
            DueInThreeDays,
            Prioritized
    }
}
