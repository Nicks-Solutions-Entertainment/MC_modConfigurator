
using UnityEngine;
using UnityEngine.UIElements;

public class InitialSettupPage : PageBase
{
    public TextField txt_fld_InstacePath;
    public Button btn_submit;

    public InitialSettupPage(VisualElement _rootElement, ScreenManager _screenManager) : base(_rootElement, _screenManager)
    {
    }

    protected override void FindAndSetScreenElements()
    {
        //Debug.Log($"root is null {root.IsUnityNull()}");
        txt_fld_InstacePath = root.Q<TextField>("txt_fld-instancePath");
        btn_submit = root.Q<Button>("btn_submit");
    }
    protected override void OnStartPage()
    {
        SetInterfaceElementEvents();
    }

    void SetInterfaceElementEvents()
    {
        //btn_submit.clicked += OnSubmit;
        btn_submit.RegisterCallback<PointerUpEvent>(OnSubmitClicked);
        txt_fld_InstacePath.RegisterValueChangedCallback(OnInstancesPathChanged);
    }

    private void OnSubmitClicked(PointerUpEvent evt)
    {
        //Debug.Log($"btn_submit size:{btn_submit.contentRect.width} | {btn_submit.contentRect.height}");
        //List<RaycastResult> _result = new List<RaycastResult>();
        //Debug.Log($"HasPointerCapture:{evt.target.HasPointerCapture(evt.pointerId)} \n" +
        //    $"ContainsPoint(evt.localPosition) :{btn_submit.ContainsPoint(evt.localPosition)}\n" +
        //    $"IsPointerLocalPositioninsideUIElement:{IsPointerValidOnVisualElement(evt, btn_submit)}\n"
        //    );
        //evt.target.HasPointerCapture(evt.pointerId) <- se o clique comecou no elemento em questao
        if (IsPointerValidOnVisualElement(evt, btn_submit))
            OnSubmit();


    }
    /// <summary>
    /// Valida o POinter Event verificando se o evento (click_down, por exemplo) foi no elemento em questao
    /// e se ao soltar o mesmo foi dentro da area esperada do mesmo elemento. <br></br>
    /// isso impossibilita um clique onde o usuario arrastou o ponteiro para fora do elemento e soltou seja considerado, da mesma forma que um clique que inicia-se
    /// fora do elemento e saiu do mesmo considere.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pointerEvent">O Evento a ser checado</param>
    /// <param name="visualElement">O VisualElement (pode sr qualquer UIElement, basicamente) que devera checar sua area.</param>
    /// <returns></returns>
    bool IsPointerValidOnVisualElement<T>(PointerEventBase<T> pointerEvent, VisualElement visualElement)
     where T : PointerEventBase<T>, new()
    {
        return pointerEvent.target.HasPointerCapture(pointerEvent.pointerId) && visualElement.ContainsPoint(pointerEvent.localPosition);
    }

    private void OnInstancesPathChanged(ChangeEvent<string> evt)
    {
        ApplicationData.instancesPath = evt.newValue;
    }

    void OnSubmit()
    {
        Debug.Log($"InitialSettupPage.OnSubmit");
        screenMng.GoToHome();
    }
    protected override void OnEnable()
    {
        txt_fld_InstacePath.value = ApplicationData.instancesPath;
    }
}