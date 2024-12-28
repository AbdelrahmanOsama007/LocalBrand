using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class GridOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string DevelopMessage { get; set; }
        public GridData Data { get; set; }
    }
}
