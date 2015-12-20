using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

namespace UUI
{
    #region UUIExtras
    public enum UIType
    {
        Panel = 0, PopupMenu, Submenu, GameScreen,
        Scene = 10,
        Button = 20, Toggle, Slider, Dropdown, Textbox, Image,
        
    };

    public struct UnityRect
    {
        public UnityRect(Vector2 Center, Vector2 Size)
        {
            _Left = Center.x - Size.x / 2;
            _Right = Center.x + Size.x / 2;
            _Top = Center.y + Size.y / 2;
            _Bottom = Center.y - Size.y / 2;
        }

        public static UnityRect SetFromTopLeftAndBotRight (Vector2 TopLeft, Vector2 BotRight)
        {
            return new UnityRect(new Vector2((TopLeft.x + BotRight.x) / 2, (TopLeft.y + BotRight.y) / 2), new Vector2(BotRight.x - TopLeft.x, TopLeft.y - BotRight.y));
        }

        public static UnityRect SetFromTopLeft(Vector2 TopLeft, Vector2 Size)
        {
            return new UnityRect(new Vector2(TopLeft.x + Size.x / 2, TopLeft.y - Size.y / 2), Size);
        }

        private float _Top;
        public float Top
        {
            get { return _Top; }
            set
            {
                if (value >= _Bottom)
                    _Top = value;
                else
                {
                    _Top = _Bottom;
                    _Bottom = value;
                }
            }
        }
        private float _Bottom;
        public float Bottom
        {
            get { return _Bottom; }
            set
            {
                if (value <= _Top)
                    _Bottom = value;
                else
                {
                    _Bottom = _Top;
                    _Top = value;
                }
            }
        }
        private float _Right;
        public float Right
        {
            get { return _Right; }
            set
            {
                if (value >= _Left)
                    _Right = value;
                else
                {
                    _Right = _Left;
                    _Left = value;
                }
            }
        }
        private float _Left;
        public float Left
        {
            get { return _Left; }
            set
            {
                if (value <= _Right)
                    _Left = value;
                else
                {
                    _Left = _Right;
                    _Right = value;
                }
            }
        }

        public Vector2 Center
        {
            get { return new Vector2((Right + Left) / 2.0f, (Top + Bottom) / 2.0f); }
            set
            {
                Vector2 S = Size;
                _Left = value.x - S.x / 2;
                _Right = value.x + S.x / 2;
                _Top = value.y + S.y / 2;
                _Bottom = value.y - S.y / 2;
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(Right - Left, Top - Bottom);
            }

            set
            {
                Vector2 C = Center;
                _Left = C.x - value.x / 2;
                _Right = C.x + value.x / 2;
                _Top = C.y + value.y / 2;
                _Bottom = C.y - value.y / 2;
            }
        }

        //TopLeft
        public Vector2 TopLeft
        {
            get
            {
                return new Vector2(Left, Top);
            }
        }
        /// <summary>
        /// Sets the TL corner while keeping the Size the same
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public Vector2 SetTopLeft(Vector2 Val)
        {
            Vector2 S = Size;
            _Top = Val.y;
            _Left = Val.x;
            _Bottom = Val.y - S.y;
            _Right = Val.x + S.y;

            return TopLeft;
        }
        /// <summary>
        /// Moves the TL corner while keeping the BR corner still
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public Vector2 MoveTopLeft(Vector2 Val)
        {
            Top = Val.y;
            Left = Val.x;

            return TopLeft;
        }

        //Bottom Left
        public Vector2 BotLeft
        {
            get
            {
                return new Vector2(Left, Bottom);
            }
        }
        /// <summary>
        /// Sets the BL corner while keeping the Size the same
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public Vector2 SetBotLeft(Vector2 Val)
        {
            Vector2 S = Size;
            _Bottom = Val.y;
            _Left = Val.x;
            _Top = Val.y + S.y;
            _Right = Val.x + S.y;

            return BotLeft;
        }
        /// <summary>
        /// Moves the BL corner while keeping the TR still
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public Vector2 MoveBotLeft(Vector2 Val)
        {
            Bottom = Val.y;
            Left = Val.x;

            return BotLeft;
        }

        //Top Right
        public Vector2 TopRight
        {
            get
            {
                return new Vector2(Right, Top);
            }
        }
        /// <summary>
        /// Sets the TR corner while keeping the Size the same
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public Vector2 SetTopRight(Vector2 Val)
        {
            Vector2 S = Size;
            _Top = Val.y;
            _Right = Val.x;
            _Bottom = Val.y - S.y;
            _Left = Val.x - S.y;

            return TopRight;
        }

        /// <summary>
        /// Moves the TR corner while keeping the BL corner still
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public Vector2 MoveTopRight(Vector2 Val)
        {
            Top = Val.y;
            Right = Val.x;

            return TopLeft;
        }

        //Bottom Right
        public Vector2 BotRight
        {
            get
            {
                return new Vector2(Right, Bottom);
            }
        }
        /// <summary>
        /// Sets the BR corner while keeping Size the same
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public Vector2 SetBotRight(Vector2 Val)
        {
            Vector2 S = Size;
            _Bottom = Val.y;
            _Right = Val.x;
            _Top = Val.y + S.y;
            _Left = Val.x - S.y;

            return BotRight;
        }
        /// <summary>
        /// Moves the BR corner while keeping the TL still
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public Vector2 MoveBotRight(Vector2 Val)
        {
            Bottom = Val.y;
            Right = Val.x;

            return BotRight;
        }
    }

