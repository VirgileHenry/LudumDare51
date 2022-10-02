using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    PlanetTile[] tiles;
    public Material planetMaterial;
    public Material undergroundMaterial;
    // prefabs
    public GameObject rockPrefab;
    public GameObject geyserPrefab;
    public GameObject[] spaceshipPrefabs;

    public static Planet instance;

    public Transform mainParent;

    private void Awake()
    {
        if(instance == null) {
            instance = this;
        }
    }

    // creates a new planet
    public void Start()
    {
        GeneratePlanet();
    }

    public void ResetPlanet() {
        Destroy(mainParent.gameObject); // clear everything
        GeneratePlanet();
    }

    private void GeneratePlanet() {
        mainParent = new GameObject().transform;

        Vector3[] icoVerts = new Vector3[12] {
            Vector3.up,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            Vector3.zero,
            -Vector3.up
        };
        float longitude = 0f; // phi
        float latitude = Mathf.Atan(0.5f); // theta
        for(int i = 1; i < 11; i++) {
            icoVerts[i] = new Vector3(Mathf.Cos(latitude) * Mathf.Cos(longitude), Mathf.Sin(latitude), Mathf.Cos(latitude) * Mathf.Sin(longitude));
            longitude += 36 * Mathf.PI / 180;
            latitude = -latitude;
        }

        tiles = new PlanetTile[20] {
            new PlanetTile(new Tilecoordinates(icoVerts[0], icoVerts[3], icoVerts[1])),
            new PlanetTile(new Tilecoordinates(icoVerts[0], icoVerts[5], icoVerts[3])),
            new PlanetTile(new Tilecoordinates(icoVerts[0], icoVerts[7], icoVerts[5])),
            new PlanetTile(new Tilecoordinates(icoVerts[0], icoVerts[9], icoVerts[7])),
            new PlanetTile(new Tilecoordinates(icoVerts[0], icoVerts[1], icoVerts[9])),
            new PlanetTile(new Tilecoordinates(icoVerts[1], icoVerts[3], icoVerts[2])),
            new PlanetTile(new Tilecoordinates(icoVerts[2], icoVerts[3], icoVerts[4])),
            new PlanetTile(new Tilecoordinates(icoVerts[3], icoVerts[5], icoVerts[4])),
            new PlanetTile(new Tilecoordinates(icoVerts[4], icoVerts[5], icoVerts[6])),
            new PlanetTile(new Tilecoordinates(icoVerts[5], icoVerts[7], icoVerts[6])),
            new PlanetTile(new Tilecoordinates(icoVerts[6], icoVerts[7], icoVerts[8])),
            new PlanetTile(new Tilecoordinates(icoVerts[7], icoVerts[9], icoVerts[8])),
            new PlanetTile(new Tilecoordinates(icoVerts[8], icoVerts[9], icoVerts[10])),
            new PlanetTile(new Tilecoordinates(icoVerts[9], icoVerts[1], icoVerts[10])),
            new PlanetTile(new Tilecoordinates(icoVerts[10], icoVerts[1], icoVerts[2])),
            new PlanetTile(new Tilecoordinates(icoVerts[11], icoVerts[2], icoVerts[4])),
            new PlanetTile(new Tilecoordinates(icoVerts[11], icoVerts[4], icoVerts[6])),
            new PlanetTile(new Tilecoordinates(icoVerts[11], icoVerts[6], icoVerts[8])),
            new PlanetTile(new Tilecoordinates(icoVerts[11], icoVerts[8], icoVerts[10])),
            new PlanetTile(new Tilecoordinates(icoVerts[11], icoVerts[10], icoVerts[2])),
        };

        // subdivide once
        for(int i = 0; i < 2; i++) {
            List<PlanetTile> newTiles = new List<PlanetTile>();
            foreach(PlanetTile tile in tiles) {
                newTiles.AddRange(tile.Subdivide());
            }
            tiles = newTiles.ToArray();
        }
        
        foreach(PlanetTile tile in tiles) {
            tile.GenerateTile(planetMaterial, undergroundMaterial, mainParent);
        }

        // generate the spaceship !
        foreach (GameObject spaceshipPartPrefab in spaceshipPrefabs)
        {
            GameObject spaceshipPart = Instantiate(spaceshipPartPrefab, mainParent);
            PlanetTile tile = tiles[Random.Range(0, tiles.Length)];
            while(tile.tileObj != TileGameObject.None) {
                tile = tiles[Random.Range(0, tiles.Length)];
            }
            tile.tileObj = TileGameObject.Spaceship;
            Vector3 center = ((tile.coordinates.v1 + tile.coordinates.v2 + tile.coordinates.v3) / 3f) * 1.005f; // slightly elevated
            float a = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1-a);
            float c = 1 - a - b;
            Vector3 randomForward = (a * tile.coordinates.v1 + b * tile.coordinates.v2 + c * tile.coordinates.v3) - center;
            Vector3 up = (tile.coordinates.v1 + tile.coordinates.v2 + tile.coordinates.v3).normalized;
            spaceshipPart.transform.position = center;
            spaceshipPart.transform.rotation = Quaternion.LookRotation(randomForward, up);

        }

    }

    public void UpdateTilesStates() {
        List<PlanetTile> alive = new List<PlanetTile>();
        foreach(PlanetTile tile in tiles) {
            switch (tile.state)
            {
                case TileState.Alive:
                    if(tile.tileObj == TileGameObject.None) {
                        alive.Add(tile);
                    }
                    break;
                case TileState.Shaking:
                    tile.Collapse();
                    break;
                case TileState.Collapsed:
                    break;
            }
            // find new tiles to shake
        }

        for(int i = 0; i < 6; i++) {
            if(alive.Count == 0) {
                break; // nothing to shake !
            }
            int indexToShake = Random.Range(0, alive.Count);
            alive[indexToShake].Shake();
            alive.RemoveAt(indexToShake);
        }
    }

    public static GameObject InstantiateGameObject(GameObject prefab) {
        return Instantiate(prefab);
    }
}

