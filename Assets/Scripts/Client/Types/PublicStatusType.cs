using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Client.Types
{
    public enum PickupStatusType : byte
    {
        Ok = 1,
        OutOfRange = 2,
        InventoryFull = 3,
        Error = 4,
    }
}
