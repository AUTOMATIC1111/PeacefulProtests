using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PeacefulProtests
{
    public class PeacefulProtests : Mod
    {
        public PeacefulProtests(ModContentPack pack) : base(pack)
        {
            new Harmony("com.github.automatic1111.peacefulprotests").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
