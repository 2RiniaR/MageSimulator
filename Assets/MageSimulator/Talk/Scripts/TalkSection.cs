using System.Collections.Generic;
using UnityEngine;

namespace MageSimulator.Scripts.Talk
{
    [CreateAssetMenu(fileName = "TalkSection", menuName = "Talk/Section", order = 0)]
    public class TalkSection : ScriptableObject
    {
        public List<TalkContent> contents;
    }
}