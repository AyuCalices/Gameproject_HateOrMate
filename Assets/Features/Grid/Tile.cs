using Features.Unit;
using UnityEngine;

namespace Features.Grid
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Color _baseColor, _offsetColor;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private GameObject _highlight;

        private LocalUnitBehaviour _localUnitBehaviour;

        public bool HasUnit => _localUnitBehaviour != null;

        public void AddUnit(LocalUnitBehaviour localUnitBehaviour)
        {
            if (HasUnit)
            {
                Debug.LogWarning("Unit Has been overwritten!");
            }
            
            _localUnitBehaviour = localUnitBehaviour;
        }

        public void RemoveUnit()
        {
            if (!HasUnit)
            {
                Debug.LogWarning("No Unit to be Removed!");
                return;
            }

            _localUnitBehaviour = null;
        }

        public void Init(bool isOffset)
        {
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