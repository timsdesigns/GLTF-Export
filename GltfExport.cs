using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.FileIO;
using Rhino.Geometry;
using System.Collections.Concurrent;

namespace Web_Exporter
{
    public class GltfExport : GH_Component
    {
        private string _lastResult = "";
        public GltfExport()
          : base("Gltf Exporter", "GltfExporter",
            "Exports geometries or rhino scene as gltf file for web view",
            "Web", "Gltf")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Export", "Exp", "Use button to export when needed.", GH_ParamAccess.item);                    //0
            pManager.AddTextParameter("File Name", "Name", "Name your output file.", GH_ParamAccess.item, "model_01");           //1
            pManager.AddTextParameter("Folder", "Path", "The folder you want to store the output in.", GH_ParamAccess.item);            //2
            pManager.AddGeometryParameter("Geometry Collection", "Geo", "Add the geometry you like to export.", GH_ParamAccess.tree);   //3
            pManager[3].Optional = true;
            pManager.AddTextParameter("Material Name", "Material", "Add a name of a predefined material to export the model with.", GH_ParamAccess.tree);//4
            pManager[4].Optional = true;
            pManager.AddGenericParameter("FileGltfWriteOptions", "Options", "Write file options.", GH_ParamAccess.item);                //5
            pManager[5].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager) =>
            pManager.AddTextParameter("Errors and Feedback", "E", "Preliminary feedback.", GH_ParamAccess.item);
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region imputs
            bool export = false; DA.GetData("Export", ref export);
            string name = ""; DA.GetData("File Name", ref name);
            string loc = ""; DA.GetData("Folder", ref loc);
            DA.GetDataTree(3, out GH_Structure<IGH_GeometricGoo>? geo);
            DA.GetDataTree(4, out GH_Structure<GH_String>? material);
            object? options = null; DA.GetData("FileGltfWriteOptions", ref options);
            #endregion
            // Sanity
            if (loc == null || !Directory.Exists(loc))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Path does not exist on machine.");
                return;
            }
            var filePath = loc;
            filePath += $@"\{name}.gltf";
            // Label Input
            var button = Params.Input.Find(p => p.NickName == "Exp")!.Sources[0].Attributes.DocObject;
            if (button.GetType() == typeof(Grasshopper.Kernel.Special.GH_ButtonObject))
                ((Grasshopper.Kernel.Special.GH_ButtonObject)button).NickName = "Export";
            // Mode
            bool scene = false;
            Message = "Geometry";
            if (geo.DataCount < 1)// == null)
            {
                string msg = "Writing entire scene since \"geo\" failed to collect data";
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, msg);
                scene = true;
                Message = "Scene";
            }

            FileGltfWriteOptions optionsGltf =
                options != null
                && options is FileGltfWriteOptions ?
                options as FileGltfWriteOptions : DefaultOptions();

            if (!export) return;
            string res = "";
            List<Guid> ids = [];
            GH_Structure<GH_Guid> idTree = new();
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            try
            {
                switch (scene)
                {
                    case false:
                        Rhino.Collections.ArchivableDictionary archiveCollection = optionsGltf!.ToDictionary();

                        // creating temporary geometry(ies)
                        Rhino.DocObjects.ObjectAttributes att = new()
                        {
                            LayerIndex = 0,
                            MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromLayer
                        };
                        // material assignment and baking
                        Task.Run(() => AssignMaterialsAsync(geo, material, doc)).ContinueWith(t =>
                        {
                            // This code runs on the background thread after AssignMaterialsAsync completes, Marshal UI updates back to the main thread
                            RhinoApp.InvokeOnUiThread(() => RhinoApp.Write("Object(s) baked\n", true));
                        }).Wait();

                        async Task AssignMaterialsAsync(GH_Structure<IGH_GeometricGoo> geo, GH_Structure<GH_String> material, RhinoDoc doc)
                        {
                            int path = 0;
                            foreach (var branch in geo!.Branches)
                            {
                                var matLst = material.Branches[RepeatLast(material.Paths, path++)]; // get mat branch list (repeating last if less)
                                int index = 0;
                                foreach (var g in branch)
                                {
                                    // assigning custom materials if exists (repeating last item if less)
                                    if (matLst != null)
                                    {
                                        var matName = matLst[RepeatLast(matLst, index++)].Value;
                                        var materialRH = doc.RenderMaterials
                                            .FirstOrDefault(n => n.Name == matName); // get render mat
                                        if (materialRH != null)
                                            att.RenderMaterial = materialRH;
                                    }

                                    // baking
                                    g.CastTo<GeometryBase>(out GeometryBase geometry);
                                    Guid id = doc.Objects.Add(geometry, att);
                                    ids.Add(id);
                                }
                            }
                            doc.Views.Redraw();
                        }

                        RhinoApp.InvokeOnUiThread(() =>
                        {
                            // select object(s) into list
                            foreach (Guid id in ids)
                                doc.Objects.Select(id, true);
                            RhinoApp.Write("Object(s) selected\n", true);

                            // attempt export temp baked object(s)
                            if (doc.ExportSelected(filePath, archiveCollection))
                                res = $"File written successfully to path: {filePath}";
                            else res = "Something went wrong";
                        });
                        break;
                    default:
                        // attempt export entire document scene
                        if (FileGltf.Write(filePath, doc, optionsGltf))
                            res = $"File written successfully to path: {filePath}";
                        else res = "Something went wrong";
                        break;
                }
            }
            catch (Exception e)
            {
                res += "\nError\n" + e;
            }
            finally
            {
                RhinoApp.InvokeOnUiThread(() =>
                {
                    RhinoApp.Write($"Export attempted to directory: {loc}\n", true);
                    if (!scene)
                    {
                        // delete baked geometry quiet
                        foreach (Guid id in ids)
                            doc.Objects.Delete(id, true);
                        RhinoApp.Write("Object(s) deleted\n", true);
                    }
                });
                _lastResult = res;
            }
            #region Outputs
            DA.SetData("Errors and Feedback", _lastResult);
            #endregion Outputs
        }
        private static FileGltfWriteOptions DefaultOptions() =>
            new()
            {
                CullBackfaces = true,
                DracoCompressionLevel = 10,
                DracoQuantizationBitsNormal = 8,
                DracoQuantizationBitsPosition = 11,
                DracoQuantizationBitsTextureCoordinate = 10,
                ExportLayers = false,
                ExportMaterials = true,
                ExportOpenMeshes = true,
                ExportTextureCoordinates = true,
                ExportVertexColors = true,
                ExportVertexNormals = true,
                MapZToY = true,
                SubDMeshType = FileGltfWriteOptions.SubDMeshing.Surface,
                SubDSurfaceMeshingDensity = 4,
                UseDisplayColorForUnsetMaterials = true,
                UseDracoCompression = true
            };
        /// <summary>Repeat last if index exceeds collection count.</summary>
        private static int RepeatLast<T>(IList<T> collection, int index) where T : class =>
            index < collection.Count ? index : collection.Count - 1;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.gltfGH;
        public override Guid ComponentGuid => new("d954350d-dcb0-486e-b78b-dd251424ec4a");
    }
}
