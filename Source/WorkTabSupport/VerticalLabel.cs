using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;
using RWLayout.alpha2.FastAccess;

namespace PawnTableGrouped
{
    static class RenderHelper
    {
        static Func<Vector2, Vector2> UnclipVector2 = Dynamic.StaticRetMethod<Vector2, Vector2>(AccessTools.TypeByName("GUIClip").GetMethod("Unclip", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(Vector2) }, null));

        static public void VerticalLabel(Rect rect, string text)
        {
            // the issue:
            // while we transforming gui, whe clip area rotates with it
            // so, if we want to draw somewhere on untrasformed screen, we have invalid clip area to deal with.

            // the best idea I have is to draw on (0,0) of current clip area and move label to desized position using gui matrix intead of label coordinates
            // limitations:
            // we need to apply clip effect to transformed label manually, we need to know clip area size for that, but I see no way to obrain it (in this implementation I do not clip right and bottom sides at all, though)
            // original clip is still applied, and width and height is now inverted. So, original clip have no space to render the label *horizontaly*, the vertical label will not be rendered correctly either.

            Matrix4x4 matrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;

            Vector2 unclipped = UnclipVector2(Vector2.zero);

            GUI.matrix =
                // move origin to (0,0) of current clip; rotate around (0,0)
                Matrix4x4.TRS(unclipped, Quaternion.Euler(0f, 0f, -90), Vector3.one) *
                // move (0,0) to label possition
                Matrix4x4.TRS(new Vector2(-rect.yMax - unclipped.x, rect.xMin - unclipped.y), Quaternion.identity, Vector3.one) *
                GUI.matrix;

            // calculating clipping effects
            var leftClip = Mathf.Min(rect.xMin, 0);
            var topClip = Mathf.Min(rect.yMin, 0);

            // applying clip
            var clip = new Rect(0, -leftClip, rect.height + topClip, rect.width + leftClip);
            GUI.BeginClip(clip);

            // rendering whatever we want to render
            var adjusted = new Rect(0, leftClip, rect.height + 1, rect.width); // +1 is to avoid jittering caused by worktab being a little to greedy with label size
            Widgets.Label(adjusted, text);

            GUI.EndClip();

            // restoring GUI matrix
            GUI.matrix = matrix;
        }
    }
}
