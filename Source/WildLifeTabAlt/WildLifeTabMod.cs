using HarmonyLib;
using RimWorld;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WildlifeTabAlt
{
    public class WildLifeTabMod : CMod
    {
        public static string PackageIdOfMine = null;
        //public static Settings Settings { get; private set; }
        public static string CommitInfo = null;

        public WildLifeTabMod(ModContentPack content) : base(content)
        {
            ReadModInfo(content);

            Harmony harmony = new Harmony(PackageIdOfMine);

            ApplyPatches(harmony);
        }


        private static void ApplyPatches(Harmony harmony)
        {
            PawnTablePatches.Patch(harmony);
        }

        private static void ReadModInfo(ModContentPack content)
        {
            PackageIdOfMine = content.PackageId;

            var name = Assembly.GetExecutingAssembly().GetName().Name;

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(name + ".git.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    CommitInfo = reader.ReadToEnd()?.TrimEndNewlines();
                }
            }
            catch
            {
                CommitInfo = null;
            }
        }

    }
}
