using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ModpackAddonDatasItem

{
    VisualElement m_root;
    AddonFileInfos addonInfos;
    public VisualElement ico_addon;
    public Label lbl_addonName, lbl_addonID;
    public Label lbl_authorNick;
    public VisualElement ico_addonType;
    public Toggle tgl_activeState;

    Action<long,bool> OnAddonStateChange = null;
    void Settup()
    {
        ico_addon = m_root.Query("ico_addon");
        lbl_addonName = m_root.Query<Label>("lbl_addonName");
        lbl_addonID = m_root.Query<Label>("lbl_addonID");
        lbl_authorNick = m_root.Query<Label>("lbl_authorNick");
        ico_addonType = m_root.Query("ico_addonType");
        tgl_activeState = m_root.Query<Toggle>("tgl_activeState");
        SettupEventsAndContents();
    }

    void SettupEventsAndContents()
    {
        lbl_addonName.text = addonInfos.addonName;
        lbl_addonID.text = addonInfos.fileName;
        lbl_authorNick.text = addonInfos.addonAuthor;

        tgl_activeState.RegisterCallback<ChangeEvent<bool>>(OnModPackAddonActiveStateChanged);
        tgl_activeState.SetValueWithoutNotify(addonInfos.enabled);
    }

    private void OnModPackAddonActiveStateChanged(ChangeEvent<bool> evt)
    {
        Debug.Log($"Addon :{addonInfos.addonName} setting to {evt.newValue}");
        OnAddonStateChange?.Invoke(addonInfos.cf_fileInfos.fileID,evt.newValue);
        
        here
        //continuar criando evento 'update' que renova o estado do addon ao confirmar ou nao mudancade estado
        //e criar o alerta de dependencias.
    }

    public ModpackAddonDatasItem(VisualElement _root, AddonFileInfos _addonInfos,Action<long, bool> _OnAddonStateChanged)
    {
        m_root = _root;
        addonInfos = _addonInfos;
        OnAddonStateChange = _OnAddonStateChanged;
        Settup();
    }
}
