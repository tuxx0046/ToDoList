using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListApp.Models
{
    public class ToDoViewModel
    {
        public ToDoViewModel()
        {
            CurrentTask = new ToDoTask();
        }

        public Filters Filters { get; set; }
        public List<Status> Statuses { get; set; }
        public List<Category> Categories { get; set; }
        
        public Dictionary<string, string> DueFilters { get; set; }
        public List<ToDoTask> Tasks { get; set; }
        public ToDoTask CurrentTask { get; set; }
    }
}
