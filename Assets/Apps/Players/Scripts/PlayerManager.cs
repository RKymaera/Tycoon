using System.Collections.Generic;
using UnityEngine;


namespace Apps.Players
{
    public class PlayerManager : MonoBehaviour
    {
        public List<PlayerBase> Players;
        public static readonly float CardSeparation = 0.01f;
    }
}