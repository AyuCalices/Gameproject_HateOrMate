using System.Collections;
using System.Collections.Generic;
using Features.Battle;
using Features.GlobalReferences;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Unit.GridMovement
{
    public class LocalUnitTilePlacementBehaviour : NetworkedUnitTilePlacementBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        //TODO: make sure the battle cant proceed into another state if a unit moves
        //TODO: there is a bug, where some cells are blocked (units not correctly assigned to current tile) - might resolve when correct refactoring below TODO
        //TODO: multiple moves after each other need to be chained directly in the move RaiseEventCallback
        
        [SerializeField] private float speed = 1.5f;
        [SerializeField] private CameraFocus_SO cameraFocus;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private GameObject visualObject;
        [SerializeField] private BattleData_SO battleData;

        private GameObject _instantiatedPrefab;
        private float _startTime;
        private float _journeyLength;
        private Vector3 _targetTileWorldPosition;
        private Vector3Int _targetTileGridPosition;

        private void Update()
        {
            if (battleData.CurrentState is not LootingState) return;
            
            if (_instantiatedPrefab == null || _journeyLength == 0) return;
            
            float distCovered = (Time.time - _startTime) * speed;
            float fractionOfJourney = distCovered / _journeyLength;
            
             _instantiatedPrefab.transform.position = Vector3.Lerp(_instantiatedPrefab.transform.position,
                _targetTileWorldPosition, fractionOfJourney);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (_instantiatedPrefab == null) return;
            
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        private Vector3 SetTileInterpolation(Vector3 worldPosition)
        {
            Vector3Int cellPosition = tileRuntimeDictionary.GetWorldToCellPosition(worldPosition);
            Vector3 tileWorldPosition = tileRuntimeDictionary.GetCellToWorldPosition(cellPosition);

            if (_targetTileWorldPosition != tileWorldPosition)
            {
                _targetTileGridPosition = cellPosition;   
                _targetTileWorldPosition = tileWorldPosition;
                _startTime = Time.time;                
                _journeyLength = Vector3.Distance(_targetTileWorldPosition, transform.position);
            }

            return tileWorldPosition;
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

            if (!tileRuntimeDictionary.ContainsGridPosition(_targetTileGridPosition)) return;
            
            if (eventData.pointerDrag == null) return;
            
            if (tileRuntimeDictionary.GenerateAStarPath(GridPosition,
                _targetTileGridPosition, out List<Vector3Int> path) && path.Count > 0)
            {
                StartCoroutine(MoveCoroutine(path));
            }
        }

        private IEnumerator MoveCoroutine(List<Vector3Int> path)
        {
            Vector3Int targetPosition = path[0];
            RequestMove(targetPosition);
            path.RemoveAt(0);
            float time = Vector3.Distance(transform.position, targetPosition) / _movementSpeed;
            yield return new WaitForSeconds(time);

            if (path.Count > 0)
            {
                StartCoroutine(MoveCoroutine(path));
            }
        }
    }
}
