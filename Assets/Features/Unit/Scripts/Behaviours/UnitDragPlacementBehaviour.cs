using System;
using Features.Battle.Scripts;
using Features.Camera.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Unit.Scripts.Behaviours
{
    [RequireComponent(typeof(BattleBehaviour))]
    public class UnitDragPlacementBehaviour : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public static Action<BattleBehaviour, Vector3Int> onPerformTeleport;
        
        [Header("References")]
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private GameObject visualObject;
        [SerializeField] private CameraFocus_SO cameraFocus;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        [Header("Balancing")]
        [SerializeField] private float hoverSpeed = 1.5f;
        
        private BattleBehaviour _battleBehaviour;
        private GameObject _instantiatedPrefab;
        private float _startTime;
        private float _journeyLength;
        private Vector3 _targetTileWorldPosition;
        private Vector3Int _targetTileGridPosition;

        private void Awake()
        {
            _battleBehaviour = GetComponent<BattleBehaviour>();
        }

        public void OnDisable()
        {
            if (_instantiatedPrefab == null) return;
            
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;
        }
        
        private void Update()
        {
            if (battleData.CurrentState is not LootingState) return;

            if (_instantiatedPrefab != null && _journeyLength != 0)
            {
                float distCovered = (Time.time - _startTime) * hoverSpeed;
                float fractionOfJourney = distCovered / _journeyLength;

                _instantiatedPrefab.transform.position = Vector3.Lerp(_instantiatedPrefab.transform.position,
                    _targetTileWorldPosition, fractionOfJourney);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (battleData.CurrentState is not LootingState) return;
            
            _instantiatedPrefab = Instantiate(visualObject);
            _instantiatedPrefab.transform.position = SetTileInterpolation(transform.position);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (battleData.CurrentState is not LootingState) return;
            
            if (canvasFocus.Get() == null) return;

            if (cameraFocus.NotNull())
            {
                Vector3 screenPoint = Input.mousePosition;
                screenPoint.z = 10f;
                Vector3 worldPosition = cameraFocus.Get().ScreenToWorldPoint(screenPoint);
                SetTileInterpolation(worldPosition);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (battleData.CurrentState is not LootingState) return;
            
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;

            if (!battleData.TileRuntimeDictionary.ContainsGridPosition(_targetTileGridPosition)) return;
            
            onPerformTeleport?.Invoke(_battleBehaviour, _targetTileGridPosition);
        }
        
        private Vector3 SetTileInterpolation(Vector3 targetWorldPosition)
        {
            Vector3Int targetCellPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(targetWorldPosition);
            Vector3 targetTileWorldPosition = battleData.TileRuntimeDictionary.GetCellToWorldPosition(targetCellPosition);

            if (_targetTileWorldPosition != targetTileWorldPosition)
            {
                _targetTileGridPosition = targetCellPosition;   
                _targetTileWorldPosition = targetTileWorldPosition;
                _startTime = Time.time;                
                _journeyLength = Vector3.Distance(_targetTileWorldPosition, transform.position);
            }

            return targetTileWorldPosition;
        }
    }
}
