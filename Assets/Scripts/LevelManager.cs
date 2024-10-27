using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance {set; get;}
    public bool SHOW_COLLIDER = true;  //$$


    // level spawning
    private const float DISTANCE_BEFORE_SPAWN = 100.0f;
    private const int INITAL_SEGMENTS = 10;
    private const int INITAL_Transition_SEGMENTS = 2;
    private const int MAX_SEGEMNTS_ON_SCREEN = 15;
    private Transform cameraContainer;
    private int amountOfActiveSegments;
    private int continousSegments;
    private int currentSpawnZ;
    private int y1, y2, y3;
    private int currentLevel;


    //List of segments
    public List<Segment> availableSegments = new List<Segment>();
    public List<Segment> availableTransitions = new List<Segment>();
    [HideInInspector]
    public List<Segment> segments = new List<Segment>();

    //game play
    private bool isMoving = false;


    // list of pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();
    [HideInInspector]
    public List<Piece> pieces = new List<Piece>(); // all the pieces in the pool


    private void Awake()
    {
        Instance = this;
        cameraContainer = Camera.main.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
    }

    private void Start()
    {
        for( int i = 0; i < INITAL_SEGMENTS; i++){
            if(i < INITAL_Transition_SEGMENTS)
                SpawnTransition();
            else 
                GenerateSegemnts();
        }
    }

    private void Update(){
        if(currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
            GenerateSegemnts();
        if(amountOfActiveSegments >= MAX_SEGEMNTS_ON_SCREEN){
            segments[amountOfActiveSegments - 1].DeSpawn();
            amountOfActiveSegments--;
        }
    }
    private void GenerateSegemnts(){
        SpawnSegment();

        if(Random.Range(0f, 1f) < (continousSegments * 0.25f)){
            //spawn transition segments
            continousSegments = 0;
            SpawnTransition();
        } else {
            continousSegments++;
        }
    }

    private void SpawnSegment(){
        List<Segment> possibleSeg = availableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSeg.Count);

        Segment s = GetSegment(id, false);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    private void SpawnTransition(){
        List<Segment> possibleTransition = availableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.length;
        amountOfActiveSegments++;
        s.Spawn();
    }

    public Segment GetSegment(int id, bool transition){
        Segment s = null;
        s = segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);
        
        
        if(s == null){
            GameObject go = Instantiate((transition) ? availableTransitions[id].gameObject : availableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segment>();
            s.SegId = id;
            s.transition = transition;

            segments.Insert(0, s);
        } else {
            segments.Remove(s);
            segments.Insert(0, s);
        }

        return s;
    }

    public Piece GetPiece(PieceType pt, int visualIndex){
        Piece p = pieces.Find(x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf);
        if(p == null){
            GameObject go = null;
            if(pt == PieceType.ramp)
                go = ramps[visualIndex].gameObject;
            else if(pt == PieceType.longblock)
                go = longblocks[visualIndex].gameObject;
            else if(pt == PieceType.jump)
                go = jumps[visualIndex].gameObject;
            else if(pt == PieceType.slide)
                go = slides[visualIndex].gameObject;

            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            pieces.Add(p);
            
        }
        return p;
    }
}
