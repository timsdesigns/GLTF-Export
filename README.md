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
    - [GLTF-Export](#gltf-export)
    - [GLTF-Options (Optional) \[v1.1.0+\]](#gltf-options-optional-v110)

## Workflow
 - some examples ![alt text](<files//gltfRH_ExampleDef2.png>)

## Testfile
 - check out the functionality with this [Grasshopper Definition](files/Workbench_GLTF_Export.ghx)

## Export
 - exported gltf![1st Result](<./files/Screenshot 2024-05-01 180729.png>)

## Motivation
 - I recently saw that shapediver is now suggesting to use the Rhino8 native gltf export, so I wrote the script to leverage that.

## Component Descriptions

### GLTF-Export
 - This Component implements two ways to output the gltfs
 1. It outputs the entire scene
 2. It outputs only the grasshopper geometry

### GLTF-Options (Optional) [v1.1.0+]
 - Here you can edit all the [export settings](https://docs.mcneel.com/rhino/8/help/en-us/fileio/gltf_import_export.htm) in one place
 1. Draco Compression
 2. Formatting
 3. Mesh Settings