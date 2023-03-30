using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaPipeUnity.Tutorial.Official_Solution;
using Unity.VisualScripting;
using UnityEngine;

public class PoseClassifier : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public class PoseSample
  {
    public string name { get; set; }
    public Vector3[] landmarks { get; set; }
    public string className { get; set; }
    public Vector3[] embedding { get; set; }

    public PoseSample(string name, Vector3[] landmarks, string className, Vector3[] embedding)
    {
      this.name = name;
      this.landmarks = landmarks;
      this.className = className;
      this.embedding = embedding;
    }
  }

  public class PoseSampleOutlier
  {
      public PoseSample sample { get; set; }
      public Dictionary<string, int> detectedClass { get; set; }
      public string[] allClasses { get; set; }

      public PoseSampleOutlier(PoseSample sample, Dictionary<string, int> detectedClass, string[] allClasses)
      {
          this.sample = sample;
          this.detectedClass = detectedClass;
          this.allClasses = allClasses;
      }
  }
  
  // Classifies pose landmarks.

  private PoseEmbedder poseEmbedder;
  private int nLandmarks;
  private int nDimensions;
  private int topNByMaxDistance;
  private int topNByMeanDistance;
  private Vector3 axesWeights;
  private List<PoseSample> poseSamples;

  public PoseClassifier(string poseSamplesFolder, PoseEmbedder poseEmbedder, string fileExtension = "csv", string fileSeparator = ",", int nLandmarks = 33, int nDimensions = 3, int topNByMaxDistance = 30, int topNByMeanDistance = 10, Vector3? axesWeights = null)
    {
        this.poseEmbedder = poseEmbedder;
        this.nLandmarks = nLandmarks;
        this.nDimensions = nDimensions;
        this.topNByMaxDistance = topNByMaxDistance;
        this.topNByMeanDistance = topNByMeanDistance;
        this.axesWeights = axesWeights ?? new Vector3(1f, 1f, 0.2f);
        
        poseSamples = LoadPoseSamples(poseSamplesFolder, fileExtension, fileSeparator, nLandmarks, nDimensions, poseEmbedder);
    }

    private static List<PoseSample> LoadPoseSamples(string poseSamplesFolder, string fileExtension, string fileSeparator, int nLandmarks, int nDimensions, PoseEmbedder poseEmbedder)
    {
        List<PoseSample> poseSamples = new List<PoseSample>();
        // Each file in the folder represents one pose class.
        string[] files = System.IO.Directory.GetFiles(poseSamplesFolder, $"*.{fileExtension}");

        foreach (string fileName in files)
        {
            // Use file name as pose class name.
            int startIdx = fileName.LastIndexOf('/') + 1;
            string className = fileName.Substring(startIdx, fileName.Length - (fileExtension.Length + 1) - startIdx);

            // if (className != "Wall Sit")
            // {
            //     print(className);
            //     continue;
            // }

            // Parse CSV.
            using (var reader = new System.IO.StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine().Split(fileSeparator);
                    if (row.Length != nLandmarks * nDimensions + 1)
                    {
                        Debug.LogError($"Wrong number of values: {row.Length}");
                        continue;
                    }
                    Vector3[] landmarks = new Vector3[nLandmarks];
                    for (int i = 1; i < row.Length; i += 3)
                    {
                        landmarks[i / 3] = new Vector3(float.Parse(row[i]), float.Parse(row[i + 1]), float.Parse(row[i + 2]));
                    }

                    poseSamples.Add(new PoseSample(name: row[0],
                        landmarks: landmarks,
                        className: className,
                        embedding: poseEmbedder.Embed(landmarks)));
                }
            }
        }

        return poseSamples;
    }

    public Dictionary<string, int> Classify(Vector3[] poseLandmarks) {
        // Check that provided and target poses have the same shape.
        Debug.Assert(poseLandmarks.Length == nLandmarks, "Unexpected shape");
    
        // Get given pose embedding.
        var poseEmbedding = poseEmbedder.Embed(poseLandmarks);
        var flippedPoseEmbedding = poseEmbedder.Embed(FlipLandmarks(poseLandmarks));
    
        // Filter by max distance.
        //
        // That helps to remove outliers - poses that are almost the same as the
        // given one, but has one joint bent into another direction and actually
        // represent a different pose class.
        var maxDistHeap = new List<Tuple<float, int>>();
        for (int i = 0; i < poseSamples.Count; i++)
        {
            var sample = poseSamples[i];
            var maxDist = Mathf.Min(
                Enumerable.Range(0, poseEmbedding.Length)
                    .Select(j => Vector3.Distance(sample.embedding[j], poseEmbedding[j]))
                    .Max(),
                Enumerable.Range(0, poseEmbedding.Length)
                    .Select(j => Vector3.Distance(sample.embedding[j], flippedPoseEmbedding[j]))
                    .Max()
            );
            maxDistHeap.Add(new Tuple<float, int>(maxDist, i));
        }
    
        maxDistHeap = maxDistHeap.OrderBy(t => t.Item1).ToList();
        maxDistHeap = maxDistHeap.Take(topNByMaxDistance).ToList();
    
        // Filter by mean distance.
        //
        // After removing outliers we can find the nearest pose by mean distance.
        var meanDistHeap = new List<Tuple<float, int>>();
        foreach (var tuple in maxDistHeap)
        {
            var sampleIdx = tuple.Item2;
            var sample = poseSamples[sampleIdx];
            var meanDist = Enumerable.Range(0, poseEmbedding.Length)
                .Select(j => Vector3.Distance(sample.embedding[j], poseEmbedding[j]))
                .Concat(Enumerable.Range(0, poseEmbedding.Length)
                    .Select(j => Vector3.Distance(sample.embedding[j], flippedPoseEmbedding[j])))
                .Average();
            meanDistHeap.Add(new Tuple<float, int>(meanDist, sampleIdx));
        }
    
        meanDistHeap = meanDistHeap.OrderBy(t => t.Item1).ToList();
        meanDistHeap = meanDistHeap.Take(topNByMeanDistance).ToList();
    
        // Collect results into map: (class_name -> n_samples)
        var classNames = meanDistHeap.Select(t => poseSamples[t.Item2].className);
        var result = classNames.GroupBy(name => name).ToDictionary(g => g.Key, g => g.Count());
    
        return result;
    }
    
    private Vector3[] FlipLandmarks(Vector3[] landmarks) {
        return landmarks.Select(l => new Vector3(-l.x, l.y, l.z)).ToArray();
    }

    public Vector3[] GetPoseLandmarks(string pose)
    {
        Vector3[] landmarks = null;

        foreach (var sample in poseSamples)
        {
            if (sample.className.Equals(pose))
            {
                landmarks = sample.landmarks;
                break;
            }
        }

        return landmarks;
    }
    
    // private Vector3 MaxAbsDiff(Vector3[] a, Vector3[] b)
    // {
    //     Vector3 diff = Vector3.zero;
    //     for (int i = 0; i < a.Length; i++)
    //     {
    //         diff.x = Mathf.Max(diff.x, Mathf.Abs(a[i].x - b[i].x));
    //         diff.y = Mathf.Max(diff.y, Mathf.Abs(a[i].y - b[i].y));
    //         diff.z = Mathf.Max(diff.z, Mathf.Abs(a[i].z - b[i].z));
    //     }
    //     return diff;
    // }
    //
    // private Vector3 MeanAbsDiff(Vector3[] a, Vector3[] b)
    // {
    //     Vector3 diff = Vector3.zero;
    //     for (int i = 0; i < a.Length; i++)
    //     {
    //         diff.x += Mathf.Abs(a[i].x - b[i].x);
    //         diff.y += Mathf.Abs(a[i].y - b[i].y);
    //         diff.z += Mathf.Abs(a[i].z - b[i].z);
    //     }
    //     diff /= a.Length;
    //     return diff;
    // }
}
