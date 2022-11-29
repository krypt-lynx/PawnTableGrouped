using RimWorld;
using RWLayout.alpha2;
using RWLayout.alpha2.FastAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PawnTableGrouped
{
    class CCheckBoxLabeledPaintable : CTitledElement
    {
        /// <summary>
        /// Is checkbox checked?
        /// </summary>
        public bool Checked
        {
            get => CheckedProp.Value;
            set => CheckedProp.Value = value;
        }
        public readonly Bindable<bool> CheckedProp = new Bindable<bool>(BindingMode.Manual, BindingMode.Auto);

        /// <summary>
        /// Called on Checked state change
        /// </summary>
        /// <remarks>first argument is wrapper itself, second argument is Checked state</remarks>
        public Action<CCheckBoxLabeledPaintable, bool> Changed { get; set; }


        public bool Disabled { get; set; } = false;
        public Texture2D TextureChecked { get; set; } = null;
        public Texture2D TextureUnchecked { get; set; } = null;
        public bool PlaceCheckboxNearText { get; set; } = false;
        public bool Paintable { get; set; } = false;

        public override Vector2 tryFit(Vector2 size)
        {
            var result = tryFitText(size, new Vector2(10 + 24, 0));
            if (result.y < 22)
            {
                result.y = 22;
            }
            return result;
        }

        static Func<bool> _get_checkboxPainting = Dynamic.StaticGetField<bool>(typeof(Widgets).GetField("checkboxPainting", BindingFlags.Static | BindingFlags.NonPublic));
        static Action<bool> _set_checkboxPainting = Dynamic.StaticSetField<bool>(typeof(Widgets).GetField("checkboxPainting", BindingFlags.Static | BindingFlags.NonPublic));
        static bool isPainting {
            get => _get_checkboxPainting();
            set => _set_checkboxPainting(value);
        }

        static Func<bool> _get_checkboxPaintingState = Dynamic.StaticGetField<bool>(typeof(Widgets).GetField("checkboxPaintingState", BindingFlags.Static | BindingFlags.NonPublic));
        static Action<bool> _set_checkboxPaintingState = Dynamic.StaticSetField<bool>(typeof(Widgets).GetField("checkboxPaintingState", BindingFlags.Static | BindingFlags.NonPublic));
        static bool paintingValue
        {
            get => _get_checkboxPaintingState();
            set => _set_checkboxPaintingState(value);
        }

        Rect labelRect;
        Vector2 checkboxLocation;
        public override void PostLayoutUpdate()
        {
            base.PostLayoutUpdate();
            checkboxLocation = new Vector2(BoundsRounded.xMin, (BoundsRounded.yMin + BoundsRounded.yMax - 24) / 2);
            labelRect = Rect.MinMaxRect(BoundsRounded.xMin + 24 + 10, BoundsRounded.yMin, BoundsRounded.xMax, BoundsRounded.yMax);
        }

#if rw_1_3_or_earlier
        static Action<float, float, bool, bool, float, Texture2D, Texture2D> _Widgets_CheckboxDraw =
            Dynamic.CreateMethodCaller<Action<float, float, bool, bool, float, Texture2D, Texture2D>>(typeof(Widgets).GetMethod("CheckboxDraw", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));
#endif

        public override void DoContent()
        {
            base.DoContent();
            CheckedProp.SynchronizeFrom();
            var newChecked = Checked;

            GuiTools.PushColor(UnityEngine.Color.white);
#if rw_1_4_or_later
            Widgets.CheckboxDraw(checkboxLocation.x, checkboxLocation.y, newChecked, Disabled, 24, TextureChecked, TextureUnchecked);
#else
            _Widgets_CheckboxDraw(checkboxLocation.x, checkboxLocation.y, newChecked, Disabled, 24, TextureChecked, TextureUnchecked);
#endif
            GuiTools.PopColor();
            ApplyAll();
            Widgets.Label(labelRect, TaggedTitle);
            RestoreAll();

            if (!Disabled)
            {
                MouseoverSounds.DoRegion(BoundsRounded);

                bool flag = false;
                Widgets.DraggableResult draggableResult = Widgets.ButtonInvisibleDraggable(BoundsRounded, false);
                if (draggableResult == Widgets.DraggableResult.Pressed)
                {
                    newChecked = !newChecked;
                    flag = true;
                }
                else if (draggableResult == Widgets.DraggableResult.Dragged && Paintable)
                {
                    newChecked = !newChecked;
                    flag = true;
                    isPainting = true;
                    paintingValue = newChecked;
                }
                if (Paintable && Mouse.IsOver(BoundsRounded) && isPainting && Input.GetMouseButton(0) && newChecked != paintingValue)
                {
                    newChecked = paintingValue;
                    flag = true;
                }
                if (flag)
                {
                    if (newChecked)
                    {
                        SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                    }
                    else
                    {
                        SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                    }
                }
            }
            
                
            if (newChecked != Checked)
            {
                Checked = newChecked;
                Changed?.Invoke(this, Checked);
            }
        }


    }
}
