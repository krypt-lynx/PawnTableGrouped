using RimWorld.Planet;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    /// <summary>
    /// Submenu option for FloatMenu
    /// </summary>
    class FloatSubmenuOption : FloatMenuOption
    {
        const float indicatorWidth = 10;
        static readonly Resource<Texture2D> TexSubmenuIndicator = new Resource<Texture2D>("UI/Buttons/Dev/Reveal");

        Func<List<FloatMenuOption>> optionsGenerator = null;
        bool closed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="optionsGenerator">submenu options generator</param>
        /// <param name="priority"></param>
        /// <param name="mouseoverGuiAction"></param>
        /// <param name="revalidateClickTarget"></param>
        /// <param name="extraPartWidth"></param>
        /// <param name="extraPartOnGUI"></param>
        /// <param name="revalidateWorldClickTarget"></param>
        public FloatSubmenuOption(string label, Func<List<FloatMenuOption>> optionsGenerator, MenuOptionPriority priority = MenuOptionPriority.Default,
#if rw_1_2_or_earlier
            Action mouseoverGuiAction = null,
#else
            Action<Rect> mouseoverGuiAction = null,
#endif
            Thing revalidateClickTarget = null, float extraPartWidth = 0,
            Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null) :
            base(label, null, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth + indicatorWidth, extraPartOnGUI, revalidateWorldClickTarget)
        {
            this.action = OptionSelected;
            this.optionsGenerator = optionsGenerator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="optionsGenerator">submenu options generator</param>
        /// <param name="shownItemForIcon"></param>
        /// <param name="priority"></param>
        /// <param name="mouseoverGuiAction"></param>
        /// <param name="revalidateClickTarget"></param>
        /// <param name="extraPartWidth"></param>
        /// <param name="extraPartOnGUI"></param>
        /// <param name="revalidateWorldClickTarget"></param>
        public FloatSubmenuOption(string label, Func<List<FloatMenuOption>> optionsGenerator, ThingDef shownItemForIcon, MenuOptionPriority priority = MenuOptionPriority.Default,
#if rw_1_2_or_earlier
            Action mouseoverGuiAction = null,
#else
            Action<Rect> mouseoverGuiAction = null,
#endif      
            Thing revalidateClickTarget = null, float extraPartWidth = 0,
            Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null) :
            base(label, null, shownItemForIcon, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth + indicatorWidth, extraPartOnGUI, revalidateWorldClickTarget)
        {
            this.action = OptionSelected;
            this.optionsGenerator = optionsGenerator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="optionsGenerator">submenu options generator</param>
        /// <param name="itemIcon"></param>
        /// <param name="iconColor"></param>
        /// <param name="priority"></param>
        /// <param name="mouseoverGuiAction"></param>
        /// <param name="revalidateClickTarget"></param>
        /// <param name="extraPartWidth"></param>
        /// <param name="extraPartOnGUI"></param>
        /// <param name="revalidateWorldClickTarget"></param>
        public FloatSubmenuOption(string label, Func<List<FloatMenuOption>> optionsGenerator, Texture2D itemIcon, Color iconColor, MenuOptionPriority priority = MenuOptionPriority.Default,
#if rw_1_2_or_earlier
            Action mouseoverGuiAction = null,
#else
            Action<Rect> mouseoverGuiAction = null,
#endif  
            Thing revalidateClickTarget = null, float extraPartWidth = 0,
            Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null) :
            base(label, null, itemIcon, iconColor, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth + indicatorWidth, extraPartOnGUI, revalidateWorldClickTarget)
        {
            this.action = OptionSelected;
            this.optionsGenerator = optionsGenerator;
        }

        //static Func<FloatMenu, List<FloatMenuOption>> get_FloatMenu_options = RWLayout.alpha2.FastAccess.Dynamic.InstanceGetField<FloatMenu, List<FloatMenuOption>>("options");
        static Func<FloatMenu, List<FloatMenuOption>> get_FloatMenu_options = (x) => (List<FloatMenuOption>)typeof(FloatMenu).GetField("options", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(x);

        /// <summary>
        /// Show submenu on selection
        /// </summary>
        void OptionSelected()
        {
            if (submenu == null)
            {
                // Close all other opened submenus
                if (OwnerMenu != null)
                {
                    CloseAllSubmenus(OwnerMenu);
                }

                // create and show submenu
                submenu = new FixedFloatMenu(GUIUtility.GUIToScreenRect(optionRect), optionsGenerator());
                submenu.onCloseCallback = () =>
                {
                    closed = true;
                };
                Find.WindowStack.Add(submenu);
            }
        }

        private void CloseAllSubmenus(FloatMenu menu)
        {
            foreach (var menuOption in get_FloatMenu_options(menu) ?? Enumerable.Empty<FloatMenuOption>())
            {
                if (menuOption is FloatSubmenuOption othersubmenu)
                {
                    othersubmenu.CloseSubmenu();
                }
            }
        }

        public void CloseSubmenu()
        {
            if (submenu != null)
            {
                submenu.onCloseCallback = null; // removing callback to prevent menu tree collapse
                CloseAllSubmenus(submenu);
                submenu.Close();
                submenu = null;
            }
        }

        Rect optionRect;
        bool needCaptureOwnerMenu = true;
        FixedFloatMenu submenu = null;
        private Verse.WeakReference<FloatMenu> ownerMenu = null;
        public FloatMenu OwnerMenu
        {
            get => ownerMenu?.Target;
            set => ownerMenu = new Verse.WeakReference<FloatMenu>(value);
        }

        public bool DoSubmenuIndicator { get; set; } = true;

        public override bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
        {
            if (needCaptureOwnerMenu)
            {
                // hihacking owner menu on first interaction
                needCaptureOwnerMenu = false;
                CaptureOwnerMenu(rect, floatMenu);
            }
            base.DoGUI(rect, colonistOrdering, floatMenu);

            // submenu indicator
            if (DoSubmenuIndicator)
            {
                float offset = (!this.Disabled && Mouse.IsOver(rect)) ? 0 : -2;

                var tex = TexSubmenuIndicator.Value.Size() * 0.6666f;
                GUI.DrawTexture(new Rect(rect.xMax - tex.x + offset, rect.yMin + (rect.height - tex.y) / 2, tex.x, tex.y), TexSubmenuIndicator.Value);
            }

            return closed; // do not close the window untill sub option selected;
        }

        private void CaptureOwnerMenu(Rect rect, FloatMenu floatMenu)
        {
            // A bit hacky, but does not require parent menu subclass
            floatMenu.vanishIfMouseDistant = false;
            var oldCloseCallback = floatMenu.onCloseCallback;
            var weakThis = new Verse.WeakReference<FloatSubmenuOption>(this);
            floatMenu.onCloseCallback = () =>
            {
                weakThis?.Target?.submenu?.Close(false);
                oldCloseCallback?.Invoke();
            };
            optionRect = rect;
            this.OwnerMenu = floatMenu;
        }
    }


    /// <summary>
    /// Menu window with fixed screen location
    /// </summary>
    class FixedFloatMenu : FloatMenu
    {
        Rect screenLocation = Rect.zero;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">Menu location in screen coordinates</param>
        /// <param name="options">menu items</param>
        /// <remarks> Menu will be shown at the edge of provided rect. Default edge is the right one, but it will be shown at the left if there is not enough space at the right.
        /// 
        /// To obtain screen coordinates use GUIUtility.GUIToScreenRect method</remarks>
        public FixedFloatMenu(Rect location, List<FloatMenuOption> options) : base(options)
        {
            onlyOneOfTypeAllowed = false;
            vanishIfMouseDistant = false;
            screenLocation = location;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="location">menu location in screen coordinates</param>
        /// <param name="options">menu items</param>
        /// <param name="needSelection">whatever it is in vanilla</param>
        /// <remarks> Menu will be shown at the edge of provided rect. Default edge is the right one, but it will be shown at the left if there is not enough space at the right.
        /// 
        /// To obtain screen coordinates use GUIUtility.GUIToScreenRect method</remarks>
        public FixedFloatMenu(Rect location, List<FloatMenuOption> options, string title, bool needSelection = false) : base(options, title, needSelection)
        {
            onlyOneOfTypeAllowed = false;
            vanishIfMouseDistant = false;
            screenLocation = location;
        }

        protected override void SetInitialSizeAndPosition()
        {
            var menuSize = this.InitialSize;

            // trying to show menu at the right of provided rect
            Vector2 vector = new Vector2(screenLocation.xMax, screenLocation.yMin);
            if (vector.x + this.InitialSize.x > (float)UI.screenWidth)
            {
                // but showing at the left if there is not enough space
                vector.x = screenLocation.xMin - menuSize.x;
            }
            if (vector.y + this.InitialSize.y > (float)UI.screenHeight)
            {
                vector.y = (float)UI.screenHeight - menuSize.y;
            }
            this.windowRect = new Rect(vector.x, vector.y, menuSize.x, menuSize.y);
        }
    }
}
