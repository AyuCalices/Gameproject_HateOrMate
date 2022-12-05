using Features.GlobalReferences;
using Features.Tiles;
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
        private Vector3 _tileWorldPosition;
        private Vector3Int _targetGridPosition;
        
        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            tileRuntimeDictionary.SetAllOrderInLayer(1);
            _instantiatedPrefab = Instantiate(visualObject);
            
            _targetGridPosition = tileRuntimeDictionary.GetWorldToCellPosition(transform.position);
            _tileWorldPosition = tileRuntimeDictionary.GetCellToWorldPosition(_targetGridPosition);
            _instantiatedPrefab.transform.position = _tileWorldPosition;
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
                Vector3Int cellPosition = tileRuntimeDictionary.GetWorldToCellPosition(worldPosition);
                Vector3 tileWorldPosition = tileRuntimeDictionary.GetCellToWorldPosition(cellPosition);

                if (_tileWorldPosition != tileWorldPosition)
                {
                    _targetGridPosition = cellPosition;   
                    _tileWorldPosition = tileWorldPosition;
                    _startTime = Time.time;                
                    _journeyLength = Vector3.Distance(_instantiatedPrefab.transform.position, transform.position);
                }
                else
                {
                    float distCovered = (Time.time - _startTime) * speed;
                    float fractionOfJourney = distCovered / _journeyLength;
                    _instantiatedPrefab.transform.position = Vector3.Lerp(_instantiatedPrefab.transform.position,
                        _tileWorldPosition, fractionOfJourney);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Destroy(_instantiatedPrefab);
            tileRuntimeDictionary.SetAllOrderInLayer(-1);
            
            if (eventData.pointerDrag == null || !eventData.pointerDrag.TryGetComponent(out NetworkedUnitTilePlacementBehaviour unitDragBehaviour)) return;

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log(_targetGridPosition + " " + unitDragBehaviour.GridPosition);
                unitDragBehaviour.NetworkMove((int)RaiseEventCode.OnMasterChangeUnitGridPosition, ReceiverGroup.All, eventData.pointerDrag.GetComponent<PhotonView>().ViewID, _targetGridPosition, unitDragBehaviour.GridPosition);
            }
            else
            {
                unitDragBehaviour.NetworkMove((int)RaiseEventCode.OnRequestChangeUnitGridPosition, ReceiverGroup.MasterClient, eventData.pointerDrag.GetComponent<PhotonView>().ViewID, _targetGridPosition, unitDragBehaviour.GridPosition);
            }
        }
    }
}
