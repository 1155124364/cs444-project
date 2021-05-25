/*
    Slicer.cs
    Description: Control the cutting function in the game.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Slicer : MonoBehaviour {
    float old_time = 0;
    float time_diff = 0;

    struct Triangle {
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 v3;

        public Vector3 getNormal() {
            return Vector3.Cross(v1 - v2, v1 - v3).normalized;
        }

        // Conver direction to point in the direction of the triangle
        public void matchDirection(Vector3 direction) {
            if (Vector3.Dot(getNormal(), direction) > 0) {
                return;
            }
            else {
                Vector3 vec = v1;
                v1 = v3;
                v3 = vec;
            }
        }
    }


    // When there is a collision
    void OnCollisionEnter(Collision other) {
        time_diff = Time.time - old_time;
        if (time_diff >= 1 && other.gameObject.tag == "Slicable") {
            slice(other);
            old_time = Time.time;
        }
    }

    void slice(Collision other) {
        Collider coll = GetComponent<Collider>();
        Debug.LogWarningFormat("other positio: " + other.transform.position);
        Debug.LogWarningFormat("coll positio: " + coll.transform.position);

        // Create cutting plane
        Vector3 vec1 = coll.bounds.center;
        Vector3 vec2 = coll.bounds.center + transform.right * coll.bounds.min.x;
        Vector3 vec3 = coll.bounds.center + transform.up * coll.bounds.max.y;

        Plane plane = new Plane(vec1, vec2, vec3);

        // Transform attached to the rigid body (transform of the body we hit)
        Transform tr = other.transform;

        // Mesh of the object we collided
        Mesh m = other.gameObject.GetComponent<MeshFilter>().mesh;
        int[] triangles = m.triangles;
        Vector3[] verts = m.vertices;

        List<Vector3> intersections = new List<Vector3>();
        List<Triangle> trianglesAbove = new List<Triangle>();
        List<Triangle> trianglesBelow = new List<Triangle>();

        // Loop through triangles of the collided mesh
        for (int i = 0; i < triangles.Length; i += 3) {
            List<Vector3> triangleIntersections = new List<Vector3>();

            // Retrive the vertices of each triangle and transform them from local to world space
            Vector3 v1 = tr.TransformPoint(verts[triangles[i]]);
            Vector3 v2 = tr.TransformPoint(verts[triangles[i + 1]]);
            Vector3 v3 = tr.TransformPoint(verts[triangles[i + 2]]);

            Vector3 norm = Vector3.Cross(v1 - v2, v1 - v3);

            float t;

            // Check if triangles intersect with the plane
            Vector3 direction = v2 - v1;
            if (plane.Raycast(new Ray(v1, direction), out t)) {
                // Check if the interesction is inside the triangle
                if (t <= direction.magnitude) {
                    Vector3 intersection = v1 + t * direction.normalized;
                    intersections.Add(intersection);
                    triangleIntersections.Add(intersection);
                }
            }

            direction = v3 - v1;
            if (plane.Raycast(new Ray(v1, direction), out t)) {
                // Check if the interesction is inside the triangle
                if (t <= direction.magnitude) {
                    Vector3 intersection = v1 + t * direction.normalized;
                    intersections.Add(intersection);
                    triangleIntersections.Add(intersection);
                }
            }

            direction = v3 - v2;
            if (plane.Raycast(new Ray(v2, direction), out t)) {
                // Check if the interesction is inside the triangle
                if (t <= direction.magnitude) {
                    Vector3 intersection = v2 + t * direction.normalized;
                    intersections.Add(intersection);
                    triangleIntersections.Add(intersection);
                }
            }

            // Check if we had an intersection between the plane and the triangle
            if (triangleIntersections.Count > 0) {
                List<Vector3> pointsAbove = new List<Vector3>();
                List<Vector3> pointsBelow = new List<Vector3>();

                // Intersection vertices
                pointsAbove.AddRange(triangleIntersections);
                pointsBelow.AddRange(triangleIntersections);

                // Check if vertex 1 is above or below the plane
                if (plane.GetSide(v1)) {
                    pointsAbove.Add(v1);
                }else {
                    pointsBelow.Add(v1);
                }

                // Check if vertex 2 is above or below the plane
                if (plane.GetSide(v2)) {
                    pointsAbove.Add(v2);
                }else {
                    pointsBelow.Add(v2);
                }

                // Check if vertex 3 is above or below the plane
                if (plane.GetSide(v3)) {
                    pointsAbove.Add(v3);
                }else {
                    pointsBelow.Add(v3);
                }

                // 3 points above the plane
                if (pointsAbove.Count == 3) {
                    Triangle triangle = new Triangle() { v1 = pointsAbove[0], v2 = pointsAbove[1], v3 = pointsAbove[2] };
                    triangle.matchDirection(norm);
                    trianglesAbove.Add(triangle);
                // 4 points above the plane, so I have to split the trapezoid into two triangles
                }else {
                    // First triangle 
                    Triangle triangle = new Triangle() { v1 = pointsAbove[0], v2 = pointsAbove[2], v3 = pointsAbove[3] };
                    triangle.matchDirection(norm);
                    trianglesAbove.Add(triangle);

                    // Second triangle 
                    triangle = new Triangle() { v1 = pointsAbove[0], v2 = pointsAbove[1], v3 = pointsAbove[3] };
                    triangle.matchDirection(norm);
                    trianglesAbove.Add(triangle);
                }
                
                // 3 points below the plane 
                if (pointsBelow.Count == 3) {
                    Triangle triangle = new Triangle() { v1 = pointsBelow[0], v2 = pointsBelow[1], v3 = pointsBelow[2] };
                    triangle.matchDirection(norm);
                    trianglesBelow.Add(triangle);
                }
                // 4 points below the plane, so I have to split the trapezoid into two triangles
                else {
                    // First triangle 
                    Triangle triangle = new Triangle() { v1 = pointsBelow[0], v2 = pointsBelow[2], v3 = pointsBelow[3] };
                    triangle.matchDirection(norm);
                    trianglesBelow.Add(triangle);
                    
                    // Second triangle 
                    triangle = new Triangle() { v1 = pointsBelow[0], v2 = pointsBelow[1], v3 = pointsBelow[3] };
                    triangle.matchDirection(norm);
                    trianglesBelow.Add(triangle);
                }
            }
            // No intersection between the plane and the triangle
            else {
                if (plane.GetSide(v1)) {
                    trianglesAbove.Add(new Triangle() { v1 = v1, v2 = v2, v3 = v3 });
                }
                else {
                    trianglesBelow.Add(new Triangle() { v1 = v1, v2 = v2, v3 = v3 });
                }
            }
        }

        if (intersections.Count > 1) {
            // Compute the center of the hole
            Vector3 center = Vector3.zero;
            
            foreach (Vector3 v in intersections) {
                center += v;
            }
            center /= intersections.Count;

            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;
            Vector3 p3 = Vector3.zero;

            // Create a set of triangles for the above and below mesh to fill the hole created by cutting it
            for (int i = 0; i < intersections.Count; i++) {
                p1 = center;
                p2 = intersections[i];
                p3 = intersections[(i + 1) % intersections.Count];

                Triangle triangleAbove = new Triangle() {v1 = p1, v2 = p2, v3 = p3};
                triangleAbove.matchDirection(-plane.normal);
                trianglesAbove.Add(triangleAbove);

                Triangle triangleBelow = new Triangle() {v1 = p1, v2 = p2, v3 = p3};
                triangleBelow.matchDirection(plane.normal);
                trianglesBelow.Add(triangleBelow);
            }
        }

        // Create the game objects
        if (intersections.Count > 0) {

            List<Vector3> tris = new List<Vector3>();
            List<int> indices = new List<int>();

            // Create the two new meshes for the two parts of the cutted object
            Mesh meshAbove = new Mesh();
            Mesh meshBelow = new Mesh();

            int index = 0;
            foreach (Triangle t in trianglesAbove) {
                tris.Add(t.v1 - other.transform.position);
                tris.Add(t.v2 - other.transform.position);
                tris.Add(t.v3 - other.transform.position);
                indices.Add(index++);
                indices.Add(index++);
                indices.Add(index++);
            }

            meshAbove.vertices = tris.ToArray();
            meshAbove.triangles = indices.ToArray();

            index = 0;
            tris.Clear();
            indices.Clear();
            foreach (Triangle t in trianglesBelow) {
                tris.Add(t.v1 - other.transform.position);
                tris.Add(t.v2 - other.transform.position);
                tris.Add(t.v3 - other.transform.position);
                indices.Add(index++);
                indices.Add(index++);
                indices.Add(index++);
            }

            meshBelow.vertices = tris.ToArray();
            meshBelow.triangles = indices.ToArray();

            meshAbove.RecalculateNormals();
            meshAbove.RecalculateBounds();
            meshBelow.RecalculateNormals();
            meshBelow.RecalculateBounds();

            // Create gameObject above the plane
            Material mat = other.gameObject.GetComponent<MeshRenderer>().material;
            CountableItem oldCountableItem = other.gameObject.GetComponent<CountableItem>();
            CountableItem.CountableType oldCountableType = oldCountableItem.countableType;

            GameObject gameObjectAbove = new GameObject();

            gameObjectAbove.AddComponent<MeshFilter>();
            gameObjectAbove.GetComponent<MeshFilter>().mesh = meshAbove;

            gameObjectAbove.AddComponent<MeshRenderer>();
            gameObjectAbove.GetComponent<MeshRenderer>().material = mat;

            gameObjectAbove.AddComponent<MeshCollider>();
            gameObjectAbove.GetComponent<MeshCollider>().convex = true;
            gameObjectAbove.GetComponent<MeshCollider>().sharedMesh = meshAbove;

            gameObjectAbove.AddComponent<Rigidbody>();

            // Example: gameObjectAbove.AddComponent<ObjectAnchor>();

            gameObjectAbove.AddComponent<CountableItem>();
            gameObjectAbove.GetComponent<CountableItem>().countableType = oldCountableType;

            // Create gameObject above below the plane
            GameObject gameObjectBelow = new GameObject();

            gameObjectBelow.AddComponent<MeshFilter>();
            gameObjectBelow.GetComponent<MeshFilter>().mesh = meshBelow;

            gameObjectBelow.AddComponent<MeshRenderer>();
            gameObjectBelow.GetComponent<MeshRenderer>().material = mat;

            gameObjectBelow.AddComponent<MeshCollider>();
            gameObjectBelow.GetComponent<MeshCollider>().convex = true;
            gameObjectBelow.GetComponent<MeshCollider>().sharedMesh = meshBelow;

            gameObjectBelow.AddComponent<Rigidbody>();

            gameObjectBelow.AddComponent<CountableItem>();
            gameObjectBelow.GetComponent<CountableItem>().countableType = oldCountableType;

            gameObjectAbove.tag = "Slicable";
            gameObjectBelow.tag = "Slicable";

            gameObjectAbove.transform.position = other.transform.position;
            gameObjectBelow.transform.position = other.transform.position;
            
            Destroy(other.gameObject);
        }
    }
}
