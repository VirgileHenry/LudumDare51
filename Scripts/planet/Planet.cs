using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet
{
    PlanetTile[] tiles;

    // creates a new planet
    public Planet(int depth, Material material)
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
        float lattitude = Mathf.Atan(0.5f); // theta
        for(int i = 1; i < 11; i++) {
            icoVerts[i] = new Vector3(Mathf.Cos(lattitude) * Mathf.Cos(longitude), Mathf.Sin(lattitude), Mathf.Cos(lattitude) * Mathf.Sin(longitude));
            longitude += 36 * Mathf.PI / 180;
            lattitude = -lattitude;
        }

        tiles = new PlanetTile[20] {
            new PlanetTile(new TileCoordonates(icoVerts[0], icoVerts[3], icoVerts[1])),
            new PlanetTile(new TileCoordonates(icoVerts[0], icoVerts[5], icoVerts[3])),
            new PlanetTile(new TileCoordonates(icoVerts[0], icoVerts[7], icoVerts[5])),
            new PlanetTile(new TileCoordonates(icoVerts[0], icoVerts[9], icoVerts[7])),
            new PlanetTile(new TileCoordonates(icoVerts[0], icoVerts[1], icoVerts[9])),
            new PlanetTile(new TileCoordonates(icoVerts[1], icoVerts[3], icoVerts[2])),
            new PlanetTile(new TileCoordonates(icoVerts[2], icoVerts[3], icoVerts[4])),
            new PlanetTile(new TileCoordonates(icoVerts[3], icoVerts[5], icoVerts[4])),
            new PlanetTile(new TileCoordonates(icoVerts[4], icoVerts[5], icoVerts[6])),
            new PlanetTile(new TileCoordonates(icoVerts[5], icoVerts[7], icoVerts[6])),
            new PlanetTile(new TileCoordonates(icoVerts[6], icoVerts[7], icoVerts[8])),
            new PlanetTile(new TileCoordonates(icoVerts[7], icoVerts[9], icoVerts[8])),
            new PlanetTile(new TileCoordonates(icoVerts[8], icoVerts[9], icoVerts[10])),
            new PlanetTile(new TileCoordonates(icoVerts[9], icoVerts[1], icoVerts[10])),
            new PlanetTile(new TileCoordonates(icoVerts[10], icoVerts[1], icoVerts[2])),
            new PlanetTile(new TileCoordonates(icoVerts[11], icoVerts[2], icoVerts[4])),
            new PlanetTile(new TileCoordonates(icoVerts[11], icoVerts[4], icoVerts[6])),
            new PlanetTile(new TileCoordonates(icoVerts[11], icoVerts[6], icoVerts[8])),
            new PlanetTile(new TileCoordonates(icoVerts[11], icoVerts[8], icoVerts[10])),
            new PlanetTile(new TileCoordonates(icoVerts[11], icoVerts[10], icoVerts[2])),
        };

        for(int i = 0; i < depth; i++) {
            List<PlanetTile> newTiles = new List<PlanetTile>();
            foreach(PlanetTile tile in tiles) {
                newTiles.AddRange(tile.Subdivide());
            }
            tiles = newTiles.ToArray();
        }
        
        foreach(PlanetTile tile in tiles) {
            tile.GenerateDebugTriangle(material);
        }
    }
}

// a position of the tile is represented by 3 corner vertices
public struct TileCoordonates
{
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;

    public TileCoordonates(Vector3 v1, Vector3 v2, Vector3 v3) {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }
}

// the state of a tile
enum TileState {
    Alive,
    Shaking,
    Collapsed,
}

// a tile of a planet
public class PlanetTile
{
    private TileState state;
    private TileCoordonates coordonates;

    public PlanetTile(TileCoordonates coordonates) {
        this.coordonates = coordonates;
        this.state = TileState.Alive;
    }

    // divide the tile in 4 smaller tiles and resturns them
    public PlanetTile[] Subdivide() {
        Vector3 v12 = (coordonates.v1 + coordonates.v2).normalized;
        Vector3 v13 = (coordonates.v1 + coordonates.v3).normalized;
        Vector3 v23 = (coordonates.v2 + coordonates.v3).normalized;

        return new PlanetTile[4] {
            new PlanetTile(new TileCoordonates(coordonates.v1, v12, v13)),
            new PlanetTile(new TileCoordonates(coordonates.v2, v23, v12)),
            new PlanetTile(new TileCoordonates(coordonates.v3, v13, v23)),
            new PlanetTile(new TileCoordonates(v12, v23, v13))
        };
    }

    public string toString() {
        return $"{coordonates.v1} {coordonates.v2} {coordonates.v3}";
    }

    public void GenerateDebugTriangle(Material material) {
        GameObject go = new GameObject();
        go.name = "debug triangle";
        MeshFilter filter = go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>().material = material;
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[3] {
            coordonates.v1,
            coordonates.v2,
            coordonates.v3,
        };
        mesh.triangles = new int[3] {
            0, 1, 2
        };
        filter.mesh = mesh;
    }

}