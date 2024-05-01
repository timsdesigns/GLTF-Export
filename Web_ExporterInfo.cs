using Grasshopper.Kernel;
using System.Reflection;
using System.Drawing;

namespace Web_Exporter
{
    public class Web_ExporterInfo : GH_AssemblyInfo
    {
        public override string Name => "Web_Exporter";
        public override Bitmap Icon => Properties.Resources.glTF_LogoBug_48px_Nov17;
        public override string Description => "Enabling 3d Models for web view";
        public override Guid Id => new("74080384-265c-4e59-a504-e2ee43d92151");
        public override string AuthorName => "Tim Fischer";
        public override string AuthorContact => "https://www.comdecon.com";
#if NET7_0_OR_GREATER
        public override string Version => _v.ToString();
#else
        public override string Version => $"{_v.Major}.0.{_v.Build}.{_v.Revision}";
#endif
        private readonly Version _v = Assembly.GetExecutingAssembly().GetName().Version;
    }
}