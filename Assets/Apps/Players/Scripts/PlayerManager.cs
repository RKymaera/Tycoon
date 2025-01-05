using System.Collections.Generic;
using UnityEngine;


namespace Apps.Players
{
    public class PlayerManager : MonoBehaviour
    {
        public List<PlayerBase> Players;
        public Vector3 CardSeparation = new Vector3(0.01f, 0, .001f);
    }
}