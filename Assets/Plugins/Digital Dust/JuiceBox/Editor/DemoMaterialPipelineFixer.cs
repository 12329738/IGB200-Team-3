using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

// ==============================================================================
//  DemoMaterialPipelineFixer: retargets the demo material to the shader matching
//  the active render pipeline (Standard, URP, or HDRP). Driven by a button on the
//  readme asset rather than a load hook, so projects that never open the demo --
//  almost all of them, after the first day -- pay nothing.
// ==============================================================================
namespace JuiceBox
{
    static class DemoMaterialPipelineFixer
    {
        const string MaterialGuid = "d8e9c7f6b5a4314253637485a6b7c8d9"; // DemoCube

        const string ShaderStandard = "Standard";
        const string ShaderURP = "Universal Render Pipeline/Lit";
        const string ShaderHDRP = "HDRP/Lit";

        enum Pipeline { Standard, URP, HDRP }

        internal static bool HasDemoMaterials()
        {
            return LoadMaterial(MaterialGuid) != null;
        }

        internal static string ButtonLabel()
        {
            return "Fix demo material for " + GetLabel(DetectPipeline());
        }

        internal static bool Fix(out string message)
        {
            Pipeline pipeline = DetectPipeline();
            string label = GetLabel(pipeline);
            string targetShaderName = GetShaderName(pipeline);

            Shader shader = Shader.Find(targetShaderName);
            if (shader == null)
            {
                message = "Could not find the shader \"" + targetShaderName +
                    "\", so the demo material was left alone.";
                return false;
            }

            Material mat = LoadMaterial(MaterialGuid);
            if (mat == null || mat.shader == shader)
            {
                message = "The demo material already targets " + label + ".";
                return true;
            }

            mat.shader = shader;
            EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssetIfDirty(mat);

            message = "Retargeted the demo material to " + label + ".";
            return true;
        }

        static Material LoadMaterial(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
                return null;
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }

        static Pipeline DetectPipeline()
        {
            RenderPipelineAsset rpa = GraphicsSettings.currentRenderPipeline;
            if (rpa == null)
                return Pipeline.Standard;

            string typeName = rpa.GetType().Name;
            if (typeName.Contains("Universal"))
                return Pipeline.URP;
            if (typeName.Contains("HDRender"))
                return Pipeline.HDRP;

            return Pipeline.Standard;
        }

        static string GetShaderName(Pipeline pipeline)
        {
            if (pipeline == Pipeline.URP)
                return ShaderURP;
            if (pipeline == Pipeline.HDRP)
                return ShaderHDRP;
            return ShaderStandard;
        }

        static string GetLabel(Pipeline pipeline)
        {
            if (pipeline == Pipeline.URP)
                return "URP";
            if (pipeline == Pipeline.HDRP)
                return "HDRP";
            return "the Built-in pipeline";
        }
    }
}
