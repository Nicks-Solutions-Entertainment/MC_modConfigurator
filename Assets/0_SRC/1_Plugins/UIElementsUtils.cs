using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIElementsUtils
{
    public static void SetVisible(this VisualElement visualElement, bool value)
    {
        if (value)
            visualElement.RemoveFromClassList("disabledElement");
        else
            visualElement.AddToClassList("disabledElement");

    }
}
