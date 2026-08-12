using System;
using System.Collections.Generic;
using System.Text;

namespace ToDoApp.Domain.Interfaces
{
    public class IEntity<TKey>
    {
        TKey Id {  get; set; }
    }
}
