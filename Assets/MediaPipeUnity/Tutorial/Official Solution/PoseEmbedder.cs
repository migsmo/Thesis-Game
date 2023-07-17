using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaPipeUnity.Tutorial.Official_Solution;
using Unity.VisualScripting;
using UnityEngine;

namespace MediaPipeUnity.Tutorial.Official_Solution
{
    public class PoseEmbedder
    {
        // Multiplier to apply to the torso to get minimal body size.
        private float _torsoSizeMultiplier;
    
        // Names of the landmarks as they appear in the prediction.
        private readonly string[] _landmarkNames = {
            "nose",
            "left_eye_inner", "left_eye", "left_eye_outer",
            "right_eye_inner", "right_eye", "right_eye_outer",
            "left_ear", "right_ear",
            "mouth_left", "mouth_right",
            "left_shoulder", "right_shoulder",
            "left_elbow", "right_elbow",
            "left_wrist", "right_wrist",
            "left_pinky_1", "right_pinky_1",
            "left_index_1", "right_index_1",
            "left_thumb_2", "right_thumb_2",
            "left_hip", "right_hip",
            "left_knee", "right_knee",
            "left_ankle", "right_ankle",
            "left_heel", "right_heel",
            "left_foot_index", "right_foot_index",
        };
    
        public PoseEmbedder(float torsoSizeMultiplier = 2.5f)
        {
            _torsoSizeMultiplier = torsoSizeMultiplier;
        }
    
        public Vector3[] Embed(Vector3[] landmarks)
        {
            // Check if the number of landmarks is the same as the expected number.
            if (landmarks.Length != _landmarkNames.Length)
            {
                throw new ArgumentException("Unexpected number of landmarks: " + landmarks.Length);
            }
    
            // Get pose landmarks.
            landmarks = landmarks.ToArray();
            // Normalize landmarks.
            landmarks = NormalizePoseLandmarks(landmarks);
    
            // Get embedding.
            var embedding = GetPoseDistanceEmbedding(landmarks);
    
            return embedding;
        }
    
        private Vector3[] NormalizePoseLandmarks(Vector3[] landmarks)
        {
            // Normalize translation.
            var poseCenter = GetPoseCenter(landmarks);
            landmarks = landmarks.Select(l => l - poseCenter).ToArray();
    
            // Normalize scale.
            var poseSize = GetPoseSize(landmarks, _torsoSizeMultiplier);
            landmarks = landmarks.Select(l => l / poseSize).ToArray();
            // Multiplication by 100 is not required, but makes it easier to debug.
            landmarks = landmarks.Select(l => l * 100f).ToArray();
    
            return landmarks;
        }
    
        private Vector3 GetPoseCenter(Vector3[] landmarks)
        {
            // Calculates pose center as point between hips.
            var leftHip = landmarks[_landmarkNames.ToList().IndexOf("left_hip")];
            var rightHip = landmarks[_landmarkNames.ToList().IndexOf("right_hip")];
            var center = (leftHip + rightHip) * 0.5f;
            return center;
        }
        
        private Vector2 GetPoseCenter(Vector2[] landmarks)
        {
            // Calculates pose center as point between hips.
            var leftHip = landmarks[_landmarkNames.ToList().IndexOf("left_hip")];
            var rightHip = landmarks[_landmarkNames.ToList().IndexOf("right_hip")];
            var center = (leftHip + rightHip) * 0.5f;
            return center;
        }
        
        private float GetPoseSize(Vector3[] landmarks, float torsoSizeMultiplier)
        {
            // This approach uses only 2D landmarks to compute pose size.
            Vector2[] landmarks2D = new Vector2[landmarks.Length];
            for (int i = 0; i < landmarks.Length; i++) {
                landmarks2D[i] = new Vector2(landmarks[i].x, landmarks[i].y);
            }

            // Hips center.
            int leftHipIndex = Array.IndexOf(_landmarkNames, "left_hip");
            int rightHipIndex = Array.IndexOf(_landmarkNames, "right_hip");
            Vector2 leftHip = landmarks2D[leftHipIndex];
            Vector2 rightHip = landmarks2D[rightHipIndex];
            Vector2 hips = (leftHip + rightHip) * 0.5f;

            // Shoulders center.
            int leftShoulderIndex = Array.IndexOf(_landmarkNames, "left_shoulder");
            int rightShoulderIndex = Array.IndexOf(_landmarkNames, "right_shoulder");
            Vector2 leftShoulder = landmarks2D[leftShoulderIndex];
            Vector2 rightShoulder = landmarks2D[rightShoulderIndex];
            Vector2 shoulders = (leftShoulder + rightShoulder) * 0.5f;

            // Torso size as the minimum body size.
            float torsoSize = Vector2.Distance(shoulders, hips);

            // Max dist to pose center.
            Vector2 poseCenter = GetPoseCenter(landmarks2D);
            float[] distances = new float[landmarks.Length];
            for (int i = 0; i < landmarks.Length; i++) {
                distances[i] = Vector2.Distance(landmarks2D[i], poseCenter);
            }
            float maxDist = distances.Max();

            return Mathf.Max(torsoSize * torsoSizeMultiplier, maxDist);
        }
        
