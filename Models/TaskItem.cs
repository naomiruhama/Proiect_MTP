using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlowStudent.Models
{
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public DateTime Deadline { get; set; }
        public TaskStatus Status { get; set; }  
    }

}
