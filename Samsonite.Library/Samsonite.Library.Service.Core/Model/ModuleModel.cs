using System.Collections.Generic;

namespace Samsonite.Library.Service.Core.Model
{
    /// <summary>
    /// 服务对象
    /// </summary>
    public class ModuleModel
    {
        public int ModuleID { get; set; }

        public IModule ModuleInstance { get; set; }
    }

    public class InitializationResponse
    {
        public bool IsInit { get; set; }

        public List<ModuleModel> Modules { get; set; }
    }
}
