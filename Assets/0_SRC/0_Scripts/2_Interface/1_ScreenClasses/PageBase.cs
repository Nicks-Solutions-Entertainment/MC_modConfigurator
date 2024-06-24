using UnityEngine;
using UnityEngine.UIElements;

public abstract class PageBase
{
    public VisualElement root;
    protected ScreenManager screenMng;
    public bool visible
    {
        get => root.visible;
        set
        {
            //root.visible = value;
            if (value)
            {
                root.RemoveFromClassList("disabledElement");
                OnEnable();
                Debug.Log($"SETTING {value} on {root.name}");
            }
            else
            {
                if (!root.ClassListContains("disabledElement"))
                    root.AddToClassList("disabledElement");
                OnDisable();
            }
        }
    }
    public PageBase(VisualElement _rootElement, ScreenManager _screenManager)
    {
        root = _rootElement;
        screenMng = _screenManager;
        //Debug.Log($"root is null (base(_rootElement)) {root.IsUnityNull()}");

        FindAndSetScreenElements();
        OnStartPage();
    }
    protected abstract void FindAndSetScreenElements();

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }


    protected abstract void OnStartPage();
}

public static class UIElementUtils
{
    public static bool HadValidClickOnEvent<T>(this VisualElement visualElement, PointerEventBase<T> pointerEvent)
     where T : PointerEventBase<T>, new()
    {
        return pointerEvent.target.HasPointerCapture(pointerEvent.pointerId) && visualElement.ContainsPoint(pointerEvent.localPosition);
    }
}
