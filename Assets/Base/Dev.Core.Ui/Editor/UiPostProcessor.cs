using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Dev.Core.Ui.UI.Panels;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dev.Core.Ui.Editor
{
    public static class UiPostProcessor/* : AssetPostprocessor*/
    {
        /// <summary>
        /// Mean that we work inside Package project Flime.<meta>
        /// </summary>

        private static string OverridenScriptPath { get; set; } = s_targetScriptPath;

        private static List<PanelInfo> s_uiPanelTypes;

        /// <summary>
        /// For package content, can be null in case of projectPackage
        /// </summary>
        private static List<PanelInfo> s_uiPanelTypesPackage = null; 
        
        private static readonly string s_targetScriptFolder = "Assets/Modules/Dev.Core.Ui/Scripts/";
        //private static readonly string s_targetScriptFolder = "Assets/UiPanelScripts/";
        //private static readonly string s_targetScriptFolder = "Packages/com.dev.core.ui/Scripts/UiManager/";

        private static readonly string s_targetScriptPath = $"{s_targetScriptFolder}{TARGET_SCRIPT_NAME}.cs";
        private static readonly string s_targetDefinitionPath = $"{s_targetScriptFolder}{TARGET_DEFINITION_NAME}.asmdef";

        private static string TargetScriptPathInPackage = $"Assets/Modules/Core.Ui/Scripts/UiManager/{TARGET_SCRIPT_NAME}.cs";

        // Start panelPath for parse from. 
        private const string PANEL_PATHES = "Assets/";
        private const string TARGET_PACKAGE_NAME = "Modules.Core.Ui.Scripts";
        private const string TARGET_SCRIPT_NAME = "UIPanels";
        private const string TARGET_DEFINITION_NAME = "Modules.UiPanels";

        private class PanelInfo
        {
            public static PanelInfo CreateFromAssetPath(string assetPath)
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                var panelTypeName = GetPanelTypeNameByAssetPath(assetPath);
                var panelPrefabName = GetPrefabNameByAssetPath(assetPath);
                
                return new PanelInfo
                {
                    Guid = guid,
                    TypeName = panelTypeName,
                    PrefabName = panelPrefabName
                };
            }

            public string ToFileConstString()
            {
                return "{" + $"\"{TypeName}\", \"{Guid}\"" + "}";
            }

            public string PrefabName { get; set; }
            public string TypeName { get; set; }
            public string Guid { get; set; }
        }

        static UiPostProcessor()
        {
            EditorApplication.delayCall += Init;
        }

        private static void Init()
        {
            if (AssetDatabase.LoadAssetAtPath<MonoScript>(s_targetScriptPath) == null &&
                AssetDatabase.LoadAssetAtPath<MonoScript>(TargetScriptPathInPackage) == null)
            {
                Debug.Log($"Init {TARGET_SCRIPT_NAME}.cs");
                RefreshPanelTypes();
            }
        }

        private static string[] FindAllPanelPrefabPathes()
        {
            List<string> panelPathes = new List<string>();
            var assetGuids = AssetDatabase.FindAssets("t:prefab");
            foreach (var assetGuid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (IsPanelAsset(assetPath))
                {
                    panelPathes.Add(assetPath);
                }
            }

            return panelPathes.ToArray();
        }

        private static List<PanelInfo> UiPanelTypes
        {
            get
            {
                if (s_uiPanelTypes == null)
                {
                    ParseUiPanelTypes();
                    Assert.IsNotNull(s_uiPanelTypes);
                }

                return s_uiPanelTypes;
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            var isSomeChanged = false;
            
            // Reparse data if this files imported (because can be imported not from this script, but by Package
            if (importedAssets.Where(IsUiPanelTypeAsset).Any())
            {
                s_uiPanelTypes = null;
                s_uiPanelTypesPackage = null;
                ParseUiPanelTypes();
            }

            foreach (var asset in importedAssets.Where(IsPanelAsset))
            {
                if (AddType(asset)) isSomeChanged = true;
            }

            foreach (var assetPath in deletedAssets)
            {
                var deletedPrefabName = GetPrefabNameByAssetPath(assetPath);
                var panelInfo = UiPanelTypes.Find(panel => panel.PrefabName == deletedPrefabName);
                if (panelInfo == null) continue;
                
                RemoveType(panelInfo);
                isSomeChanged = true;
            }

            for (int i = 0; i < movedAssets.Length; i++)
            {
                if (IsPanelAsset(movedAssets[i]) || IsPanelAsset(movedFromAssetPaths[i]))
                {
                    ReplaceType(movedFromAssetPaths[i], movedAssets[i]);
                    isSomeChanged = true;
                    break;
                }
            }

            if (isSomeChanged)
            {
                string fullProjPath = Path.Combine(Directory.GetCurrentDirectory(), OverridenScriptPath);
                WriteToFile(s_uiPanelTypes, fullProjPath);
            }
        }

        private static bool IsPanelAsset(string assetPath)
        {
            if (!assetPath.StartsWith(PANEL_PATHES, true, CultureInfo.InvariantCulture)) return false;
            if (!assetPath.EndsWith(".prefab")) return false;
            
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            return asset != null && asset.GetComponent<UIPanel>() != null;
        }

        private static bool IsUiPanelTypeAsset(string assetPath)
        {
            var assetName = Path.GetFileName(assetPath);
            if (assetName == $"{TARGET_SCRIPT_NAME}.cs")
            {
                return true;
            }

            return false;
        }

        private static string GetPanelTypeNameByAssetPath(string assetPath)
        {
            var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            var uiPanel = gameObject.GetComponent<UIPanel>();
            var typeName = uiPanel.GetType().FullName;
            return typeName;
        }

        private static string GetPrefabNameByAssetPath(string assetPath)
        {
            if (assetPath.EndsWith(".prefab"))
            {
                assetPath = assetPath.Substring(0, assetPath.Length - ".prefab".Length);
            }

            var parts = assetPath.Split('/');
            var res = parts[parts.Length - 1];
            res = res.Replace(" ", "_");
            
            return res;
        }
        
        private static bool AddType(string assetPath)
        {
            var name = GetPanelTypeNameByAssetPath(assetPath);
            if (UiPanelTypes.Find(p => p.TypeName == name) != null) return false;

            var panelInfo = PanelInfo.CreateFromAssetPath(assetPath);
            UiPanelTypes.Add(panelInfo);
            AddressablePanels.AddToAddressables(panelInfo.Guid);
           
            return true;
        }

        private static void RemoveType(PanelInfo panelInfo)
        {
            UiPanelTypes.Remove(panelInfo);
            AddressablePanels.RemoveFromAddressables(panelInfo.Guid);
        }

        private static void ReplaceType(string assetPath, string newAssetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(newAssetPath);

            var oldPrefabName = GetPrefabNameByAssetPath(assetPath);
            var newPrefabName = GetPrefabNameByAssetPath(newAssetPath);

            var target = UiPanelTypes.Find(up => up.PrefabName == oldPrefabName);
            if (target == null) return;
            
            // Can be null if moved from not Panels folder
            /*if (target == null)
            {
                AddType(assetPath);
                return;
            }*/
            
            Assert.IsTrue(target.Guid == guid,
                $"Not valid guid of '{oldPrefabName}'({target.Guid}) for rename to '{newPrefabName}'({guid})");
            target.TypeName = GetPanelTypeNameByAssetPath(newAssetPath);
            target.PrefabName = GetPrefabNameByAssetPath(newAssetPath);
        }

        public static void UpdateUiPanels()
        {
            RefreshPanelTypes();
        }

        [MenuItem("Tools/RefreshUiPanelTypes")]
        private static void RefreshPanelTypes()
        {
            var panelPrefabs = FindAllPanelPrefabPathes();
            if (panelPrefabs.Length == 0)
            {
                ParseUiPanelTypes();
            }
            else
            {
                OnPostprocessAllAssets(panelPrefabs, new string[0] { }, new string[0] { }, new string[0] { });
            }
        }

        private static void ParseUiPanelTypes()
        {
            var targs = AssetDatabase.FindAssets($"{TARGET_SCRIPT_NAME} t:script");
            List<string> targScriptsPathes =
                targs.Select(AssetDatabase.GUIDToAssetPath).Where(p => p.EndsWith($"/{TARGET_SCRIPT_NAME}.cs")).ToList();
            var inProjectFilePath = targScriptsPathes.Find(ts => ts.StartsWith("Assets"));
            var inPackageFile = targScriptsPathes.Find(ts => ts.StartsWith("Packages"));
            if (inPackageFile == null)
            {
                s_uiPanelTypesPackage = new List<PanelInfo>();
                // Mean we not package
            }
            else
            {
                s_uiPanelTypesPackage = ParseScript(inPackageFile);
            }

            if (inProjectFilePath == null)
            {
                OverridenScriptPath = s_targetScriptPath;
                inProjectFilePath = OverridenScriptPath;

                if (!Directory.Exists(s_targetScriptFolder)) Directory.CreateDirectory(s_targetScriptFolder);
                AssetDatabase.CreateAsset(new MonoScript(), inProjectFilePath);

                s_uiPanelTypes = new List<PanelInfo>();
            }
            else
            {
                OverridenScriptPath = inProjectFilePath;
                s_uiPanelTypes = ParseScript(inProjectFilePath);
            }

            string fullProjPath = Path.Combine(Directory.GetCurrentDirectory(), OverridenScriptPath);
            WriteToFile(s_uiPanelTypes, fullProjPath);

            ApplyDefine();
        }

        private static void ApplyDefine()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(target);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var defs = defines.Split(';').ToList();
            defines = string.Join(";", defs.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
        }

        private static void WriteToFile(List<PanelInfo> uiPanelTypes, string filePath, bool isPackageProject = false)
        {
            File.Delete(filePath);
            var usedNames = new List<string>();
            
            using var str = File.CreateText(filePath);
            str.WriteLine("using System.Collections.Generic;");
            str.WriteLine("using Dev.Core.Ui.UI.Panels;");
            str.WriteLine();
            
            if (isPackageProject)
            {
                str.WriteLine($"namespace {TARGET_PACKAGE_NAME}");
                str.WriteLine("{");
            }
            else
            {
                str.WriteLine("    [UIPanelsGuidProviderAttribute]");
            }
            
            str.WriteLine("    public static class UIPanels");
            str.WriteLine("    {");
            str.WriteLine("        public static readonly Dictionary<string, string> Panels = new Dictionary<string, string>");
            str.WriteLine("        {");

            for (var index = 0; index < uiPanelTypes.Count; index++)
            {
                var info = uiPanelTypes[index];
                if (usedNames.Contains(info.TypeName)) continue;
                usedNames.Add(info.TypeName);

                var value = $"             {info.ToFileConstString()}";
                if (index < uiPanelTypes.Count - 1) value += ",";
                str.WriteLine(value);
            }

            str.WriteLine("        };");
            str.WriteLine("    }");

            if (isPackageProject)
            {
                str.WriteLine("}");
            }

            //CreateToFile(); 
        }
        
        private static void CreateToFile()
        {
            string fullProjPath = Path.Combine(Directory.GetCurrentDirectory(), s_targetDefinitionPath);

            using var str = File.CreateText(fullProjPath);
            str.WriteLine("{");
            str.WriteLine("    \"name\": \"Modules.UiPanels\",");
            str.WriteLine("    \"rootNamespace\": \"\",");
            str.WriteLine("    \"references\": [");
            str.WriteLine("        \"GUID:889c64b6965f64493a61806b50a52664\"");
            str.WriteLine("    ],");
            str.WriteLine("    \"includePlatforms\": [");
            str.WriteLine("    ],");
            str.WriteLine("    \"excludePlatforms\": [],");
            str.WriteLine("    \"allowUnsafeCode\": false, ");
            str.WriteLine("    \"overrideReferences\": false, ");
            str.WriteLine("    \"precompiledReferences\": [], ");
            str.WriteLine("    \"autoReferenced\": true, ");
            str.WriteLine("    \"defineConstraints\": [],");
            str.WriteLine("    \"versionDefines\": [],");
            str.WriteLine("    \"noEngineReferences\": false");
            str.WriteLine("}");
        }

        private static List<PanelInfo> ParseScript(string scriptPath)
        {
            var uiPanelTypes = new List<PanelInfo>();
            var fullProjPath = Path.Combine(Directory.GetCurrentDirectory(), scriptPath);
            var lines = File.ReadLines(fullProjPath).ToList();

            const string constKey = "public static readonly Dictionary<string, string> Panels";
            var lineIndex = lines.FindIndex(str => str.Contains(constKey)) + 2;

            while (true)
            {
                var infoLine = lines[lineIndex].Trim();
                lineIndex++;
                
                if (infoLine.StartsWith("{\""))
                {
                   var item = ParsePanelLine(infoLine);
                   //if (!uiPanelTypes.Exists(panelInfo => panelInfo.TypeName == item.TypeName)) uiPanelTypes.Add(item);
                }
                else
                {
                    break;
                }
            }

            return uiPanelTypes;
        }

        private static PanelInfo ParsePanelLine(string infoLine)
        {
            if (infoLine.EndsWith(",")) infoLine = infoLine.Remove(infoLine.Length - 1);
            infoLine = infoLine.Replace("{", "").Replace("}", "")
                .Replace("\"", "").Replace(" ", "");

            var values = infoLine.Split(',');
            var type = values[0];
            var guid = values[1];

            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefabName = GetPrefabNameByAssetPath(path);
            
            var res = new PanelInfo
            {
                TypeName = type,
                PrefabName = prefabName,
                Guid = guid
            };

            return res;
        }
    }
}