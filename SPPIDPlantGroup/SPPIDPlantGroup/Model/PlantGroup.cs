using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPPIDPlantGroup
{
    public class PlantGroup
    {
        public string SPID { get; set; }
        public string ParentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PlantGroupType { get; set; }
        public string Path { get; set; }
        public bool IsExpanded { get; set; }

        public List<PlantGroup> Children { get; set; }

        public PlantGroup()
        { }
    }
}
