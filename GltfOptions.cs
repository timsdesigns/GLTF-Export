using Grasshopper.Kernel;
using Rhino.FileIO;

namespace Web_Exporter
{
    public class GltfOptions : GH_Component
    {
        public GltfOptions()
          : base("Gltf Options", "GltfOptions",
              "Options used when writing a glTF file.",
            "Web", "Gltf")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Cull Backfaces", "CullBackfaces", "Setting determines whether meshes in glTF will be rendered with or without backface culling. Known as DoubleSided in the glTF specification.", GH_ParamAccess.item, true);//bool cullBackfaces,
            pManager.AddIntegerParameter("Draco CompressionLevel", "DracoCompressionLevel", "Level of compression used by draco in range of 1 to 10 inclusive.", GH_ParamAccess.item, 10);//int dracoCompressionLevel,
            pManager.AddIntegerParameter("Draco Quantization Bits Normal", "DracoQuantizationBitsNormal", "Number of bits used when quantizing mesh normals in range of 8 to 32 inclusive.", GH_ParamAccess.item, 8);//int dracoQuantizationBitsNormal,
            pManager.AddIntegerParameter("Draco Quantization Bits Position", "DracoQuantizationBitsPosition", "Number of bits used when quantizing mesh vertex positions in range of 8 to 32 inclusive.", GH_ParamAccess.item, 11);//int dracoQuantizationBitsPosition,
            pManager.AddIntegerParameter("Draco Quantization Bits Texture Coordinate", "DracoQuantizationBitsTextureCoordinate", "Number of bits used when quantizing mesh texture coordinates in the range of 8 to 32 inclusive.", GH_ParamAccess.item, 10);//int dracoQuantizationBitsTextureCoordinate,
            pManager.AddBooleanParameter("Export Layers", "ExportLayers", "glTF uses a scene hierarchy structure, where nodes are organized in a parent-child relationship. This setting enables/disables writing of empty nodes with same names as layers and places exported objects a children of corresponding layer node.", GH_ParamAccess.item, false);//bool exportLayers,
            pManager.AddBooleanParameter("Export Materials", "ExportMaterials", "Setting determines whether materials are written to glTF file.", GH_ParamAccess.item, true);//bool exportMaterials,
            pManager.AddBooleanParameter("Export Open Meshes", "ExportOpenMeshes", "Enable/disable export of open meshes.", GH_ParamAccess.item, true);//bool exportOpenMeshes,
            pManager.AddBooleanParameter("Export Texture Coordinates", "ExportTextureCoordinates", "Enable/disable export of texture coordinates.", GH_ParamAccess.item, true);//bool exportTextureCoordinates,
            pManager.AddBooleanParameter("Export Vertex Colors", "ExportVertexColors", "Enable/disable export of vertex colors.", GH_ParamAccess.item, true);//bool exportVertexColors,
            pManager.AddBooleanParameter("Export Vertex Normals", "ExportVertexNormals", "Enable/disable export of vertex normals.", GH_ParamAccess.item, true);//bool exportVertexNormals,
            pManager.AddBooleanParameter("MapZ to Y", "MapZToY", "Set to transform Rhino's Z-axis to glTF's Y-axis.", GH_ParamAccess.item, true);//bool mapZToY,
            pManager.AddIntegerParameter("SubD Mesh Type", "SubDMeshType", "The mesh type for exported SubDs.", GH_ParamAccess.item, 0);//int subDMeshType,
            pManager.AddIntegerParameter("SubD Surface Meshing Density", "SubDSurfaceMeshingDensity", "Determines how coarse mesh output will be, when surface meshing subd objects. See (RhinoCommon) comments for ON_SubDDisplayParameters in opennurbs_mesh.h for details regarding numbers used.", GH_ParamAccess.item, 4);//int subDSurfaceMeshingDensity,
            pManager.AddBooleanParameter("Use Display Color For Unset Materials", "UseDisplayColorForUnsetMaterials", "Set to display color as material when object mat. index is -1.", GH_ParamAccess.item, true);//bool useDisplayColorForUnsetMaterials,
            pManager.AddBooleanParameter("Use Draco Compression", "UseDracoCompression", "Enable/disable use of Draco mesh compression in glTF file.", GH_ParamAccess.item, true);//bool useDracoCompression,
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("FileGltfWriteOptions", "options", "Write file options.", GH_ParamAccess.item);
            pManager.AddGenericParameter("ArchivableDictionary", "ArchiveCollection", "Export selection options.", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            FileGltfWriteOptions optionsGltf = new();
            bool cullBackfaces=false; DA.GetData("Cull Backfaces", ref cullBackfaces); optionsGltf.CullBackfaces = cullBackfaces;
            int dracoCompressionLevel=0; DA.GetData("Draco CompressionLevel", ref dracoCompressionLevel); optionsGltf.DracoCompressionLevel = dracoCompressionLevel;
            int dracoQuantizationBitsNormal=0; DA.GetData("Draco Quantization Bits Normal", ref dracoQuantizationBitsNormal); optionsGltf.DracoQuantizationBitsNormal = dracoQuantizationBitsNormal;
            int dracoQuantizationBitsPosition=0; DA.GetData("Draco Quantization Bits Position", ref dracoQuantizationBitsPosition); optionsGltf.DracoQuantizationBitsPosition = dracoQuantizationBitsPosition;
            int dracoQuantizationBitsTextureCoordinate=0; DA.GetData("Draco Quantization Bits Texture Coordinate", ref dracoQuantizationBitsTextureCoordinate); optionsGltf.DracoQuantizationBitsTextureCoordinate = dracoQuantizationBitsTextureCoordinate;
            bool exportLayers=false; DA.GetData("Export Layers", ref exportLayers); optionsGltf.ExportLayers = exportLayers;
            bool exportMaterials=false; DA.GetData("Export Materials", ref exportMaterials); optionsGltf.ExportMaterials = exportMaterials;
            bool exportOpenMeshes=false; DA.GetData("Export Open Meshes", ref exportOpenMeshes); optionsGltf.ExportOpenMeshes = exportOpenMeshes;
            bool exportTextureCoordinates=false; DA.GetData("Export Texture Coordinates", ref exportTextureCoordinates); optionsGltf.ExportTextureCoordinates = exportTextureCoordinates;
            bool exportVertexColors=false; DA.GetData("Export Vertex Colors", ref exportVertexColors); optionsGltf.ExportVertexColors = exportVertexColors;
            bool exportVertexNormals=false; DA.GetData("Export Vertex Normals", ref exportVertexNormals); optionsGltf.ExportVertexNormals = exportVertexNormals;
            bool mapZToY=false; DA.GetData("MapZ to Y", ref mapZToY); optionsGltf.MapZToY = mapZToY;
            int subDMeshType=0; DA.GetData("SubD Mesh Type", ref subDMeshType); optionsGltf.SubDMeshType = (FileGltfWriteOptions.SubDMeshing)subDMeshType;
            int subDSurfaceMeshingDensity=0; DA.GetData( "SubD Surface Meshing Density", ref subDSurfaceMeshingDensity); optionsGltf.SubDSurfaceMeshingDensity = subDSurfaceMeshingDensity;
            bool useDisplayColorForUnsetMaterials=false; DA.GetData( "Use Display Color For Unset Materials", ref useDisplayColorForUnsetMaterials); optionsGltf.UseDisplayColorForUnsetMaterials = useDisplayColorForUnsetMaterials;
            bool useDracoCompression = false; DA.GetData("Use Draco Compression", ref useDracoCompression); optionsGltf.UseDracoCompression = useDracoCompression;
            DA.SetData("FileGltfWriteOptions", optionsGltf);
            DA.SetDataList("ArchivableDictionary", optionsGltf.ToDictionary());
        }
        protected override System.Drawing.Bitmap Icon => Properties.Resources.gltfOptionsGH;
        public override Guid ComponentGuid => new("41A329B1-8BF5-40EE-A816-C2C02CA3B470");
    }
}

