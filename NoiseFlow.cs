using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlow : MonoBehaviour
{
    FastNoise fastNoise;
    public Vector3Int gridSize;
    public float increment;
    public float cellSize;
    public Vector3 offset,offsetSpeed;
    public Vector3[,,] flowFieldDirection;
    // Start is called before the first frame update


    public GameObject particlePrefab;
    public int  maxParticles;
    public List<FlowFieldParticle> particles; 
    public List<MeshRenderer> particleMeshRenderer;
    public float particleScale;
    public float spawnRadius;

    public float particleMoveSpeed, particleRotateSpeed;
    bool  validSpawnPos(Vector3 position)
    {
        bool valid = true;
        foreach(FlowFieldParticle particle in particles)
        {

            if(Vector3.Distance(position, particle.transform.position) < spawnRadius)
            {
                valid = false;
                break;
            }
        }
        if(valid)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    void Awake()
    {
        flowFieldDirection = new Vector3[gridSize.x,gridSize.y,gridSize.z];
        fastNoise = new FastNoise();
        particles = new List<FlowFieldParticle>();
        particleMeshRenderer = new List<MeshRenderer>(); 

        for(int i = 0; i < maxParticles; i++)
        {
            int spawnAttempt = 0;

            while(spawnAttempt < 100)
            {

            
                Vector3 randPos = new Vector3(
                    Random.Range(this.transform.position.x, this.transform.position.x + gridSize.x * cellSize),
                    Random.Range(this.transform.position.y, this.transform.position.y + gridSize.y * cellSize),
                    Random.Range(this.transform.position.z, this.transform.position.z + gridSize.z * cellSize)
                );
                bool isValid = validSpawnPos(randPos);

                if(isValid)
                {
                    GameObject particleInstance = (GameObject)Instantiate(particlePrefab);
                    particleInstance.transform.position = randPos;
                    particleInstance.transform.parent = this.transform;
                    particleInstance.transform.localScale = new Vector3(particleScale, particleScale,particleScale);
                    particles.Add(particleInstance.GetComponent<FlowFieldParticle>());
                    particleMeshRenderer.Add(particleInstance.GetComponent<MeshRenderer>());
                    break;
                }
                if(!isValid)
                {
                    spawnAttempt++;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        calcFlowField();
        ParticleBehavior();
        
    }

    void calcFlowField()
    {
        offset = new Vector3(offset.x + (offsetSpeed.x* Time.deltaTime), offset.y + (offsetSpeed.y * Time.deltaTime), offset.z + (offsetSpeed.z * Time.deltaTime));

        float xTemp = 0f;

        for(int x = 0; x < gridSize.x; x++)
        {
            float yTemp = 0f;

            for(int y = 0; y < gridSize.y; y++)
            {
                float zTemp = 0f;

                for(int z = 0; z < gridSize.z; z++)
                {
                    float noise = fastNoise.GetSimplex(xTemp+offset.x, yTemp+offset.y, zTemp+offset.z) + 1;
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
                    
                    flowFieldDirection[x,y,z] = Vector3.Normalize(noiseDirection);
                    


                    zTemp += increment;
                }

                yTemp += increment;

            }

            xTemp += increment;
        }
    }

    void ParticleBehavior()
    {
        foreach(FlowFieldParticle p in particles)
        {
            // X 
            if(p.transform.position.x > this.transform.position.x + (gridSize.x * cellSize))
            {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            if(p.transform.position.x < this.transform.position.x)
            {
                p.transform.position = new Vector3((this.transform.position.x + (gridSize.x * cellSize)), p.transform.position.y, p.transform.position.z);
            }

            // Y 
            if(p.transform.position.y > this.transform.position.y + (gridSize.y * cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y, p.transform.position.z);
            }
            if(p.transform.position.y < this.transform.position.y)
            {
                p.transform.position = new Vector3(p.transform.position.x, (this.transform.position.y + (gridSize.y * cellSize)), p.transform.position.z);
            }

            // Z 
            if(p.transform.position.z > this.transform.position.z + (gridSize.z * cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, this.transform.position.z);
            }
            if(p.transform.position.z < this.transform.position.z)
            {
                p.transform.position = new Vector3(p.transform.position.x, p.transform.position.y, (this.transform.position.z + (gridSize.z * cellSize)));
            }


            Vector3Int particlePosition = new Vector3Int(
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.x - this.transform.position.x) / cellSize, 0, gridSize.x-1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.y - this.transform.position.y) / cellSize, 0, gridSize.y-1)),
                Mathf.FloorToInt(Mathf.Clamp((p.transform.position.z - this.transform.position.z) / cellSize, 0, gridSize.z-1))
            );
            p.ApplyRotation(flowFieldDirection[particlePosition.x, particlePosition.y, particlePosition.z], particleRotateSpeed);
            p.moveSpeed = particleMoveSpeed;
            //p.transform.localScale = new Vector3(particleScale, particleScale, particleScale);
        }
    }
    private void OnDrawGizmos()
    {
       Gizmos.color = Color.white;
       Gizmos.DrawWireCube(this.transform.position + new Vector3((gridSize.x * cellSize)*0.5f,(gridSize.y * cellSize)*0.5f,(gridSize.z * cellSize)*0.5f),
                            new Vector3(gridSize.x * cellSize, gridSize.y * cellSize, gridSize.z * cellSize));
    }
   
}
