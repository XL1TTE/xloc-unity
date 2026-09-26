using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace xlLoc.Editor
{
    [ScriptedImporter(1, new[] { "yml" })]
    public sealed class YamlTextImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string text = File.ReadAllText(ctx.assetPath);
            var asset = new TextAsset(text);
            ctx.AddObjectToAsset("main", asset);
            ctx.SetMainObject(asset);
        }
    }
}
