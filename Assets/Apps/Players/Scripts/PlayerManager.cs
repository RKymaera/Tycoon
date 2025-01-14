using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Apps.Common.Utils;


namespace Apps.Players
{
    public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
    {
        public List<PlayerBase> Players;
        public Vector3 CardSeparation = new Vector3(0.01f, 0, .001f);
    }
}