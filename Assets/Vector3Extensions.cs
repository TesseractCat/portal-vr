using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3Int RoundToInt(this Vector3 v) {
        return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }
    
    public static int IndexOf(this Vector3 a, int n) {
        for (int i = 0; i < 3; i++) {
            if (a[i] == n)
                return i;
        }
        return -1;
    }
    
    public static Vector3 Abs(this Vector3 v) {
        return new Vector3(Mathf.Abs(v.x),Mathf.Abs(v.y),Mathf.Abs(v.z));
    }
}

public static class Vector3IntExtensions {
    public static int IndexOf(this Vector3Int a, int n) {
        for (int i = 0; i < 3; i++) {
            if (a[i] == n)
                return i;
        }
        return -1;
    }
    
    public static Vector2Int MaskToVector2Int(this Vector3Int v, Vector3Int mask) {
        if (mask[0] == 1) {
            return new Vector2Int(v.z, v.y);
        } else if (mask[1] == 1) {
            return new Vector2Int(v.x, v.z);
        } else {
            return new Vector2Int(v.x, v.y);
        }
    }
    
    public static Vector3Int SetValueByMask(this Vector3Int v, Vector3Int mask, int val) {
        v[mask.IndexOf(1)] = val;
        return v;
    }
    
    public static Vector3Int Abs(this Vector3Int v) {
        return new Vector3Int(Mathf.Abs(v.x),Mathf.Abs(v.y),Mathf.Abs(v.z));
    }
}

public static class Vector2IntExtensions {
    public static Vector2Int Abs(this Vector2Int v) {
        return new Vector2Int(Mathf.Abs(v.x),Mathf.Abs(v.y));
    }
    
    public static Vector3Int MaskToVector3Int(this Vector2Int v, Vector3Int mask) {
        if (mask[0] == 1) {
            return new Vector3Int(0, v.x, v.y);
        } else if (mask[1] == 1) {
            return new Vector3Int(v.x, 0, v.y);
        } else {
            return new Vector3Int(v.x, v.y, 0);
        }
    }
    public static Vector3Int MaskToVector3Int(this Vector2Int v, Vector3Int mask, int val) {
        if (mask[0] == 1) {
            return new Vector3Int(val, v.x, v.y);
        } else if (mask[1] == 1) {
            return new Vector3Int(v.x, val, v.y);
        } else {
            return new Vector3Int(v.x, v.y, val);
        }
    }
}

//Serializable Types
public class JsonVector3 {
    public float x;
    public float y;
    public float z;
    
    public JsonVector3() { }
    
    public JsonVector3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public JsonVector3(Vector3 v) {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    
    public static implicit operator Vector3(JsonVector3 v) => new Vector3(v.x, v.y, v.z);
    public static implicit operator JsonVector3(Vector3 v) => new JsonVector3(v.x, v.y, v.z);
}

public class JsonVector3Int {
    public int x;
    public int y;
    public int z;
    
    public JsonVector3Int() { }
    
    public JsonVector3Int(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public JsonVector3Int(Vector3Int v) {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    
    public static implicit operator Vector3Int(JsonVector3Int v) => new Vector3Int(v.x, v.y, v.z);
    public static implicit operator JsonVector3Int(Vector3Int v) => new JsonVector3Int(v.x, v.y, v.z);
}

public class JsonQuaternion {
    public float x;
    public float y;
    public float z;
    public float w;
    
    public JsonQuaternion() { }
    
    public JsonQuaternion(float x, float y, float z, float w) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    
    public JsonQuaternion(Quaternion v) {
        x = v.x;
        y = v.y;
        z = v.z;
        w = v.w;
    }
    
    public static implicit operator Quaternion(JsonQuaternion v) => new Quaternion(v.x, v.y, v.z, v.w);
    public static implicit operator JsonQuaternion(Quaternion v) => new JsonQuaternion(v.x, v.y, v.z, v.w);
}
