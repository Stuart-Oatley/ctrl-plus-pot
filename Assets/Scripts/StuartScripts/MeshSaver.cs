using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// class to allow serialisation of Vector3
/// </summary>
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

/// <summary>
/// class to allow serialisation of vector 2
/// </summary>
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

/// <summary>
/// class to allow serialisation of quaternion
/// </summary>
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

/// <summary>
/// Class to allow serialisation of model data
/// </summary>
[System.Serializable]
public class MeshData {
    public SerializableVec3[] verts;
    public int[] tris;
    public SerializableVec2[] uvs;
    public SerializableVec3[] norms;
    public SerializableQuat rotation;
    public SerializableVec3 scale;
    public bool constructed = false;

    /// <summary>
    /// Constructor that creates Meshdata from provided gameobject
    /// </summary>
    /// <param name="meshObject"></param>
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

    /// <summary>
    /// Adds the model to the provided gameobject
    /// </summary>
    /// <param name="gameO"></param>
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

/// <summary>
/// class to save and load models
/// </summary>
public class MeshSaver
{
    private const string meshFileType = ".msh";
    private const string textureFileType = ".png";

    /// <summary>
    /// Saves the model and texture from the gameobject provided
    /// </summary>
    /// <param name="go"></param>
    /// <param name="filename"></param>
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
        Debug.Log(go.GetComponent<MeshRenderer>().material.mainTexture.GetType());
        Texture2D texture;
        if(go.GetComponent<MeshRenderer>().material.mainTexture.GetType() == typeof(RenderTexture)) {
            RenderTexture renderTexture = (RenderTexture)go.GetComponent<MeshRenderer>().material.mainTexture;
            RenderTexture.active = renderTexture;
            texture = new Texture2D(renderTexture.width, renderTexture.height);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
        } else {
            texture = (Texture2D)go.GetComponent<MeshRenderer>().material.mainTexture;
        }
        string texFilePathAndName = Application.persistentDataPath + "\\" + filename + textureFileType;
        File.WriteAllBytes(texFilePathAndName, texture.EncodeToPNG());
    }

    /// <summary>
    /// loads model and texture onto provided gameobject 
    /// </summary>
    /// <param name="gameO"></param>
    /// <param name="filename"></param>
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

    /// <summary>
    /// checks whether the file is locked. Uses texture as that's the larger file, so most likely to still be saving
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
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
