using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Rhino;
using Rhino.FileIO;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Web_Exporter
{
    public class GltfExport : GH_Component
    {
        private string _lastResult = "";
        public GltfExport()
          : base("Gltf Exporter", "gltfExp",
            "Exports geometries or rhino scene as gltf file for web view",
            "Web", "Gltf")
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Export", "Exp", "Use button to export when needed.", GH_ParamAccess.item, false);      //0
            pManager.AddTextParameter("File Name", "Name", "Name your output file.", GH_ParamAccess.item, "model_01");           //1
            pManager.AddTextParameter("Folder", "Path", "The folder you want to store the output in.", GH_ParamAccess.item);            //2
            pManager.AddGeometryParameter("Geometry Collection", "Geo", "Add the geometry you like to export.", GH_ParamAccess.tree);   //3
            pManager[3].Optional = true;
            pManager.AddTextParameter("Material Name", "Material", "Add a name of a predefined material to export the model with.", GH_ParamAccess.tree);//4
            pManager[4].Optional = true;
            pManager.AddGenericParameter("FileGltfWriteOptions", "Options", "Write file options.", GH_ParamAccess.item);                //5
            pManager[5].Optional = true;
            pManager.AddGenericParameter("ArchivableDictionary", "ArchiveCollection", "Export selection options.", GH_ParamAccess.item);//6
            pManager[6].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager) =>
            pManager.AddTextParameter("Errors and Feedback", "E", "Preliminary feedback.", GH_ParamAccess.item);
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region imputs
            bool export = false;
            string name = "";
            string loc = "";
            //GH_Structure<IGH_Goo>? geo = null;
            //GH_Structure<GH_String>? material = null;
            object? options = null;
            Rhino.Collections.ArchivableDictionary? aCollection = null;
            DA.GetData("Export", ref export);
            DA.GetData("File Name", ref name);
            DA.GetData("Folder", ref loc);
            DA.GetDataTree(3, out GH_Structure<IGH_GeometricGoo>? geo);
            DA.GetDataTree(4, out GH_Structure<GH_String>? material);
            DA.GetData("FileGltfWriteOptions", ref options);
            DA.GetData("ArchivableDictionary", ref aCollection);
            #endregion
            // Sanity
            if (loc == null || !Directory.Exists(loc))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Path does not exist on machine.");
                return;
            }
            var filePath = loc;
            filePath += $@"\{name}.gltf";

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
                && options! is FileGltfWriteOptions ?
                options as FileGltfWriteOptions : DefaultOptions();

            if (!export) return;
            string res = "";
            List<Guid> ids = new List<Guid>();
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            try
            {
                switch (scene)
                {
                    case false:
                        Rhino.Collections.ArchivableDictionary archiveCollection =
                            aCollection is Rhino.Collections.ArchivableDictionary ?
                            aCollection as Rhino.Collections.ArchivableDictionary :
                            optionsGltf!.ToDictionary();

                        // creating temporary geometry(ies)
                        Rhino.DocObjects.ObjectAttributes att = new Rhino.DocObjects.ObjectAttributes
                        {
                            LayerIndex = 0,
                            MaterialSource = Rhino.DocObjects.ObjectMaterialSource.MaterialFromLayer
                        };
                        foreach (var branch in geo!.Branches)
                            foreach (var g in branch)
                            {
                                g.CastTo<GeometryBase>(out GeometryBase geometry);
                                ids.Add(doc.Objects.Add(geometry, att));
                            }
                        RhinoApp.Write("Object(s) baked\n", true);

                        // select the object(s)
                        foreach (var id in ids)
                            doc.Objects.Select(id, true);
                        RhinoApp.Write("Object(s) selected\n", true);

                        // Assign the material(s) to the object(s) if exist(s)
                        int b = 0;
                        int i = 0;
                        foreach (var branch in material.Branches)
                        {
                            foreach (var m in branch)
                            {
                                var materialRH = doc.RenderMaterials.FirstOrDefault(n => n.Name == m.Value);
                                if (materialRH != null)
                                {
                                    var rhObj = doc.Objects.FindId(ids[(b * i++) % geo.Paths[b].Length]); //TODO: check if that math works
                                    rhObj.RenderMaterial = materialRH;
                                    rhObj.CommitChanges();
                                    doc.Views.Redraw();
                                    RhinoApp.Write("Object material assigned\n", true);
                                }
                            }
                            b++;
                        }

                        // attempt export temp baked object
                        if (doc.ExportSelected(filePath, archiveCollection))
                            res = $"File written successfully to path: {filePath}";
                        else res = "Something went wrong";

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
                RhinoApp.Write("Export attempted\n", true);
                if (!scene)
                {
                    // delete baked geometry quiet
                    foreach (var id in ids)
                        doc.Objects.Delete(id, true);
                    RhinoApp.Write("Object(s) deleted\n", true);
                }
                _lastResult = res;
            }
            #region Outputs
            DA.SetData("Errors and Feedback", _lastResult);
            #endregion Outputs
        }
        private FileGltfWriteOptions DefaultOptions() =>
            new FileGltfWriteOptions()
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
        public override Guid ComponentGuid => new Guid("d954350d-dcb0-486e-b78b-dd251424ec4a");
    }
}
