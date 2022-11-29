using System;
using ExitGames.Client.Photon;
using Features.ModView;
using Features.Unit;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Grid
{
    //https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
    public class UnitDropBehaviour : MonoBehaviour, IDropHandler
    {
        [SerializeField] private GameObjectFocus_SO gameObjectFocus;
        
        private float _speed = 1.5f;

        private bool _lerpToThisTransform;
        private float _startTime;
        private float _journeyLength;


        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null || !eventData.pointerDrag.TryGetComponent(out NetworkedUnitDragBehaviour unitDragBehaviour)) return;
            
            if (PhotonNetwork.IsMasterClient)
            {
                unitDragBehaviour.NetworkMove(eventData.pointerDrag.GetComponent<PhotonView>().ViewID, transform.position);
            }
            else
            {
                unitDragBehaviour.RequestMove(eventData.pointerDrag.GetComponent<PhotonView>().ViewID, transform.position);
            }
            
            Debug.Log("called");
        }

        private void Update()
        {
            if (!_lerpToThisTransform || !gameObjectFocus.NotNull()) return;

            float distCovered = (Time.time - _startTime) * _speed;
            float fractionOfJourney = distCovered / _journeyLength;
            gameObjectFocus.Get().transform.position = Vector3.Lerp(gameObjectFocus.Get().transform.position,
                transform.position, fractionOfJourney);
        }

        private void OnMouseEnter()
        {
            if (gameObjectFocus.NotNull())
            {
                _startTime = Time.time;                
                _journeyLength = Vector3.Distance(gameObjectFocus.Get().transform.position, transform.position);
                
                _lerpToThisTransform = true;
                gameObjectFocus.isFixedToMousePosition = false;
            }
        }

        private void OnMouseExit()
        {
            _lerpToThisTransform = false;
            gameObjectFocus.isFixedToMousePosition = true;
        }
    }
}
