using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.Actions.MenuPriority;

[Serializable]
public class ProfileAddonsListPage : ContentPage
{
    public VisualTreeAsset addonItemTreeAsset;
    public List<ModpackAddonDatasItem> addonItem = new List<ModpackAddonDatasItem>();
    //public ListView table_contents;

    public ProfileAddonsListPage(VisualElement _rootElement, ScreenManager _screenManager) : base(_rootElement, _screenManager)
    {
    }


    protected override void OnStartPage()
    {
        
    }

    public void SetAddons(List<AddonFileInfos> addons,Action<long, bool> OnAddonStateChange)
    {
        //List<VisualElement> _items = new List<VisualElement>();
        string _DBG = $"SetAddons:: \n[\n";
        //ClearContents();
        foreach (var addon in addons)
        {
            TemplateContainer _addonItem = addonItemTreeAsset.CloneTree();
            _addonItem.name = "addon_item";
            addonItem.Add(new ModpackAddonDatasItem(_addonItem,addon, OnAddonStateChange));
            _DBG += $"{addon.addonName},\n";

            AddContent(_addonItem);
        }
            _DBG += $"]\n";

        //Debug.Log(_DBG);

    }
}
