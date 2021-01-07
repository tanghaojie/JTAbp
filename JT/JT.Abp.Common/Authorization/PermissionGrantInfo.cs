using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Authorization
{
    public class PermissionGrantInfo
    {
        public string Name { get; private set; }

        public bool IsGranted { get; private set; }

        public PermissionGrantInfo(string name, bool isGranted)
        {
            Name = name;
            IsGranted = isGranted;
        }
    }
}
