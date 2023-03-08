using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDbGui.Data
{
    public enum RecordState
    {
        Initial = 0,
        New,
        Hollow,
        Clean,
        Dirty,
        Deleted
    }
}
