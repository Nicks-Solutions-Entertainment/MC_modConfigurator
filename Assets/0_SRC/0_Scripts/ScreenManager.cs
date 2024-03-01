using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ScreenManager : MonoBehaviour
{

    [SerializeField] UIDocument documnnent;
    [SerializeField] VisualTreeAsset m_modpackItem;
    //[SerializeField] UIDocument m_modpack;
    VisualElement doc_root;
    InitialSettupPage page_initialSettup;
    ContentPage page_modpackList;
    ContentPage page_modpackDetails;

    // Start is called before the first frame update
    void Start()
    {
        documnnent = GetComponent<UIDocument>();
        LoadElements();
        StartApp();
    }

    void LoadElements()
    {
        doc_root = documnnent.rootVisualElement;
        page_modpackList = new ContentPage(doc_root.Query("page_library"),this);
        page_modpackDetails = new ContentPage(doc_root.Query("page_modpackDetail"), this);
        page_initialSettup = new InitialSettupPage(doc_root.Query("page_initialSetup"), this);
    }

    void StartApp()
    {
        //if (!ApplicationData.HasInitialSettup)
            GotoInitialSetup();
        //else
            //GoToHome();
    }


    private void ClearScreens()
    {
        page_initialSettup.visible = 
        page_modpackList.visible = 
        page_modpackDetails.visible = 
            false;

    }
    private void GotoInitialSetup()
    {
        ClearScreens();
        page_initialSettup.visible = true;
    }

    void GoToHome()
    {
        ClearScreens();
        page_modpackList.visible = true;
        AddElementsHandlingGapBetweenThem();
    }

    void AddElementsHandlingGapBetweenThem()
    {
        //TemplateContainer _lastElement = null;
        for (int i = 0; i < 7; i++)
        {
            TemplateContainer _item = m_modpackItem.CloneTree();
            _item.name = "ModpackItem";
            page_modpackList.AddContent(_item);
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

    void GoToModpackDetails()
    {
        page_modpackList.visible = false;
        page_modpackDetails.visible = true;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public abstract class PageBase
    {
        public VisualElement root;
        protected ScreenManager screenMng;
        public bool visible
        {
            get => root.visible;
            set
            {
                root.visible = value;
                if (value)
                    OnEnable();
                else
                    OnDisable();
            }
        }
        public PageBase(VisualElement _rootElement,ScreenManager _screenManager)
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

    public class ContentPage : PageBase
    {
        public ScrollView scrollElements;

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
            scrollElements.Add(content);
        }

        protected override void OnEnable()
        {
            ClearContents();
        }

        public void ClearContents() => scrollElements.Clear();

    }

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
            Debug.Log($"btn_submit size:{btn_submit.contentRect.width} | {btn_submit.contentRect.height}");
            List<RaycastResult> _result = new List<RaycastResult>() ;
            Debug.Log($"HasPointerCapture:{evt.target.HasPointerCapture(evt.pointerId)} \n" +
                $"IsPointerLocalPositioninsideUIElement:{IsPointerLocalPositioninsideUIElement(evt,btn_submit)}" 
                );
            //evt.target.HasPointerCapture(evt.pointerId) <- se o clique comecou no elemento em questao

            if (EventSystem.current.IsPointerOverGameObject(evt.pointerId))
                OnSubmit();

            
        }

        bool IsPointerLocalPositioninsideUIElement<T>(PointerEventBase<T> pointerEvent,VisualElement visualElement)
         where T : PointerEventBase<T>,new()
        {
            Vector3 absPosition = new Vector3(
                Mathf.Abs(pointerEvent.localPosition.x),
                Mathf.Abs(pointerEvent.localPosition.y)
                );
            bool isinside_on_X = absPosition.x <= visualElement.contentRect.width;
            bool isinside_on_Y = absPosition.y <= visualElement.contentRect.height;
            //bool isinside_on_Y 
            return isinside_on_X && isinside_on_Y;
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
}