    public static class Helpers
    {
        public static Interactives.UIInteractive TagChainToInteractive(UIController Controller, string[] TagChain)
        {
            Containers.UIContainer CurContainer = Controller.GetSceneByTag(TagChain[0]);

            for (int i=1;i<TagChain.Length - 1;i++)
            {
                CurContainer = CurContainer.GetSubContainer(TagChain[i]);
            }
            return CurContainer.GetSubInteractive(TagChain[TagChain.Length - 1]);
        }

        public static Containers.UIContainer FollowTagChainToContainer(UIController Controller, string[] TagChain)
        {
            Containers.UIContainer CurContainer = Controller.GetSceneByTag(TagChain[0]);

            for (int i = 1; i < TagChain.Length; i++)
            {
                CurContainer = CurContainer.GetSubContainer(TagChain[i]);
            }
            return CurContainer;
        }

        
    }
    #endregion

    public abstract class UIElement
    {
        #region Variables 
        public string Tag { get; private set; }

        public GameObject ScreenElement { get; private set; }

        public Image Img { get; private set; }

        public Image BGImg { get; private set; }

        public Color BGColor { get; private set; }

        protected bool UsingSprite
        {
            get { return (BGImg != null); }
        }

        public int Priority { get; private set; }

        public Vector2 MinSize { get; protected set; }

