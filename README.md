# Web_Exporter
A Grasshopper Plugin for web export of 3d models

## Content
- [Web\_Exporter](#web_exporter)
  - [Content](#content)
  - [Workflow](#workflow)
  - [Export](#export)
  - [Motivation](#motivation)
  - [Component Descriptions](#component-descriptions)
    - [GLTF-Export](#gltf-export)
    - [GLTF-Options (Optional)](#gltf-options-optional)

## Workflow
 - some examples ![alt text](<files/gltfRH_ExampleDef.png>)

## Export
 - exported gltf![1st Result](<./files/Screenshot%202024-04-29%20225810.png>)

## Motivation
 - I recently saw that shapediver is now suggesting to use the Rhino8 native gltf export, so I wrote the script to leverage that.

## Component Descriptions

### GLTF-Export
 - This Component implements two ways to output the gltfs
 1. It outputs the entire scene
 2. It outputs only the grasshopper geometry

### GLTF-Options (Optional)
>To implement
 - Here you can edit all the export settings in one place
 1. Draco Compression
 2. Formatting
 3. Mesh Settings