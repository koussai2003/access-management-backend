using System.Collections.Generic;

namespace AccessManagementAPI.Dtos
{
    public class ModuleDto
    {
        public string Name { get; set; }
        public List<FunctionDto> Functions { get; set; }
    }
}
