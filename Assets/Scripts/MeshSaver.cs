using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SerializableVec3 {
    public float xValue;
    public float yValue;
    public float zValue;

    public SerializableVec3 (Vector3 vector) {
        xValue = vector.x;
        yValue = vector.y;
        zValue = vector.z;
    }

    public Vector3 getVec() {
        return new Vector3(xValue, yValue, zValue);
    }
}

[System.Serializable]
public class SerializableVec2 {
    public float xValue;
    public float yValue;

    public SerializableVec2(Vector2 vector) {
        xValue = vector.x;
        yValue = vector.y;
    }

    public Vector3 getVec() {
        return new Vector2(xValue, yValue);
    }
}

[System.Serializable]
public class SerializableQuat {
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializableQuat(Quaternion quaternion) {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion GetQuaternion() {
        return new Quaternion(x, y, z, w);
    }
}

[System.Serializable]
public class MeshData {
    public SerializableVec3[] verts;
    public int[] tris;
    public SerializableVec2[] uvs;
    public SerializableVec3[] norms;
    public SerializableQuat rotation;
    public SerializableVec3 scale;
    public bool constructed = false;

    public MeshData (GameObject meshObject) {
        MeshFilter filter = meshObject.GetComponent<MeshFilter>();
        if (!filter) {
            return;
        }
        Mesh mesh = filter.mesh;
        if (!mesh) {
            return;
        }
        verts = new SerializableVec3[mesh.vertices.Length];
        tris = new int[mesh.triangles.Length];
        uvs = new SerializableVec2[mesh.uv.Length];
        norms = new SerializableVec3[mesh.normals.Length];
        for(int i = 0; i < verts.Length; i++) {
            verts[i] = new SerializableVec3(mesh.vertices[i]);
        }
        tris = mesh.triangles;
        for(int i = 0; i < uvs.Length; i++) {
            uvs[i] = new SerializableVec2(mesh.uv[i]);
        }
        for(int i = 0; i < norms.Length; i++) {
            norms[i] = new SerializableVec3(mesh.normals[i]);
        }
        rotation = new SerializableQuat(meshObject.transform.rotation);
        scale = new SerializableVec3(meshObject.transform.localScale);
        constructed = true;
    }

    public void GetMesh(GameObject gameO) {
        if(!gameO.GetComponent<MeshFilter>()) {
            gameO.AddComponent<MeshFilter>();
        }
        if (!gameO.GetComponent<MeshRenderer>()) {
            gameO.AddComponent<MeshRenderer>();
        }

        Mesh mesh = new Mesh();
        Vector3[] meshVerts = new Vector3[verts.Length];
        Vector2[] meshUVs = new Vector2[uvs.Length];
        Vector3[] meshNorms = new Vector3[norms.Length];
        for(int i = 0; i < verts.Length; i++) {
            meshVerts[i] = verts[i].getVec();
        }
        for(int i = 0; i < uvs.Length; i++) {
            meshUVs[i] = uvs[i].getVec();
        }
        for(int i = 0; i < norms.Length; i++) {
            meshNorms[i] = norms[i].getVec();
        }
        mesh.vertices = meshVerts;
        mesh.uv = meshUVs;
        mesh.triangles = tris;
        mesh.normals = meshNorms;
        gameO.GetComponent<MeshFilter>().mesh = mesh;
        gameO.transform.rotation = rotation.GetQuaternion();
        gameO.transform.localScale = scale.getVec();
    }
}

public class MeshSaver
{
    private const string meshFileType = ".msh";
    private const string textureFileType = ".png";

    public static void SaveMesh(GameObject go, string filename) {
        MeshData meshData = new MeshData(go);
        if (!meshData.constructed) {
            return;
        }
        string filePathAndName = Application.persistentDataPath + "\\" + filename + meshFileType;
        using (Stream stream = File.Open(filePathAndName, FileMode.Create)) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, meshData);
        }
        Texture2D texture = (Texture2D)go.GetComponent<MeshRenderer>().material.mainTexture;
        string texFilePathAndName = Application.persistentDataPath + "\\" + filename + textureFileType;
        File.WriteAllBytes(texFilePathAndName, texture.EncodeToPNG());
    }

    public static void LoadMesh(GameObject gameO, string filename) {
        string filePathAndName = Application.persistentDataPath + "\\" + filename + meshFileType;
        MeshData meshData;
        using (Stream stream = File.Open(filePathAndName, FileMode.Open)) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            meshData = (MeshData)binaryFormatter.Deserialize(stream);
        }
        meshData.GetMesh(gameO);
        byte[] bytes;
        string texFilePathAndName = Application.persistentDataPath + "\\" + filename + textureFileType;
        if (File.Exists(texFilePathAndName)) {
            bytes = File.ReadAllBytes(texFilePathAndName);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            gameO.GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
    }

    public static bool IsFileLocked(string filename) {
        Stream stream = null;
        FileInfo file = new FileInfo(Application.persistentDataPath + "\\" + filename + textureFileType);
        try {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        } catch (IOException e) {
            Debug.Log(e.Message);
            return true;
        } finally {
            if(stream != null) {
                stream.Close();
            }
        }
        return false;
    }
}
