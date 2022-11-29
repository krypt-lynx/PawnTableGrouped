using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PawnTableGrouped
{
    [StaticConstructorOnStartup]
    static public class Textures
    {
        public static readonly Texture2D TexButton_Paste = ContentFinder<Texture2D>.Get("UI/Buttons/Paste");
        public static readonly Texture2D LinearGradient = ContentFinder<Texture2D>.Get("UI/PTG_linear_gradient");
        public static readonly Texture2D RadialGradient = ContentFinder<Texture2D>.Get("UI/PTG_radial_gradient");
        public static readonly Texture2D AltTexture = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.03f));
    }
}
