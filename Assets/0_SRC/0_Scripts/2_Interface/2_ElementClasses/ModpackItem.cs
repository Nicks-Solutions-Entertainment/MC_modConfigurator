using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class ModpackItem
{
    //public string instancePath;
    public VisualElement root;
    public VisualElement img_thumb;
    public VisualElement ve_loadderInfo;
    public VisualElement img_loadderIcon;
    public Label lbl_mcVersion;
    public Label lbl_modpackVersion;
    public Label lbl_name;
    public Label lbl_author;
    public Button btn_play;
    public Button btn_notConfigured;

    Action<ModpackItem> OnOpenModpack = null;
    //public ModpackSettings modpackSettings = null;
    public bool hasSettup = false;
    public CF_RunetimeProfileInfos instanceInfos { get; private set; } = null;
    public ModpackItem(VisualElement _root, CF_RunetimeProfileInfos _runetimeInfos, Action<ModpackItem> OnClickEnvent)
    {
        instanceInfos = _runetimeInfos;
        //instancePath = _instancePath;
        root = _root;
        img_thumb = root.Q("img_thumb");
        lbl_name = root.Q<Label>("lbl_name");
        lbl_author = root.Q<Label>("lbl_author");

        btn_play = root.Q<Button>("btn_play");
        btn_notConfigured = root.Q<Button>("btn_notConfigured");

        ve_loadderInfo = root.Q("ve_loadderInfo");
        img_loadderIcon = root.Q("img_loadderIcon");
        lbl_mcVersion = root.Q<Label>("lbl_mcVersion");
        lbl_modpackVersion = root.Q<Label>("lbl_modpackVersion");
        
        OnOpenModpack = OnClickEnvent;
        hasSettup = !instanceInfos.IsUnityNull();
        StartItem();
    }
     
    private void StartItem()
    {
        //instanceInfos = await CourseForgeConnector.GetRunetimeInstanceInfos(instancePath);
        btn_play.SetVisible(false);
        btn_notConfigured.SetVisible(!hasSettup);
        ve_loadderInfo.SetVisible(hasSettup);

        lbl_name.text = instanceInfos.name;
        lbl_author.text = instanceInfos.manifest.HasAuthorRegistered()? 
            instanceInfos.manifest.author 
            : 
            "nao configurado";
        SetEvents();

    }

    public void SetEvents()
    {
        //root.RegisterCallback<PointerUpEvent>(OnModpackItemPointerUp);
        root.RegisterCallback<ClickEvent>(OnModpackItemClicked);
        root.RegisterCallback<PointerEnterEvent>(OnHoverEnter);
        root.RegisterCallback<PointerLeaveEvent>(OnHoverExit);

    }


    private void OnModpackItemClicked(ClickEvent evt)
    {
        //GetInstanceInfos();
        //if (!AppManager.Instance.TryGetModPacksettings(instancePath, out modpackSettings))
        //{
        //    modpackSettings = AppManager.Instance.CreateAndGetModPackInfos(instanceInfos);
        //}    
        OnOpenModpack?.Invoke(this);
        //root.RemoveFromHierarchy();
    }

    private void OnHoverEnter(PointerEnterEvent evt)
    {
        btn_play.SetVisible(hasSettup);
        btn_notConfigured.text = "CONFIGURAR";
        //btn_notConfigured.SetVisible(!hasSettup);
        ve_loadderInfo.SetVisible(false);

    }
    private void OnHoverExit(PointerLeaveEvent evt)
    {
        btn_play.SetVisible(false);
        btn_notConfigured.text = "NAO CONFIGURADO";
        //btn_notConfigured.SetVisible(false);
        //btn_notConfigured.SetVisible(false);
        ve_loadderInfo.SetVisible(true);

    }


    private void OnModpackItemPointerUp(PointerUpEvent evt)
    {
        Debug.Log($"OnModpackItemCliecked :: {instanceInfos.name} valid:{img_thumb.HadValidClickOnEvent(evt)}");
        
        if (img_thumb.HadValidClickOnEvent(evt))
        {
            
        }
    }

    
}