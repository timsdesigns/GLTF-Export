# Web_Exporter
A Grasshopper Plugin for web export of 3d models

## Content
- [Web\_Exporter](#web_exporter)
  - [Content](#content)
  - [Workflow](#workflow)
  - [Testfile](#testfile)
  - [Export](#export)
  - [Motivation](#motivation)
  - [Component Descriptions](#component-descriptions)
    - [ GLTF-Export](#-gltf-export)
    - [ GLTF-Options (Optional) \[v1.1.0+\]](#-gltf-options-optional-v110)
    - [Adding Materials](#adding-materials)

## Workflow
 - some examples ![alt text](<files//gltfRH_ExampleDef2.png>)

## Testfile
 - check out the functionality with this [Grasshopper Definition](files/Workbench_GLTF_Export.ghx)

## Export
 - exported gltf![1st Result](<./files/Screenshot 2024-05-01 180729.png>)

## Motivation
 - I recently saw that shapediver is now suggesting to use the Rhino8 native gltf export, so I wrote the script to leverage that.

## Component Descriptions

### ![Component Icon](/images/gltfGH.png) GLTF-Export
 - This Component implements two ways to output the gltfs
 1. It outputs the entire scene
 2. It outputs only the grasshopper geometry

### ![Component Icon](/images/gltfOptionsGH.png) GLTF-Options (Optional) [v1.1.0+]
 - Here you can edit all the <a href="https://docs.mcneel.com/rhino/8/help/en-us/fileio/gltf_import_export.htm" target="_blank">export settings</a> in one place
 1. Draco Compression
 2. Formatting
 3. Mesh Settings

 ### Adding Materials
 - A simple video each; showing:
   - <video src="./files/ExportWithMaterial.mp4" height="240" controls>how to use this in general</video>
   - <video src="./files/ExportWithDownloadedMaterial.mp4" height="240" controls>how to use this with a downloaded material</video>
   - <video src="https://vimeo.com/manage/videos/942102502" height="240" controls>how to use this in general</video>
   - <video src="https://vimeo.com/manage/videos/942102471" height="240" controls>how to use this with a downloaded material</video>
 - Find out more about <a href="https://docs.mcneel.com/rhino/8/help/en-us/commands/materials.htm#physicallybased" target="_blank"><img src="https://docs.mcneel.com/rhino/8/help/en-us/image/icons/material_pbr.png" alt="" height="20"> how to create Physically Based Materials in the Rhino Documentation</a>
