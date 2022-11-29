using Features.Unit;
using UnityEngine;

namespace Features.Grid
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color _baseColor, _offsetColor;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private GameObject _highlight;

        private NetworkedUnitBehaviour _localUnitBehaviour;

        public Vector3 GridPosition { get; private set; }
        public bool ContainsUnit => _localUnitBehaviour != null;

        public void AddUnit(NetworkedUnitBehaviour localUnitBehaviour)
        {
            if (ContainsUnit)
            {
                Debug.LogWarning("Unit Has been overwritten!");
            }
            
            _localUnitBehaviour = localUnitBehaviour;
        }

        public void RemoveUnit()
        {
            if (!ContainsUnit)
            {
                Debug.LogWarning("No Unit to be Removed!");
                return;
            }

            _localUnitBehaviour = null;
        }

        public void Init(bool isOffset, Vector3 gridPosition)
        {
            GridPosition = gridPosition;
            _renderer.color = isOffset ? _offsetColor : _baseColor;
        }

        private void OnMouseEnter()
        {
            _highlight.SetActive(true);
        }

        private void OnMouseExit()
        {
            _highlight.SetActive(false);
        }
    }
}