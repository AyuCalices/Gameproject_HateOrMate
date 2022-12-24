using System;
using System.Collections.Generic;
using Features.ModView;
using UnityEngine;

namespace Features.Unit.Modding
{
    [RequireComponent(typeof(NetworkedStatsBehaviour))]
    public class ModUnitBehaviour : MonoBehaviour
    {
        [Header("Balancing")]
        [SerializeField] private int modCount;
    
        public UnitMods UnitMods { get; private set; }
    
        public static Func<UnitModHud> onInstantiateModSlot;

        private UnitModHud _unitModView;
    
        // Start is called before the first frame update
        private void Awake()
        {
            _unitModView = onInstantiateModSlot.Invoke();
            List<ModSlotBehaviour> modDropBehaviours = _unitModView.GetAllChildren();
            //TODO: getComponent
            UnitMods = new UnitMods(modCount, GetComponent<NetworkedStatsBehaviour>(), modDropBehaviours);
        }

        private void OnDestroy()
        {
            Destroy(_unitModView.gameObject);
        }
    }
}
