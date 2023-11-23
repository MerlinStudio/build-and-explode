using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace Dev.Core.Ui.Editor
{
    public static class AddressablePanels
    {
        private const string ADDRESSABLE_GROUP_NAME = "UIPanels";
        private static AddressableAssetSettings s_addressableAssetSettings;

        private static AddressableAssetSettings AddressableAssetSettings
        {
            get
            {
                if (s_addressableAssetSettings == null)
                    s_addressableAssetSettings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                return s_addressableAssetSettings;
            }
        }

        private static AddressableAssetGroup CreateNewGroup(string groupName)
        {
            var schemas = new List<AddressableAssetGroupSchema>
            {
                ScriptableObject.CreateInstance<BundledAssetGroupSchema>(),
                ScriptableObject.CreateInstance<ContentUpdateGroupSchema>()
            };

            return AddressableAssetSettings.CreateGroup(groupName, false, false, false, schemas);
        }

        private static AddressableAssetGroup Group
        {
            get
            {
                var group = AddressableAssetSettings.FindGroup(ADDRESSABLE_GROUP_NAME);
                if (group == null) group = CreateNewGroup(ADDRESSABLE_GROUP_NAME);

                return group;
            }
        }

        public static void AddToAddressables(string guid)
        {
            if (Group.GetAssetEntry(guid) != null) return;

            AddressableAssetSettings.CreateOrMoveEntry(guid, Group);
        }

        public static void RemoveFromAddressables(string guid)
        {
            var entry = Group.GetAssetEntry(guid);
            if (entry == null) return;

            Group.RemoveAssetEntry(entry);
        }
    }
}