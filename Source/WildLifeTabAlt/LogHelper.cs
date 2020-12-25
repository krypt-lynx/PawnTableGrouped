﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WildlifeTabAlt
{
    public static class LogHelper
    {
        public enum MessageType
        {
            Message,
            Warning,
            Error,
        }

        public static void Log(this string str, MessageType type = MessageType.Message)
        {
            switch (type)
            {
                case MessageType.Message:
                    Verse.Log.Message(str);
                    return;
                case MessageType.Warning:
                    Verse.Log.Warning(str);
                    return;
                case MessageType.Error:
                    Verse.Log.Error(str);
                    return;
            }
        }
    }
}
