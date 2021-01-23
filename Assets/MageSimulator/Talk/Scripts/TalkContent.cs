using System;
using UnityEngine;

namespace MageSimulator.Talk.Scripts
{
    [Serializable]
    public struct TalkContent
    {
        public string talker;

        [Multiline]
        public string content;
    }
}