// a position of the tile is represented by 3 corner vertices
public struct Tilecoordinates
{
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;

    public Tilecoordinates(Vector3 v1, Vector3 v2, Vector3 v3) {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }
}

// the state of a tile
public enum TileState {
    Alive,
    Shaking,
    Collapsed,
}

public enum TileGameObject {
    None,
    Geyser,
    Spaceship,
}

// a tile of a planet
public class PlanetTile
{
    public TileState state;
    public Tilecoordinates coordinates;
    public GameObject tileGameObject;
    public TileGameObject tileObj;

    public PlanetTile(Tilecoordinates coordinates) {
        this.coordinates = coordinates;
        this.state = TileState.Alive;
        this.tileObj = Random.Range(0f, 1f) < 0.05f ? TileGameObject.Geyser : TileGameObject.None;
    }

    // divide the tile in 4 smaller tiles and returns them
    public PlanetTile[] Subdivide() {
        Vector3 v12 = (coordinates.v1 + coordinates.v2).normalized;
        Vector3 v13 = (coordinates.v1 + coordinates.v3).normalized;
        Vector3 v23 = (coordinates.v2 + coordinates.v3).normalized;

        return new PlanetTile[4] {
            new PlanetTile(new Tilecoordinates(coordinates.v1, v12, v13)),
            new PlanetTile(new Tilecoordinates(coordinates.v2, v23, v12)),
            new PlanetTile(new Tilecoordinates(coordinates.v3, v13, v23)),
            new PlanetTile(new Tilecoordinates(v12, v23, v13))
        };
    }

    public string toString() {
        return $"{coordinates.v1} {coordinates.v2} {coordinates.v3}";
    }

