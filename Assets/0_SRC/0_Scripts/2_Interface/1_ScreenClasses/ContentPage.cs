using System.Collections.Generic;
using UnityEngine.UIElements;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class ContentPage : PageBase
{
    public ScrollView scrollElements;
    protected List<VisualElement> contents = new List<VisualElement>();

    public ContentPage(VisualElement _rootElement, ScreenManager _screenManager) : base(_rootElement, _screenManager)
    {
    }

    protected override void FindAndSetScreenElements()
    {
        scrollElements = root.Query<ScrollView>("scrollContent");


    }
    protected override void OnStartPage()
    {
        //scrollElements = root.Query<ScrollView>("scrollContent");
    }

    public void AddContent(VisualElement content)
    {
        contents.Add(content);
        scrollElements?.Add(content);
    }
    
    protected override void OnEnable()
    {
        ClearContents();
    }
    protected override void OnDisable()
    {
        ClearContents();
    }
    public void ClearContents()
    {
        foreach (var content in contents)
            content.RemoveFromHierarchy();

        scrollElements?.Clear();
    }

}
