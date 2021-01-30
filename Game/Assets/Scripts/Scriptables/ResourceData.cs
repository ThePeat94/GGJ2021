using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "Resource Data", menuName = "Data/Resource Data", order = 0)]
    public class ResourceData : ScriptableObject
    {
        [SerializeField] private int m_initMaxValue;
        [SerializeField] private int m_startValue;

        public int InitMaxValue => this.m_initMaxValue;
        public int StartValue => this.m_startValue;
    }
}