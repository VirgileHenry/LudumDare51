using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    PlanetTile[] tiles;
    public Material planetMaterial; 

    // creates a new planet
    public void Start()
    {
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
            tile.GenerateTriangle(planetMaterial);
        }
    }

    public void Update() {

    }

    public void UpdateTilesStates(int newFallingTileNumber) {
        List<PlanetTile> alive = new List<PlanetTile>();
        foreach(PlanetTile tile in tiles) {
            switch (tile.state)
            {
                case TileState.Alive:
                    alive.Add(tile);
                    break;
                case TileState.Shaking:
                    tile.Collapse();
                    break;
                case TileState.Collapsed:
                    break;
            }
        }
    }

    private IEnumerator CollapseAnimation(PlanetTile tile, float duration) {
        float timer = 0;
        Vector3 oldPos = tile.tileObject.transform.position;
        Vector3 targetPos = (tile.coordinates.v1 + tile.coordinates.v1 + tile.coordinates.v1).normalized * 0.5f;
        while(timer < duration) {
            tile.tileObject.transform.position = oldPos + (timer / duration) * (targetPos - oldPos);

            duration += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
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

// a tile of a planet
public class PlanetTile
{
    public TileState state;
    public Tilecoordinates coordinates;
    public GameObject tileObject;

    public PlanetTile(Tilecoordinates coordinates) {
        this.coordinates = coordinates;
        this.state = TileState.Alive;
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

    public void GenerateTriangle(Material material) {
        tileObject = new GameObject();
        tileObject.name = "debug triangle";
        MeshFilter filter = tileObject.AddComponent<MeshFilter>();
        tileObject.AddComponent<MeshRenderer>().material = material;
        Mesh mesh = new Mesh();
        Vector3 normal = (coordinates.v1 + coordinates.v2 + coordinates.v3).normalized;
        mesh.vertices = new Vector3[6] {
            coordinates.v1,
            coordinates.v2,
            coordinates.v3,
            coordinates.v1 * 0.5f,
            coordinates.v2 * 0.5f,
            coordinates.v3 * 0.5f,
        };
        mesh.normals = new Vector3[6] {
            normal,
            normal,
            normal,
            normal,
            normal,
            normal,
        };
        mesh.triangles = new int[21] {
            0, 1, 2,
            0, 3, 1,
            3, 4, 1,
            2, 1, 4,
            2, 4, 5,
            0, 2, 3,
            2, 5, 3
        };
        filter.mesh = mesh;
    }

    public void Collapse() {
        state = TileState.Collapsed;
    }

    public void Update() {

    }


}