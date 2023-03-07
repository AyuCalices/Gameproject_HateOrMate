using System;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Camera.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Battle;
using ThirdParty.LeanTween.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Unit.Scripts.Behaviours
{
    [RequireComponent(typeof(UnitBattleBehaviour))]
    public class UnitDragPlacementBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public static Action<UnitServiceProvider, Vector3Int> onPerformTeleport;
        
        [Header("References")]
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private GameObject unitSpriteGameObject;
        [SerializeField] private CameraFocus_SO cameraFocus;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        [Header("Movement")]
        [SerializeField] private float hoverSpeed = 7f;
        [SerializeField] private LeanTweenType leanTweenType;
        
        private UnitServiceProvider _unitServiceProvider;
        private GameObject _instantiatedPrefab;
        private Vector3 _currentTileWorldPosition;

        public void Initialize(bool isEnabled)
        {
            _unitServiceProvider = GetComponent<UnitServiceProvider>();
            enabled = isEnabled;
        }

        public void OnDisable()
        {
            if (_instantiatedPrefab == null) return;
            
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;
        }

        private void Update()
        {
            if (_instantiatedPrefab == null) return;
            if (!battleData.StateIsValid(typeof(PlacementState), StateProgressType.Execute)) return;
            if (canvasFocus.Get() == null) return;
            if (!cameraFocus.NotNull()) return;
            if (LeanTween.isTweening(_instantiatedPrefab)) return;
            
            TweenOverGrid();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!battleData.StateIsValid(typeof(PlacementState), StateProgressType.Execute)) return;
            
            _instantiatedPrefab = Instantiate(unitSpriteGameObject);
            
            _currentTileWorldPosition = GetWorldToCellToWorldPosition(transform.position);
            _instantiatedPrefab.transform.position = _currentTileWorldPosition;
        }
        
        public void OnDrag(PointerEventData eventData) { }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!battleData.StateIsValid(typeof(PlacementState), StateProgressType.Execute)) return;
            
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;

            if (!battleData.TileRuntimeDictionary.ContainsGridPosition(battleData.TileRuntimeDictionary.GetWorldToCellPosition(_currentTileWorldPosition))) return;
            
            onPerformTeleport?.Invoke(_unitServiceProvider, battleData.TileRuntimeDictionary.GetWorldToCellPosition(_currentTileWorldPosition));
        }
        
        private void TweenOverGrid()
        {
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 10f;
            Vector3 worldPosition = cameraFocus.Get().ScreenToWorldPoint(screenPoint);
            Vector3 targetTileWorldPosition = GetWorldToCellToWorldPosition(worldPosition);
            if (targetTileWorldPosition == _currentTileWorldPosition) return;
            
            float speed = Vector3.Distance(_currentTileWorldPosition, targetTileWorldPosition) / hoverSpeed;
            _currentTileWorldPosition = targetTileWorldPosition;
            LeanTween.move(_instantiatedPrefab, targetTileWorldPosition, speed)
                .setEase(leanTweenType);
        }

        private Vector3 GetWorldToCellToWorldPosition(Vector3 worldPosition)
        {
            Vector3Int targetCellPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(worldPosition);
            return battleData.TileRuntimeDictionary.GetCellToWorldPosition(targetCellPosition);
        }
    }
}
