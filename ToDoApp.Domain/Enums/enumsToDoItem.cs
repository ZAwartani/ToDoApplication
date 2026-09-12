using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Domain.Enums
{
    public enum EnumPriority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }

    public enum EnumRecurring
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    public enum EnumCompleteStatus
    {
        NotComplete = 0,
        Complete = 1
    }

}
