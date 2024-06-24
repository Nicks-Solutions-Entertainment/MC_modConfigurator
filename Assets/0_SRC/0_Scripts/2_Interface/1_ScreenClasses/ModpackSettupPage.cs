using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ModpackSettupPage : PageBase
{
    public ModpackSettupPage(VisualElement _rootElement, ScreenManager _screenManager) : base(_rootElement, _screenManager)
    {
    }

    public TextField txt_fld_backgroundImage;
    public TextField txt_fld_modpackRichTittle;
    public Label lbl_modpackTittleExemple;
    public Button btn_submit;

    public ModpackSettings modpackSettings = new ModpackSettings();

    protected override void FindAndSetScreenElements()
    {
        txt_fld_backgroundImage = root.Query<TextField>("txt_fld_backgroundImage");
        txt_fld_modpackRichTittle = root.Query<TextField>("txt_fld_modpackRichTittle");
        lbl_modpackTittleExemple = root.Query<Label>("lbl_modpackTittleExemple");
        btn_submit = root.Query<Button>("btn_submit");
    }

    protected override void OnStartPage()
    {
        
        SetInterfaceElementEvents();
    }
    void SetInterfaceElementEvents()
    {
        txt_fld_modpackRichTittle.RegisterValueChangedCallback(OnModpackTittleChanged);
        btn_submit.RegisterCallback<ClickEvent>(OnSubmitClicked);
    }


    private void OnModpackTittleChanged(ChangeEvent<string> evt)
    {
        lbl_modpackTittleExemple.text = evt.newValue;

    }

    private void OnSubmitClicked(ClickEvent evt)
    {
        modpackSettings.ApplyRichName(txt_fld_modpackRichTittle.text);
        Debug.Log($"{JsonUtility.ToJson(modpackSettings, true)}");
        screenMng.GoToModpackAddonList(modpackSettings);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        txt_fld_modpackRichTittle.SetValueWithoutNotify(modpackSettings.richName);
        txt_fld_backgroundImage.SetValueWithoutNotify("");
        lbl_modpackTittleExemple.text = txt_fld_modpackRichTittle.text;
    }
}