        private Vector3[] GetPoseDistanceEmbedding(Vector3[] landmarks) {
            Vector3[] embedding = new Vector3[] {
                // One joint.
                GetDistance(
                    GetAverageByNames(landmarks, "left_hip", "right_hip"),
                    GetAverageByNames(landmarks, "left_shoulder", "right_shoulder")),
        
                GetDistanceByNames(landmarks, "left_shoulder", "left_elbow"),
                GetDistanceByNames(landmarks, "right_shoulder", "right_elbow"),
        
                GetDistanceByNames(landmarks, "left_elbow", "left_wrist"),
                GetDistanceByNames(landmarks, "right_elbow", "right_wrist"),
        
                GetDistanceByNames(landmarks, "left_hip", "left_knee"),
                GetDistanceByNames(landmarks, "right_hip", "right_knee"),
        
                GetDistanceByNames(landmarks, "left_knee", "left_ankle"),
                GetDistanceByNames(landmarks, "right_knee", "right_ankle"),
        
                // Two joints.
                GetDistanceByNames(landmarks, "left_shoulder", "left_wrist"),
                GetDistanceByNames(landmarks, "right_shoulder", "right_wrist"),
        
                GetDistanceByNames(landmarks, "left_hip", "left_ankle"),
                GetDistanceByNames(landmarks, "right_hip", "right_ankle"),
        
                // Four joints.
                GetDistanceByNames(landmarks, "left_hip", "left_wrist"),
                GetDistanceByNames(landmarks, "right_hip", "right_wrist"),
        
                // Five joints.
                GetDistanceByNames(landmarks, "left_shoulder", "left_ankle"),
                GetDistanceByNames(landmarks, "right_shoulder", "right_ankle"),
                
                GetDistanceByNames(landmarks, "left_hip", "left_wrist"),
                GetDistanceByNames(landmarks, "right_hip", "right_wrist"),
        
                // Cross body.
                GetDistanceByNames(landmarks, "left_elbow", "right_elbow"),
                GetDistanceByNames(landmarks, "left_knee", "right_knee"),
        
                GetDistanceByNames(landmarks, "left_wrist", "right_wrist"),
                GetDistanceByNames(landmarks, "left_ankle", "right_ankle"),
        
                // Body bent direction.
                // GetDistance(
                //     GetAverageByNames(landmarks, "left_wrist", "left_ankle"),
                //     landmarks[_landmarkNames.IndexOf("left_hip")]),
                // GetDistance(
                //     GetAverageByNames(landmarks, "right_wrist", "right_ankle"),
                //     landmarks[_landmarkNames.IndexOf("right_hip")])
            };
        
            return embedding;
        }
        
        public Vector3 GetAverageByNames(Vector3[] landmarks, string nameFrom, string nameTo)
        {
            Vector3 lmkFrom = landmarks[Array.IndexOf(_landmarkNames, nameFrom)];
            Vector3 lmkTo = landmarks[Array.IndexOf(_landmarkNames, nameTo)];
            return (lmkFrom + lmkTo) * 0.5f;
        }

        public Vector3 GetDistanceByNames(Vector3[] landmarks, string nameFrom, string nameTo)
        {
            Vector3 lmkFrom = landmarks[Array.IndexOf(_landmarkNames, nameFrom)];
            Vector3 lmkTo = landmarks[Array.IndexOf(_landmarkNames, nameTo)];
            return GetDistance(lmkFrom, lmkTo);
        }

        public Vector3 GetDistance(Vector3 lmkFrom, Vector3 lmkTo)
        {
            // return Vector3.Distance(lmkFrom, lmkTo);
            return lmkTo - lmkFrom;
        }
    }
}