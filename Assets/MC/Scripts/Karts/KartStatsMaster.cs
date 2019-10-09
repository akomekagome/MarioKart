using UnityEngine;

namespace MC.Karts
{

    [CreateAssetMenu(menuName = "MyScriptable/Kar Statasmaster")]
    public class KartStatsMaster : ScriptableObject
    {
        public KartStats[] statses;
    }
}
