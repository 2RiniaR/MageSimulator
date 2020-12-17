using System;
using UnityEngine;

namespace MageSimulator.Scripts.Talk
{
    [Serializable]
    public struct TalkContent
    {
        public string talker;

        [Multiline]
        public string content;
    }
}