using System.Collections;
using System.Collections.Generic;
using Aoiti.Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace ThirdParty.Aoiti.Example.Tilemap
{
    public class MoveOnTilemap : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private GameObject dragObject;
        [SerializeField] private Camera camera;
    
        Vector3Int[] directions=new Vector3Int[4] {Vector3Int.left,Vector3Int.right,Vector3Int.up,Vector3Int.down };

        public UnityEngine.Tilemaps.Tilemap tilemap;
        public TileAndMovementCost[] tiles;
        Pathfinder<Vector3Int> pathfinder;

        [System.Serializable]
        public struct TileAndMovementCost
        {
            public Tile tile;
            public bool movable;
            public float movementCost;
        }

        public List<Vector3Int> path;
        [Range(0.001f,1f)]
        public float stepTime;


        public float DistanceFunc(Vector3Int a, Vector3Int b)
        {
            return (a-b).sqrMagnitude;
        }


        public Dictionary<Vector3Int,float> connectionsAndCosts(Vector3Int a)
        {
            Dictionary<Vector3Int, float> result= new Dictionary<Vector3Int, float>();
            foreach (Vector3Int dir in directions)
            {
                foreach (TileAndMovementCost tmc in tiles)
                {
                    if (tilemap.GetTile(a+dir)==tmc.tile)
                    {
                        if (tmc.movable) result.Add(a + dir, tmc.movementCost);

                    }
                }
                
            }
            return result;
        }

        // Start is called before the first frame update
        void Start()
        {
            pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, connectionsAndCosts);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(1) )
            {
                var currentCellPos=tilemap.WorldToCell(transform.position);
                var target = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                target.z = 0;
                pathfinder.GenerateAstarPath(currentCellPos, target, out path);
                StopAllCoroutines();
                StartCoroutine(Move());
            }

        
        }

        IEnumerator Move()
        {
            while (path.Count > 0)
            {
                transform.position = tilemap.CellToWorld(path[0]);
                path.RemoveAt(0);
                yield return new WaitForSeconds(stepTime);
            }
        }

        private GameObject _instantiatedDragObject;
    
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("o/");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _instantiatedDragObject = Instantiate(dragObject, transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 10f;
            _instantiatedDragObject.transform.position = camera.ScreenToWorldPoint(screenPoint);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var currentCellPos=tilemap.WorldToCell(transform.position);
            Vector3Int target = tilemap.WorldToCell(_instantiatedDragObject.transform.position);
            target.z = 0;
            pathfinder.GenerateAstarPath(currentCellPos, target, out path);
            StopAllCoroutines();
            StartCoroutine(Move());
        
            Destroy(_instantiatedDragObject);
        }
    }
}

