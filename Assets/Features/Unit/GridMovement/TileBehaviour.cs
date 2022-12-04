using Features.GlobalReferences;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Unit.GridMovement
{
    //https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
    public class TileBehaviour : MonoBehaviour, IDropHandler
    {
        [SerializeField] private float _speed = 1.5f;
        [SerializeField] private Color _baseColor, _offsetColor;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private GameObject _highlight;
        [SerializeField] private GridFocus_SO gridFocus;

        private Vector3 _gridPosition;
        private bool _lerpToThisTransform;
        private float _startTime;
        private float _journeyLength;
        private NetworkedUnitTilePlacementBehaviour _containedUnitTilePlacementBehaviour;

        public bool ContainsUnit => _containedUnitTilePlacementBehaviour != null;

        public void AddUnit(NetworkedUnitTilePlacementBehaviour localUnitTilePlacementBehaviour)
        {
            if (ContainsUnit)
            {
                Debug.LogWarning("Unit Has been overwritten!");
            }
            
            _containedUnitTilePlacementBehaviour = localUnitTilePlacementBehaviour;
        }

        public void RemoveUnit()
        {
            if (!ContainsUnit)
            {
                Debug.LogWarning("No Unit to be Removed!");
                return;
            }

            _containedUnitTilePlacementBehaviour = null;
        }

        public void Init(bool isOffset, Vector3 gridPosition)
        {
            _gridPosition = gridPosition;
            _renderer.color = isOffset ? _offsetColor : _baseColor;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null || !eventData.pointerDrag.TryGetComponent(out NetworkedUnitTilePlacementBehaviour unitDragBehaviour)) return;

            if (PhotonNetwork.IsMasterClient)
            {
                unitDragBehaviour.NetworkMove((int)RaiseEventCode.OnMasterChangeUnitGridPosition, ReceiverGroup.All, eventData.pointerDrag.GetComponent<PhotonView>().ViewID, _gridPosition, unitDragBehaviour.GridPosition);
            }
            else
            {
                unitDragBehaviour.NetworkMove((int)RaiseEventCode.OnRequestChangeUnitGridPosition, ReceiverGroup.MasterClient, eventData.pointerDrag.GetComponent<PhotonView>().ViewID, _gridPosition, unitDragBehaviour.GridPosition);
            }
        }

        private void Update()
        {
            if (!_lerpToThisTransform || !gridFocus.NotNull()) return;

            float distCovered = (Time.time - _startTime) * _speed;
            float fractionOfJourney = distCovered / _journeyLength;
            gridFocus.Get().transform.position = Vector3.Lerp(gridFocus.Get().transform.position,
                transform.position, fractionOfJourney);
        }

        private void OnMouseEnter()
        {
            _highlight.SetActive(true);
            
            if (gridFocus.NotNull())
            {
                _startTime = Time.time;                
                _journeyLength = Vector3.Distance(gridFocus.Get().transform.position, transform.position);
                
                _lerpToThisTransform = true;
                gridFocus.isFixedToMousePosition = false;
            }
        }

        private void OnMouseExit()
        {
            _highlight.SetActive(false);
            
            _lerpToThisTransform = false;
            gridFocus.isFixedToMousePosition = true;
        }
    }
}
