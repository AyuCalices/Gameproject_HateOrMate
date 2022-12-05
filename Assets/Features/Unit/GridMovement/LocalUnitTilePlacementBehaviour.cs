using Features.GlobalReferences;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Unit.GridMovement
{
    public class LocalUnitTilePlacementBehaviour : NetworkedUnitTilePlacementBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private float speed = 1.5f;
        [SerializeField] private CameraFocus_SO cameraFocus;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private GameObject visualObject;

        private GameObject _instantiatedPrefab;
        private float _startTime;
        private float _journeyLength;
        private Vector3 _targetTileWorldPosition;
        private Vector3Int _targetTileGridPosition;

        private void Update()
        {
            if (_instantiatedPrefab == null || _journeyLength == 0) return;
            
            float distCovered = (Time.time - _startTime) * speed;
            float fractionOfJourney = distCovered / _journeyLength;
            
             _instantiatedPrefab.transform.position = Vector3.Lerp(_instantiatedPrefab.transform.position,
                _targetTileWorldPosition, fractionOfJourney);
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
            tileRuntimeDictionary.SetAllOrderInLayer(1);
            _instantiatedPrefab = Instantiate(visualObject);
            _instantiatedPrefab.transform.position = SetTileInterpolation(transform.position);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            //TODO: when a unit dies while dragging these events get lost => dont make player be able to drag while battle
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
            Destroy(_instantiatedPrefab);
            _instantiatedPrefab = null;
            
            tileRuntimeDictionary.SetAllOrderInLayer(-1);
            
            if (eventData.pointerDrag == null || !eventData.pointerDrag.TryGetComponent(out NetworkedUnitTilePlacementBehaviour unitDragBehaviour)) return;

            if (PhotonNetwork.IsMasterClient)
            {
                unitDragBehaviour.NetworkMove((int)RaiseEventCode.OnMasterChangeUnitGridPosition, ReceiverGroup.All, eventData.pointerDrag.GetComponent<PhotonView>().ViewID, _targetTileGridPosition, unitDragBehaviour.GridPosition);
            }
            else
            {
                unitDragBehaviour.NetworkMove((int)RaiseEventCode.OnRequestChangeUnitGridPosition, ReceiverGroup.MasterClient, eventData.pointerDrag.GetComponent<PhotonView>().ViewID, _targetTileGridPosition, unitDragBehaviour.GridPosition);
            }
        }
    }
}