        public Vector2 Location { get; private set; }

        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (value)
                    Show();
                else
                    Hide();
            }
        }
        protected bool _Visible = false;

        public UIController Controller { get; private set; }

        public UIElement Parent { get; private set; }

		public RectTransform GetTransform { get { return ScreenElement.GetComponent<RectTransform> (); } }

        public Vector2 RectSize { get { return ScreenElement.GetComponent<RectTransform>().rect.size; } }

        public UIType Type { get; private set; }

        public string[] TracePathFromContainer
        {
            get
            {
                if (_TracePathFromContainer.Length == 0)
                {
                    List<string> TracePath = new List<string>();
                    TracePath.Insert(0, Tag);
                    UIElement CurParent = Parent;
                    while (CurParent != null)
                    {
                        TracePath.Insert(0, CurParent.Tag);
                    }
                    _TracePathFromContainer = TracePath.ToArray();
                    return TracePath.ToArray();
                }
                else
                    return _TracePathFromContainer;
            }
        }

        private string[] _TracePathFromContainer = { };
        #endregion Variables

        #region Initialisers
        //Base Code all Initialisers implement
        private void BaseInitialism(string Tag, UIType Type, Vector2 MinSize, Vector2 Location, int Priority)
        {
            if (Tag.Contains("."))
                throw new System.Exception("UI Element or Container cannot contain the '.' character");
            this.Tag = Tag;
            this.Type = Type;
            this.MinSize = MinSize;
        }

        private void BaseInitialism(string Tag, UIType Type, Vector2 MinSize, int Priority)
        {
            BaseInitialism(Tag, Type, MinSize, Vector2.zero, Priority);
        }

        private void BaseInitialism(string Tag, UIType Type, int Priority)
        {
            BaseInitialism(Tag, Type, Vector2.zero, Vector2.zero, Priority);
        }

        //Initialisers
        public UIElement(string Tag, UIType Type, Color BGColor, int Priority)
        {
            BaseInitialism(Tag, Type, Priority);
            this.BGColor = BGColor;
        }

        public UIElement(string Tag, UIType Type, Image BGImg, int Priority)
        {
            BaseInitialism(Tag, Type, Priority);
            this.BGImg = BGImg;
        }

        public UIElement (string Tag, UIType Type, Color BGColor, Vector2 MinSize, int Priority)
        {
            BaseInitialism(Tag, Type, MinSize, Priority);
            this.BGColor = BGColor;
        }

        public UIElement(string Tag, UIType Type, Image BGImg, Vector2 MinSize, int Priority)
        {
            BaseInitialism(Tag, Type, MinSize, Priority);
            this.BGImg = BGImg;
        }

        public UIElement(string Tag, UIType Type, Color BGColor, Vector2 MinSize, Vector2 Location, int Priority)
        {
            BaseInitialism(Tag, Type, MinSize, Location, Priority);
            this.BGColor = BGColor;
        }

        public UIElement(string Tag, UIType Type, Image BGImg, Vector2 MinSize, Vector2 Location, int Priority)
        {
            BaseInitialism(Tag, Type, MinSize, Location, Priority);
            this.BGImg = BGImg;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Creates and Establishes the Screen Element
        /// </summary>
        /// <param name="Parent">The Container Element directly above this UIElement</param>
        /// <param name="BotLeft">The Bottom Left Point of the Screen Element Relative to the Bottom Left of the Parent</param>
        /// <param name="TopRight">The Top Right Point of the Screen Element Relative to the Top Right of the Parent</param>
        protected void CreateGameObject(Transform Parent, Vector2 BotLeft, Vector2 TopRight)
        {
            switch (Type)
            {
                case UIType.GameScreen:

                    break;
                default:
                    ScreenElement = new GameObject(Type.ToString() + "." + Tag);
                    break;
            }
            ScreenElement.AddComponent<RectTransform>();
            ScreenElement.AddComponent<CanvasRenderer>();
            Img = ScreenElement.AddComponent<Image>();
            ScreenElement.AddComponent<Mask>();
            Img.color = BGColor;

            GetTransform.SetParent(Parent);

            GetTransform.anchorMin = Vector2.zero;
            GetTransform.anchorMax = Vector2.one;

            GetTransform.offsetMin = BotLeft;
            GetTransform.offsetMax = TopRight;
        }

        public virtual void Unload()
        {
            UnityEngine.Object.Destroy(ScreenElement);
        }

        /// <summary>
        /// Loads the UIElement
        /// </summary>
        /// <param name="Controller">The Master Controller for the whole UI</param>
        /// <param name="Parent">The Container Element directly above this UIElement</param>
        /// <param name="BotLeft">The Bottom Left Point of the Screen Element Relative to the Bottom Left of the Parent</param>
        /// <param name="TopRight">The Top Right Point of the Screen Element Relative to the Top Right of the Parent</param>
        public virtual void Load(UIController Controller, UIElement Parent, Vector2 BotLeft, Vector2 TopRight)
        {
            CreateGameObject(Parent.GetTransform.transform, BotLeft, TopRight);
            this.Controller = Controller;
            this.Parent = Parent;
        }

        /// <summary>
        /// Loads the UIElement without a Parent Element
        /// </summary>
        /// <param name="Controller">The Master Controller for the whole UI</param>
        /// <param name="BotLeft">The Bottom Left Point of the Screen Element Relative to the Bottom Left of the Parent</param>
        /// <param name="TopRight">The Top Right Point of the Screen Element Relative to the Top Right of the Parent</param>
        public virtual void Load(UIController Controller, Vector2 BotLeft, Vector2 TopRight)
        {
            CreateGameObject(Controller.GetTransform.transform, BotLeft, TopRight);
            this.Controller = Controller;
            Parent = null;
        }

        public virtual void Show()
        {
            _Visible = true;
            ScreenElement.GetComponent<CanvasRenderer>().SetAlpha(100.0f);
        }

        public virtual void Hide()
        {
            _Visible = false;
            ScreenElement.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        }

        public virtual void ToggleVisible()
        {
            if (Visible)
                Hide();
            else
                Show();
        }
        #endregion
    }

    public class UIController
    {
        #region Variables
        private List<Containers.UIScene> Scenes;
        public Canvas MainCanvas { get { return _Canvas; } }
        private Canvas _Canvas;
        public RectTransform GetTransform { get { return MainCanvas.GetComponent<RectTransform>(); } }

        private string CurrentUISceneTag = "";
        public Containers.UIScene CurrentScene { get { foreach (Containers.UIScene Scene in Scenes) if (Scene.Tag == CurrentUISceneTag) return Scene; return null; } }
        #endregion

        #region Initialisers
        public UIController (Canvas MainCanvas, Containers.UIScene BaseScene)
        {
            _Canvas = MainCanvas;
            Scenes = new List<Containers.UIScene>();
            Scenes.Add(BaseScene);
            CurrentUISceneTag = BaseScene.Tag;

            //MainCanvas.gameObject.AddComponent<Mask>();

            BaseScene.Load(this);
        }
        #endregion

        #region Functions
        public void AddUIScene(Containers.UIScene Scene)
        {
            Scenes.Add(Scene);
        }

        public void RemoveUIScene(string UISceneTag)
        {
            if (UISceneTag != CurrentUISceneTag)
            {
                foreach (Containers.UIScene Scene in Scenes)
                    if (Scene.Tag == UISceneTag)
                        Scenes.Remove(Scene);
            }
            else throw new System.Exception("Cannot remove current Scene from List");
        }
        
        public void ChangeUIScene(string UISceneTag)
        {
            if (CurrentScene != null)
            {
                if (CheckSceneTagValidity(UISceneTag))
                {
                    CurrentScene.Unload();
                    CurrentUISceneTag = UISceneTag;
                    CurrentScene.Load(this);
                }
                else throw new Exception("No matching Tag found for UIScene");
            }
            else throw new Exception("CurrentScene == null, something has gone seriously wrong. \n Contact Dev");
        }

        private bool CheckSceneTagValidity(string UISceneTag)
        {
            foreach (Containers.UIScene Scene in Scenes)
            {
                if (Scene.Tag == UISceneTag)
                    return true;
            }
            return false;
        }

        public Containers.UIScene GetSceneByTag(string Tag)
        {
            foreach (Containers.UIScene Scene in Scenes)
                if (Tag == Scene.Tag)
                    return Scene;
            throw new System.Exception("Container \"" + Tag + "\"not found");
        }
        #endregion
    }

    namespace Containers
    {
        public abstract class UIContainer : UIElement
        {
            #region Variables
            protected List<Interactives.UIInteractive> ContainedInteractives;
            protected List<Interactives.UIInteractive> SortedInteractives
            {
                get
                {
                    if (_SortedInteractives == null || _SortedInteractives.Count != ContainedInteractives.Count)
                    {
                        List<Interactives.UIInteractive> SortedInteractives = ContainedInteractives;
                        int Completed = 0;
                        while (Completed < SortedInteractives.Count)
                        {
                            for (int i = 0; i < SortedInteractives.Count - (Completed + 1); i++)
                            {
                                if (SortedInteractives[i].Priority > SortedInteractives[i + 1].Priority)
                                {
                                    Interactives.UIInteractive Temp = SortedInteractives[i];
                                    SortedInteractives[i] = SortedInteractives[i + 1];
                                    SortedInteractives[i + 1] = Temp;
                                }
                            }
                            Completed++;
                        }
                        _SortedInteractives = SortedInteractives;
                    }
                    return _SortedInteractives;
                }
            }
            private List<Interactives.UIInteractive> _SortedInteractives;

            protected List<UIContainer> ContainedContainers;
            protected List<UIContainer> SortedContainers
            {
                get
                {
                    if (_SortedContainers == null || _SortedContainers.Count != ContainedContainers.Count)
                    {
                        List<UIContainer> SortedContainers = ContainedContainers;
                        int Completed = 0;
                        while (Completed < SortedContainers.Count)
                        {
                            for (int i = 0; i < SortedContainers.Count - (Completed + 1); i++)
                            {
                                if (SortedContainers[i].Priority > SortedContainers[i + 1].Priority)
                                {
                                    UIContainer Temp = SortedContainers[i];
                                    SortedContainers[i] = SortedContainers[i + 1];
                                    SortedContainers[i + 1] = Temp;
                                }
                            }
                            Completed++;
                        }
                        _SortedContainers = SortedContainers;
                    }
                    return _SortedContainers;
                }
            }
            private List<UIContainer> _SortedContainers;

            protected List<UIElement> SortedElements
            {
                get
                {
                    if (_SortedElements == null || _SortedElements.Count != SortedContainers.Count + SortedInteractives.Count)
                    {
                        _SortedElements = new List<UIElement>();
                        foreach (Interactives.UIInteractive I in SortedInteractives)
                            _SortedElements.Add(I);
                        foreach (UIContainer C in SortedContainers)
                            _SortedElements.Add(C);
                    }
                    return _SortedElements;
                }
            }
            private List<UIElement> _SortedElements;

            #endregion

            #region Initialisers
            public UIContainer(string Tag, UIType Type, Color BGColor, int Priority) : this(Tag, Type, BGColor, Vector2.zero, Priority)
            { }

            public UIContainer(string Tag, UIType Type, Image BGImg, int Priority) : this(Tag, Type, BGImg, Vector2.zero, Priority)
            { }

            public UIContainer(string Tag, UIType Type, Color BGColor, Vector2 MinSize, int Priority) : this(Tag, Type, BGColor, MinSize, Vector2.zero, Priority)
            { }

            public UIContainer(string Tag, UIType Type, Image BGImg, Vector2 MinSize, int Priority) : this(Tag, Type, BGImg, MinSize, Vector2.zero, Priority)
            { }

            public UIContainer(string Tag, UIType Type, Color BGColor, Vector2 MinSize, Vector2 Location, int Priority) : base(Tag, Type, BGColor, MinSize, Location, Priority)
            {
                if ((int)Type / 20 == 0)
                {
                    ContainedInteractives = new List<Interactives.UIInteractive>();
                    ContainedContainers = new List<UIContainer>();
                }
                else throw new System.Exception("Wrong type used to create container, please choose a container type");
            }

            public UIContainer(string Tag, UIType Type, Image BGImg, Vector2 MinSize, Vector2 Location, int Priority) : base(Tag, Type, BGImg, MinSize, Location, Priority)
            {
                if ((int)Type / 20 == 0)
                {
                    ContainedInteractives = new List<Interactives.UIInteractive>();
                    ContainedContainers = new List<UIContainer>();
                }
                else throw new System.Exception("Wrong type used to create container, please choose a container type");
            }

            #endregion

            #region Functions

            public override void Load(UIController Controller, UIElement Parent, Vector2 BotLeft, Vector2 TopRight)
            {
                base.Load(Controller, Parent, BotLeft, TopRight);

                AutoPlaceSubElements();

            }

            public override void Load(UIController Controller, Vector2 BotLeft, Vector2 TopRight)
            {
                base.Load(Controller, BotLeft, TopRight);

                AutoPlaceSubElements();
            }

            public void addUIInteractive(Interactives.UIInteractive Element)
            {
                bool UnusedTag = true;
                foreach (Interactives.UIInteractive CheckingElement in ContainedInteractives)
                {
                    if (CheckingElement.Tag == Element.Tag)
                        UnusedTag = false;
                }
                if (UnusedTag)
                    ContainedInteractives.Add(Element);
                else
                    throw new System.Exception("Tag \"" + Element.Tag + "\" already used on an element in " + this.Tag + ".");
            }

            public void addUIInteractive(Interactives.UIInteractive[] Elements)
            {
                foreach (Interactives.UIInteractive Element in Elements)
                    addUIInteractive(Element);
            }

            public void addUIContainer(UIContainer Container)
            {
                bool UnusedTag = true;
                foreach (UIContainer CheckingContainer in ContainedContainers)
                {
                    if (CheckingContainer.Tag == Container.Tag)
                        UnusedTag = false;
                }

                if (UnusedTag)
                    ContainedContainers.Add(Container);
                else
                    throw new System.Exception("Tag \"" + Container.Tag + "\" already used on a container in " + this.Tag + ".");
            }

            public void addUIContainer(UIContainer[] Containers)
            {
                for (int i = 0; i < Containers.Length; i++)
                    addUIContainer(Containers[i]);
            }

            public bool removeUIInteractive(string Tag)
            {
                bool Removed = false;
                foreach (Interactives.UIInteractive CheckingElement in ContainedInteractives)
                {
                    if (CheckingElement.Tag == Tag)
                    {
                        ContainedInteractives.Remove(CheckingElement);
                        Removed = true;
                    }
                }
                return Removed;
            }

            public bool removeUIInteractive(string[] Tags)
            {
                bool Removed = true;
                foreach (string Tag in Tags)
                    if (!removeUIInteractive(Tag))
                        Removed = false;
                return Removed;
            }

            public bool removeUIContainer(string Tag)
            {
                bool Removed = false;
                foreach (UIContainer CheckingContainer in ContainedContainers)
                {
                    if (CheckingContainer.Tag == Tag)
                    {
                        ContainedContainers.Remove(CheckingContainer);
                        Removed = true;
                    }
                }
                return Removed;
            }

            public bool removeUIContainer(string[] Tags)
            {
                bool Removed = true;
                foreach (string Tag in Tags)
                    if (!removeUIContainer(Tag))
                        Removed = false;
                return Removed;
            }

            public virtual UIContainer GetSubContainer(string Tag)
            {
                foreach (UIContainer Container in ContainedContainers)
                    if (Container.Tag == Tag)
                        return Container;
                throw new System.Exception("Container \"" + Tag + "\"not found");
                //return null;
            }

            public Interactives.UIInteractive GetSubInteractive(string Tag)
            {
                foreach (Interactives.UIInteractive Interactive in ContainedInteractives)
                    if (Interactive.Tag == Tag)
                        return Interactive;
                throw new System.Exception("Container \"" + Tag + "\"not found");
                //return null;
            }

            public override void Show()
            {
                base.Show();
                foreach (UIContainer C in ContainedContainers)
                {
                    C.Show();
                }
                foreach (Interactives.UIInteractive E in ContainedInteractives)
                {
                    E.Show();
                }
            }

            public override void Hide()
            {
                base.Hide();
                foreach (UIContainer C in ContainedContainers)
                {
                    C.Hide();
                }
                foreach (Interactives.UIInteractive E in ContainedInteractives)
                {
                    E.Hide();
                }
            }

            private void AutoPlaceSubElements()
            {
                int NumElements = SortedElements.Count;
                Vector2 Spacing = new Vector2(0.5f, 0.5f);

                
                //Check Orientation
                if (RectSize.x <= RectSize.y)
                {
                    //Portait Orientation

                    //GetElementPositions
                    //LoadElements in their allotted positions
                    //Load Scrollbar if needed
                    

                }
                else
                {
                    //Landscape Orientation
                }
            }

            private static UnityRect[] GetElementPositions(UIElement[] Elements, Vector2 ContainerSize, Vector2 Spacing, bool Portrait)
            {
                UnityRect[] Positions = new UnityRect[Elements.Length];
                Vector2 CurrentCursor = new Vector2(Spacing.x, ContainerSize.y - Spacing.y);

                for (int i = 0; i < Elements.Length; i++)
                {
                    if (Elements[i].MinSize.x != 0)
                    {
                        int NumElementsOnRow = GetNumElementsFitPortrait(Elements, ContainerSize, i, CurrentCursor.x, Spacing.x);

                        for (int j = i; j < i + NumElementsOnRow; j++)
                        {

                        }

                        i += NumElementsOnRow;
                        Positions[i] = UnityRect.SetFromTopLeft(CurrentCursor, Elements[i].MinSize);
                    }
                    else
                    {
                        Positions[i] = UnityRect.SetFromTopLeft(CurrentCursor, ContainerSize - (2 * Spacing));
                        CurrentCursor = new Vector2(Spacing.x, CurrentCursor.y + ContainerSize.y);
                    }
                }

                return Positions;
            }

            private static int GetNumElementsFitPortrait(UIElement[] Elements, Vector2 ContainerSize, int CurrentIndex, float CurrentXCoordinate, float HorizontalSpacing)
            {
                if (CurrentIndex < Elements.Length)
                {
                    if ((2 * HorizontalSpacing) + CurrentXCoordinate + Elements[CurrentIndex].MinSize.x <= ContainerSize.x && Elements[CurrentIndex].MinSize.x != 0)
                    {
                        return 1 + GetNumElementsFitPortrait(Elements, ContainerSize, CurrentIndex + 1, (2 * HorizontalSpacing) + CurrentXCoordinate + Elements[CurrentIndex].MinSize.x, HorizontalSpacing);
                    }
                    else return 0;
                }
                else
                    return 0;
            }

            private Vector2[] GetPortraitElementSize(int CurrentIndex, int NumElementsOnLine, float HorizontalSpacing)
            {
                Vector2[] Sizes = new Vector2[NumElementsOnLine];
                float TotalMinElementSize = 0.0f;
                float SpareWidth = RectSize.x - (NumElementsOnLine * 2 * HorizontalSpacing);
                float MaxDeltaY = SortedElements[CurrentIndex].MinSize.y;

                for (int i = CurrentIndex; i < NumElementsOnLine + CurrentIndex; i++)
                {
                    if (SortedElements[i].MinSize.y > MaxDeltaY)
                    {
                        MaxDeltaY = SortedElements[i].MinSize.y;
                    }

                    SpareWidth -= SortedElements[i].MinSize.x;
                    TotalMinElementSize += SortedElements[i].MinSize.x;
                }
                Debug.Log(TotalMinElementSize);
                //Each Element is assigned a certain percentage of the spare width, based on how much its min size takes up of the full size
                for (int i = 0; i < NumElementsOnLine; i++)
                {
                    Sizes[i] = new Vector2(((SortedElements[i+CurrentIndex].MinSize.x / TotalMinElementSize) * SpareWidth) + SortedElements[i+CurrentIndex].MinSize.x, MaxDeltaY);
                }
                
                return Sizes;
            }
            #endregion
        }

        public class UIScene : UIContainer
        {
            #region Variables

            #endregion

            #region Initialisers
            public UIScene(string Tag, Color BGColor, int Priority = 1) : base(Tag, UIType.Scene, BGColor, Priority) { }
            public UIScene(string Tag, Image BGImg, int Priority = 1) : base(Tag, UIType.Scene, BGImg, Priority) { }
            #endregion

            #region Functions
            public override void Load(UIController Controller, UIElement Parent, Vector2 BotLeft, Vector2 BotRight)
            {
                base.Load(Controller, Parent, Vector2.zero, Vector2.zero);
            }

            public void Load(UIController Controller, UIElement Parent)
            {
                Load(Controller, Parent, Vector2.zero, Vector2.zero);
            }

            public override void Load(UIController Controller, Vector2 BotLeft, Vector2 BotRight)
            {
                base.Load(Controller, Vector2.zero, Vector2.zero);
            }

            public void Load(UIController Controller)
            {
                Load(Controller, Vector2.zero, Vector2.zero);
            }


            public override void Unload()
            {
                foreach (UIContainer Container in ContainedContainers)
                {
                    Container.Unload();
                }
                foreach (Interactives.UIInteractive Interactive in ContainedInteractives)
                {
                    Interactive.Unload();
                }
            }
            #endregion
        }


        public class UIPanel : UIContainer
        {
            #region Variables

            #endregion

            #region Initialisers
            public UIPanel(string Tag, Image BGImg, int Priority = 1) : base(Tag, UIType.Panel, BGImg, Priority)
            { }

            public UIPanel(string Tag, Color BGColor, int Priority = 1) : base(Tag, UIType.Panel, BGColor, Priority)
            { }

            public UIPanel(string Tag, Image BGImg, Vector2 MinSize, int Priority = 1) : base(Tag, UIType.Panel, BGImg, MinSize, Priority)
            { }            

            public UIPanel(string Tag, Color BGColor, Vector2 MinSize, int Priority = 1) : base(Tag, UIType.Panel, BGColor, MinSize, Priority)
            { }

            public UIPanel(string Tag, Image BGImg, Vector2 MinSize, Vector2 Location, int Priority = 1) : base(Tag, UIType.Panel, BGImg, MinSize, Location, Priority)
            { }

            public UIPanel(string Tag, Color BGColor, Vector2 MinSize, Vector2 Location, int Priority = 1) : base(Tag, UIType.Panel, BGColor, MinSize, Location, Priority)
            { }
            #endregion

            #region Function
            public override void Load(UIController Controller, UIElement Parent, Vector2 BotLeft, Vector2 TopRight)
            {
                base.Load(Controller, Parent, BotLeft, TopRight);
            }

            public override void Load(UIController Controller, Vector2 BotLeft, Vector2 TopRight)
            {
                base.Load(Controller, BotLeft, TopRight);
            }

            public override void Unload()
            {
                foreach (Containers.UIContainer Container in ContainedContainers)
                {
                    Container.Unload();
                }
                foreach (Interactives.UIInteractive Interactive in ContainedInteractives)
                {
                    Interactive.Unload();
                }
            }
            #endregion
        }

        //public class UISubmenu : UIContainer
        //{
        //    #region Variables
        //    public int CurrentTabIndex { get { return _CurrentTabIndex; } }
        //    private int _CurrentTabIndex = 0;

        //    private UIContainer Menu;
        //    private UIContainer CurrentTab;
        //    #endregion

        //    #region Initialisers
        //    public UISubmenu(string Tag, Color BGColor, Rect Location, UIPanel Default) : base(Tag, UIType.Submenu, BGColor, Location)
        //    { ContainedContainers.Add(Default); }

        //    public UISubmenu(string Tag, Color BGColor, UIPanel Default) : base(Tag, UIType.Submenu, BGColor)
        //    { ContainedContainers.Add(Default); }

        //    public UISubmenu(string Tag, Image BGImg, Rect Location, UIPanel Default) : base(Tag, UIType.Submenu, BGImg, Location)
        //    {   
        //        ContainedContainers.Add(Default); }

        //    public UISubmenu(string Tag, Image BGImg, UIPanel Default) : base(Tag, UIType.Submenu, BGImg)
        //    { ContainedContainers.Add(Default); }
        //    #endregion

        //    #region Functions
        //    public override void Load(UIController Controller, UIElement Parent)
        //    {
        //        base.Load(Controller, Parent);

        //        Menu = new UIPanel("Menu", BGColor, new Rect(0, 0, 20, 100));
        //        Menu.Load(Controller, this);
        //        CurrentTab = new UIPanel("CurrentTab", BGColor, new Rect(20, 0, 80, 100));
        //        CurrentTab.Load(Controller, this);

        //        //X increase ->, Y increase ^
        //        Vector2 BotLeft = new Vector2(5, 5); //Bottom Left Point relative to Bottom Left
        //        Vector2 TopRight = new Vector2(-5, -5); //Top Right point relative to Top Right

        //        GetTransform.anchorMin = new Vector2(0.0f, 0.0f);
        //        GetTransform.anchorMax = new Vector2(1.0f, 1.0f);

        //        if (Location.size.magnitude != 0)
        //        {
        //            BotLeft = new Vector2(Location.xMin, Parent.GetTransform.rect.size.y - Location.yMax);
        //            TopRight = new Vector2(Location.xMax - Parent.GetTransform.rect.size.x, -Location.yMin);
        //        }

        //        GetTransform.offsetMin = BotLeft;
        //        GetTransform.offsetMax = TopRight;

        //        //Load Sub Containers
        //        //Load Menu
        //        foreach (UIContainer C in ContainedContainers)
        //        {
        //            Interactives.UITriggers.UITriggerAction[] Actions = new Interactives.UITriggers.UITriggerAction[ContainedContainers.Count];

        //            for (int i = 0; i < ContainedContainers.Count; i++)
        //            {
        //                if (ContainedContainers[i].Tag != C.Tag)
        //                    Actions[i] = new Interactives.UITriggers.UITriggerAction(Interactives.UITriggers.UITriggerType.HideContainer, string.Join(".", ContainedContainers[i].TracePathFromContainer));
        //                else
        //                    Actions[i] = new Interactives.UITriggers.UITriggerAction(Interactives.UITriggers.UITriggerType.ShowContainer, string.Join(".", ContainedContainers[i].TracePathFromContainer));
        //            }
        //            Menu.addUIInteractive(new Interactives.UITriggers.UIButton(C.Tag, Color.grey, "Open " + C.Tag, Actions));
        //        }

        //        //Load CurrentTab
        //        CurrentTab.addUIContainer(ContainedContainers[CurrentTabIndex]);
                
        //    }

        //    public override void Load(UIController Controller)
        //    {
        //        base.Load(Controller, Parent);

        //        //X increase ->, Y increase ^
        //        Vector2 BotLeft = new Vector2(5, 5); //Bottom Left Point relative to Bottom Left
        //        Vector2 TopRight = new Vector2(-5, -5); //Top Right point relative to Top Right

        //        GetTransform.anchorMin = new Vector2(0.0f, 0.0f);
        //        GetTransform.anchorMax = new Vector2(1.0f, 1.0f);

        //        if (Location.size.magnitude != 0)
        //        {
        //            BotLeft = new Vector2(Location.xMin, Parent.GetTransform.rect.size.y - Location.yMax);
        //            TopRight = new Vector2(Location.xMax - Parent.GetTransform.rect.size.x, -Location.yMin);
        //        }

        //        GetTransform.offsetMin = BotLeft;
        //        GetTransform.offsetMax = TopRight;

        //        //Load Sub Containers
        //        //Load Menu
        //        foreach (UIContainer C in ContainedContainers)
        //        {
        //            Interactives.UITriggers.UITriggerAction[] Actions = new Interactives.UITriggers.UITriggerAction[ContainedContainers.Count];

        //            for (int i = 0; i < ContainedContainers.Count; i++)
        //            {
        //                if (ContainedContainers[i].Tag != C.Tag)
        //                    Actions[i] = new Interactives.UITriggers.UITriggerAction(Interactives.UITriggers.UITriggerType.HideContainer, string.Join(".", ContainedContainers[i].TracePathFromContainer));
        //                else
        //                    Actions[i] = new Interactives.UITriggers.UITriggerAction(Interactives.UITriggers.UITriggerType.ShowContainer, string.Join(".", ContainedContainers[i].TracePathFromContainer));
        //            }
        //            Menu.addUIInteractive(new Interactives.UITriggers.UIButton(C.Tag, Color.grey, "Open " + C.Tag, Actions));
        //        }

        //        //Load CurrentTab
        //        CurrentTab.addUIContainer(ContainedContainers[CurrentTabIndex]);
        //    }

        //    public override void Unload()
        //    {

        //    }

        //    public int ChangeTab(string TabTag)
        //    {
        //        foreach(UIContainer Container in ContainedContainers)
        //        {
        //            if (Container.Tag == TabTag)
        //            {
        //                int NewIndex = ContainedContainers.IndexOf(Container);
        //                ContainedContainers[CurrentTabIndex].Unload();
        //                _CurrentTabIndex = NewIndex;
        //                ContainedContainers[CurrentTabIndex].Load(Controller, this);
        //                return NewIndex;
        //            }
        //        }
        //        throw new System.Exception("Tab, " + TabTag + ", does not exist, cannot change tab in Submenu, " + Tag);
        //    }

        //    public string ChangeTab(int TabIndex)
        //    {
        //        if (ContainedContainers.Count < TabIndex)
        //        {
        //            ContainedContainers[CurrentTabIndex].Unload();
        //            _CurrentTabIndex = TabIndex;
        //            ContainedContainers[CurrentTabIndex].Load(Controller, this);
        //            return ContainedContainers[CurrentTabIndex].Tag;
        //        }
        //        throw new System.Exception("Tab, " + ContainedContainers[CurrentTabIndex].Tag + ", does not exist, cannot change tab in Submenu, " + Tag);
        //    }
        //    #endregion
        //}

        //
        //        public class UIGameScreen : UIContainer
        //        {
        //            public UIGameScreen(string Tag, Rect Location) : base(Tag, UIType.GameScreen, Location)
        //            { }
        //
        //            public UIGameScreen(string Tag) : base(Tag, UIType.GameScreen)
        //            { }
        //
        //            public override void Load(Canvas MainCanvas, UIBase Parent)
        //            {
        //
        //            }
        //
        //            public override void Load(Canvas MainCanvas, UIController Parent)
        //            {
        //
        //            }
        //
        //            public override void Unload()
        //            {
        //
        //            }
        //        }

    }

    namespace Interactives
    {
        public abstract class UIInteractive : UIElement
        {
            #region Variables
            public string Text { get { return _Text; } }
            protected string _Text;
            #endregion

            #region Inititalisms
            public UIInteractive(string Tag, UIType Type, Color BGColor, string Text, int Priority) : base(Tag, Type, BGColor, Priority)
            { }

            public UIInteractive(string Tag, UIType Type, Image BGImage, string Text, int Priority) : base(Tag, Type, BGImage, Priority)
            { }
               
            public UIInteractive (string Tag, UIType Type, Color BGColor, string Text, Vector2 MinSize, int Priority) : base(Tag, Type, BGColor, MinSize, Priority)
            {
                _Text = Text;
            }

            public UIInteractive(string Tag, UIType Type, Image BGImage, string Text, Vector2 MinSize, int Priority) : base(Tag, Type, BGImage, MinSize, Priority)
            {
                _Text = Text;
            }
            #endregion
            
            #region Functions
            public abstract void Trigger();
            #endregion
        }

        namespace UITriggers
        {
            public enum UITriggerType
            {
                None, ChangeScene, ShowContainer, HideContainer
            }

            public struct UITriggerAction
            {
                public UITriggerType Type;
                public string Data;

                public UITriggerAction(UITriggerType Type, string Data)
                {
                    this.Type = Type;
                    this.Data = Data;
                }
            }

            public abstract class UITrigger : UIInteractive
            {
                public UITriggerAction[] Actions { get { return _Actions; } }
                protected UITriggerAction[] _Actions;

                public UITrigger(string Tag, UIType Type, Color BGColor, string Text, UITriggerAction[] Action, int Priority) : base(Tag, Type, BGColor, Text, Priority)
                {
                    _Actions = Action;
                }

                public UITrigger(string Tag, UIType Type, Color BGColor, string Text, UITriggerAction[] Action, Vector2 MinSize, int Priority) : base(Tag, Type, BGColor, Text, Priority)
                {
                    _Actions = Action;
                }

                public UITrigger(string Tag, UIType Type, Image BGImage, string Text, UITriggerAction[] Action, int Priority) : base(Tag, Type, BGImage, Text, Priority)
                {
                    _Actions = Action;
                }

                public UITrigger(string Tag, UIType Type, Image BGImage, string Text, UITriggerAction[] Action, Vector2 MinSize, int Priority) : base(Tag, Type, BGImage, Text, MinSize, Priority)
                {
                    _Actions = Action;
                }
            }

            public class UIButton : UITrigger
            {
                public UIButton(string Tag, Color BGColor, string Text, UITriggerAction[] Action, int Priority = 1) : base(Tag, UIType.Button, BGColor, Text, Action, Priority)
                {
                    _Actions = Action;
                }

                public UIButton(string Tag, Color BGColor, string Text, UITriggerAction[] Action, Vector2 MinSize, int Priority = 1) : base(Tag, UIType.Button, BGColor, Text, Action, MinSize, Priority)
                {
                    _Actions = Action;
                }

                public UIButton(string Tag, Image BGImage, string Text, UITriggerAction[] Action, int Priority = 1) : base(Tag, UIType.Button, BGImage, Text, Action, Priority)
                {
                    _Actions = Action;
                }

                public UIButton(string Tag, Image BGImage, string Text, UITriggerAction[] Action, Vector2 MinSize, int Priority = 1) : base(Tag, UIType.Button, BGImage, Text, Action, MinSize, Priority)
                {
                    _Actions = Action;
                }

                public override void Load(UIController Controller, UIElement Parent, Vector2 BotLeft, Vector2 TopRight)
                {
                    base.Load(Controller, Parent, BotLeft, TopRight);
                }

                public override void Load(UIController Controller, Vector2 BotLeft, Vector2 TopRight)
                {
                    base.Load(Controller, BotLeft, TopRight);
                }

                public override void Trigger()
                {
                    foreach (UITriggerAction Action in Actions)
                    {
                        switch (Action.Type)
                        {
                            case UITriggerType.None:
                                throw new System.Exception("Button has no Button Type");

                            case UITriggerType.ChangeScene:
                                Controller.ChangeUIScene(Action.Data);
                                break;

                            case UITriggerType.ShowContainer:
                                string[] TagChain = Action.Data.Split('.');


                                break;

                            case UITriggerType.HideContainer:

                                break;
                        }
                    }
                }

            }

        }

        namespace UIModifiers
        {
            public abstract class UIModifier : UIInteractive
            {
                protected object Var;

                public UIModifier(string Tag, UIType Type, Color BGColor, string Text, ref object Var, int Priority) : base(Tag, Type, BGColor, Text, Priority)
                {
                    this.Var = Var;
                }

                public UIModifier(string Tag, UIType Type, Image BGImage, string Text, ref object Var, int Priority) : base(Tag, Type, BGImage, Text, Priority)
                {
                    this.Var = Var;
                }

                public override void Trigger()
                {
                    
                }
            }

//            public class UIToggle : UIModifier
//            {
//                public UIToggle(string Tag, string Text, ref object var) : base(Tag, UIType.Toggle, Text, ref var) { }
//
//                public override void Load(Canvas MainCanvas, UIBase Parent)
//                {
//
//                }
//
//                public override void Load(Canvas MainCanvas, UIController Parent)
//                {
//
//                }
//
//                public override void Unload()
//                {
//                    throw new NotImplementedException();
//                }
//
//                public override void Activate()
//                {
//                    throw new NotImplementedException();
//                }
//            }

        }
    }
}