    public void GenerateTile(Material material, Material groundMaterial, Transform parent) {
        tileGameObject = new GameObject();
        tileGameObject.transform.SetParent(parent);
        tileGameObject.name = "Planet Tile";
        MeshFilter filter = tileGameObject.AddComponent<MeshFilter>();
        tileGameObject.AddComponent<MeshRenderer>().material = material;
        Vector3 normal = (coordinates.v1 + coordinates.v2 + coordinates.v3).normalized;
        tileGameObject.AddComponent<TileScript>().SetNormal(normal);
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[3] {
            coordinates.v1,
            coordinates.v2,
            coordinates.v3,
        };
        mesh.normals = new Vector3[3] {
            (coordinates.v1).normalized,
            (coordinates.v2).normalized,
            (coordinates.v3).normalized,
        };
        mesh.triangles = new int[3] {
            0, 1, 2,
        };
        filter.mesh = mesh;
        tileGameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        tileGameObject.layer = 9;
        // different mesh for the ground
        GameObject groundObj = new GameObject();
        groundObj.transform.SetParent(tileGameObject.transform);
        Mesh groundMesh = new Mesh();
        groundMesh.vertices = new Vector3[6] {
            coordinates.v1,
            coordinates.v2,
            coordinates.v3,
            coordinates.v1 * 0.5f,
            coordinates.v2 * 0.5f,
            coordinates.v3 * 0.5f,
        };
        groundMesh.triangles = new int[18] {
            0, 3, 1,
            3, 4, 1,
            2, 1, 4,
            2, 4, 5,
            0, 2, 3,
            2, 5, 3
        };
        groundObj.AddComponent<MeshFilter>().mesh = groundMesh;
        groundObj.AddComponent<MeshRenderer>().material = groundMaterial;
        // add artifacts rocks
        int rockNumber = new int[8]{0, 1, 1, 2, 2, 2, 4, 4}[Random.Range(0, 8)];

        for(int i = 0; i < rockNumber; i++) {
            GameObject newRock = Planet.InstantiateGameObject(Planet.instance.rockPrefab);
            // random position
            float a = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1-a);
            float c = 1 - a - b;
            newRock.transform.position = a * coordinates.v1 + b * coordinates.v2 + c * coordinates.v3;
            a = Random.Range(0f, 1f);
            b = Random.Range(0f, 1-a);
            c = 1 - a - b;
            Vector3 randomForward = (a * coordinates.v1 + b * coordinates.v2 + c * coordinates.v3) - newRock.transform.position;
            Vector3 up = (coordinates.v1 +coordinates.v2 + coordinates.v3).normalized;
            newRock.transform.rotation = Quaternion.LookRotation(randomForward, up);
            float x = Random.Range(0.5f, 1f);
            newRock.transform.localScale = Vector3.one * x * x;
            newRock.transform.SetParent(tileGameObject.transform, true);
        }

        if(this.tileObj == TileGameObject.Geyser) {
            // generate a geyser
            GameObject geyser = Planet.InstantiateGameObject(Planet.instance.geyserPrefab);
            Vector3 center = (coordinates.v1 +coordinates.v2 +coordinates.v3) / 3f;
            float a = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1-a);
            float c = 1 - a - b;
            Vector3 randomForward = (a * coordinates.v1 + b * coordinates.v2 + c * coordinates.v3) - center;
            Vector3 up = (coordinates.v1 + coordinates.v2 + coordinates.v3).normalized;
            geyser.transform.position = center;
            geyser.transform.rotation = Quaternion.LookRotation(randomForward, up);
            geyser.transform.SetParent(tileGameObject.transform, true);
        }
    }

    public void Collapse() {
        state = TileState.Collapsed;
        tileGameObject.GetComponent<TileScript>().Collapse();
    }

    public void Shake() {
        state = TileState.Shaking;
        tileGameObject.GetComponent<TileScript>().Shake();
    }


}