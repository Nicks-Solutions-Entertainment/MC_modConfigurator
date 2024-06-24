using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class ScreenManager : MonoBehaviour
{

    [SerializeField] UIDocument documnnent;
    [SerializeField] VisualTreeAsset m_modpackItem;
    [SerializeField] VisualTreeAsset m_modpackAddonTableItem;
    //[SerializeField] UIDocument m_modpack;
    VisualElement doc_root;
    InitialSettupPage page_initialSettup;
    ContentPage page_modpackList;
    ContentPage page_modpackDetails;
    ProfileAddonsListPage page_addonsList;

    ModpackSettupPage page_modpackSettup;


    // Start is called before the first frame update
    void Start()
    {
        documnnent = GetComponent<UIDocument>();
        LoadElements();
        AppManager.Instance.OnStartApp += OnStartApp;
        AppManager.Instance.StartApp();
    }

    private void OnStartApp()
    {
        if (!ApplicationData.HasInitialSettup)
            GotoInitialSetup();
        else
            GoToHome();
    }

    void LoadElements()
    {
        doc_root = documnnent.rootVisualElement;
        page_modpackList = new ContentPage(doc_root.Query("page_library"), this);
        page_modpackDetails = new ContentPage(doc_root.Query("page_modpackDetail"), this);
        page_modpackSettup = new ModpackSettupPage(doc_root.Query("page_modepackSettup"), this);
        page_initialSettup = new InitialSettupPage(doc_root.Query("page_initialSetup"), this);
        
        page_addonsList = new ProfileAddonsListPage(doc_root.Query("page_addonsList"), this);
        page_addonsList.addonItemTreeAsset = m_modpackAddonTableItem;
    }


    //usar packageType para definir o tipo de addon
    //
    private void ClearScreens()
    {
        page_initialSettup.visible =
        page_modpackList.visible =
        page_modpackDetails.visible =
        page_modpackSettup.visible =
            false;
    }

    private void GotoInitialSetup()
    {
        ClearScreens();
        page_initialSettup.visible = true;
    }

    public void GoToHome()
    {
        //StartCoroutine(_YieldLoadHome());
        ClearScreens();
        page_modpackList.visible = true;
        AddElementsHandlingGapBetweenThem();
    }
    IEnumerator _YieldLoadHome()
    {
        ClearScreens();
        yield return new WaitForEndOfFrame();
        page_modpackList.visible = true;
        {
            //TemplateContainer _lastElement = null;
            //string[] _instances = CourseForgeConnector.GetInstancesOnFolder(ApplicationData.instancesPath);
            //for (int i = 0; i < _instances.Length; i++)
            foreach (var _instance in AppManager.RunetimeInstances)
            {
                TemplateContainer _item = m_modpackItem.CloneTree();
                _item.name = "ModpackItem";
                var _modpackItem = new ModpackItem(_item, _instance.Value, OnModpackItemClicked);
                //_modpackItem.SetEvents();

                page_modpackList.AddContent(_item);
                //if(i+1 < _instances.Length)
                AddGapOnContentToNext(_item, 24);
                yield return new WaitForSeconds(.2f);

                //_lastElement = _item;
            }
        }
    }

    public void GoToModpackDetails(ModpackSettings modpackSettings)
    {
        ClearScreens();
        Debug.Log($"GoToModpackDetails :: {modpackSettings.name}");
        page_modpackDetails.visible = true;

    }


    private void GoToSettupModPack(ModpackSettings modpackSettings)
    {
        ClearScreens();
        page_modpackSettup.modpackSettings = modpackSettings;
        page_modpackSettup.visible = true;
        Debug.Log($"GoToSettupModPack :: {modpackSettings.name}");

    }


    public void GoToModpackAddonList(ModpackSettings modpackSettings)
    {
        ClearScreens();
        Debug.Log($"GoToModpackAddonList :: ");
        page_addonsList.visible = true;
        page_addonsList.SetAddons(modpackSettings.addons,modpackSettings.OnAddonStateChanged);
    }

    private void OnModpackItemClicked(ModpackItem item)
    {
        if (AppManager.Instance.TryGetModPacksettings(item.instanceInfos.installPath, out ModpackSettings _settings))
            GoToModpackDetails(_settings);
        else
        {
            _settings = AppManager.Instance.CreateAndGetModPackSettings(item.instanceInfos);
            GoToSettupModPack(_settings);
        }
    }


    void AddElementsHandlingGapBetweenThem()
    {
        //TemplateContainer _lastElement = null;
        string[] _instances = CourseForgeConnector.GetInstancesOnFolder(ApplicationData.instancesPath);
        //for (int i = 0; i < _instances.Length; i++)
        foreach (var _instance in AppManager.RunetimeInstances)
        {
            TemplateContainer _item = m_modpackItem.CloneTree();
            _item.name = "ModpackItem";
            var _modpackItem = new ModpackItem(_item, _instance.Value, OnModpackItemClicked);
            //_modpackItem.SetEvents();

            page_modpackList.AddContent(_item);
            //if(i+1 < _instances.Length)
            AddGapOnContentToNext(_item, 24);
            //_lastElement = _item;
        }
    }


    void AddGapOnContentToNext(TemplateContainer elementToAddGap, int gap)
    {
        if (elementToAddGap != null)
        {
            elementToAddGap.style.marginRight = gap;
            elementToAddGap.style.left = gap;
            elementToAddGap.style.marginTop = gap;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }



}
