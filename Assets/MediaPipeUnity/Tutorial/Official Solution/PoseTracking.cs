using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Stopwatch = System.Diagnostics.Stopwatch;
using Mediapipe.Unity.CoordinateSystem;
using MediaPipeUnity.Tutorial.Official_Solution;

namespace Mediapipe.Unity.PoseLandmark
{
  public class PoseTracking : MonoBehaviour
  {
    [SerializeField] private TextAsset _configAsset;

    [SerializeField] private RawImage _screen;

    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _fps;


    [SerializeField] int camaraSwitch;

    //[SerializeField] private MultiFaceLandmarkListAnnotationController _multiFaceLandmarksAnnotationController;

    private CalculatorGraph _graph;

    private ResourceManager _resourceManager;

    private WebCamTexture _webCamTexture;

    private Texture2D _inputTexture;
    private Color32[] _inputPixelData;
    private Texture2D _outputTexture;
    private Color32[] _outputPixelData;

    SidePacket _sidePacketpass;

    private PilotLogic _pilotLogic;
    public GameObject gameObject;
    
    public GameObject skeletonModel;


    private IEnumerator Start()
    {
      var exerciseList = Array.Empty<string>();

      try
      {
        exerciseList = LevelSelectDisplay.exerciseList;
        _pilotLogic = gameObject.GetComponent<PilotLogic>();
        print(_pilotLogic.currExercise);
      }
      catch (Exception e)
      {
        
      }
      
      if (WebCamTexture.devices.Length == 0)
      {
        throw new System.Exception("no camara");
      }
      var webCamDevice = WebCamTexture.devices[camaraSwitch];
      _webCamTexture = new WebCamTexture(webCamDevice.name, _width, _height, _fps);
      _webCamTexture.Play();

      yield return new WaitUntil(() => _webCamTexture.width > 16);

      _screen.rectTransform.sizeDelta = new Vector2(_width, _height);

      _inputTexture = new Texture2D(_width, _height, TextureFormat.RGB24, false);
      _inputPixelData = new Color32[_width * _height];

      _screen.texture = _webCamTexture;

      _resourceManager = new LocalResourceManager();
      yield return _resourceManager.PrepareAssetAsync("pose_detection.bytes");
      yield return _resourceManager.PrepareAssetAsync("pose_landmark_heavy.bytes");

      var stopwatch = new Stopwatch();

      _graph = new CalculatorGraph(_configAsset.text);

      var outputVideoStream = new OutputStream<ImageFramePacket, ImageFrame>(_graph, "segmentation_mask");
      var poseLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(_graph, "pose_landmarks");
      outputVideoStream.StartPolling().AssertOk();
      poseLandmarksStream.StartPolling().AssertOk();

      _sidePacketpass = new SidePacket();

      _sidePacketpass.Emplace("input_rotation", new IntPacket(0));
      _sidePacketpass.Emplace("input_horizontally_flipped", new BoolPacket(false));
      _sidePacketpass.Emplace("input_vertically_flipped", new BoolPacket(true));


      _sidePacketpass.Emplace("output_rotation", new IntPacket(0));
      _sidePacketpass.Emplace("output_horizontally_flipped", new BoolPacket(false));
      _sidePacketpass.Emplace("output_vertically_flipped", new BoolPacket(false));

      _graph.StartRun(_sidePacketpass).AssertOk();
      stopwatch.Start();

      var screenRect = _screen.GetComponent<RectTransform>().rect;
      
      PoseEmbedder poseEmbedder = new PoseEmbedder();
      PoseClassifier classifier = new PoseClassifier("Assets/fitness_poses_csvs_out", poseEmbedder);
      EMADictSmoothing smoothing = new EMADictSmoothing();

      while (true)
      {
        _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_inputPixelData));
        var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgb, _width, _height, _width * 3, _inputTexture.GetRawTextureData<byte>());
        var currentTimestamp = stopwatch.ElapsedTicks / (System.TimeSpan.TicksPerMillisecond / 1000);
        _graph.AddPacketToInputStream("input_video", new ImageFramePacket(imageFrame, new Timestamp(currentTimestamp))).AssertOk();

        yield return new WaitForEndOfFrame();

        if (outputVideoStream.TryGetNext(out var outputVideo))
        {
          if (outputVideo.TryReadPixelData(_outputPixelData))
          {
            _outputTexture.SetPixels32(_outputPixelData);
            _outputTexture.Apply();
          }
        }

        if (poseLandmarksStream.TryGetNext(out var poseLandmarks))
        {
          if (poseLandmarks != null && poseLandmarks.Landmark.Count > 0)
          {
            // print($"Coordinates detected: {poseLandmarks.Landmark.Count}");
            // print(poseLandmarks.Landmark);
            // break;
            
            // TODO: Detect pose here   
            var classifications = classifier.Classify(poseLandmarks.Landmark.Select(lmk =>
              new Vector3(lmk.X * _width, lmk.Y * _height, lmk.Z * _width)
            ).ToArray());

            var smoothed = smoothing.Smooth(classifications);
            
            foreach (KeyValuePair<string, int> poseA in classifications)
            {
              if (poseA.Value >= 0)
              {
                print($"{poseA.Key}: {poseA.Value}");
              }
            }

            // string pose = "Sumo Squat Down";

            try
            {
              string pose = exerciseList[_pilotLogic.currExercise];
              print($"{pose}: {smoothed[pose]}");

              _pilotLogic.SyncPercentage = (int) Math.Floor(smoothed[pose] * 10);
              // var percentage = smoothed[pose] < 6 ? 0 : 
            }
            catch (Exception e)
            {
              print($"Pose not detected");
              
              _pilotLogic.SyncPercentage = 0;
            }
            
           // Update Skeleton
           // for (int i = 0; i < poseLandmarks.Landmark.Count; i++)
           // {
           //   // Get the landmark and its position
           //   var landmark = poseLandmarks.Landmark[i];
           //   Vector3 position = new Vector3(landmark.X, landmark.Y, landmark.Z);
           //
           //   // Update the corresponding joint in the skeletonModel
           //   // Use the landmark index or name to find the corresponding joint in the skeleton
           //   // (e.g., using GameObject.Find or accessing a predefined list of joint GameObjects)
           //   GameObject joint = GetJointByLandmarkIndex(i);
           //   if (joint != null)
           //   {
           //     joint.transform.position = position;
           //     // Update the rotation of the joint based on the position of connected joints
           //   }
           // }
          }
        }
      }

    }
    private void OnDestroy()
    {

      if (_webCamTexture != null)
      {
        _webCamTexture.Stop();
      }
      if (_graph != null)
      {
        try
        {
          _graph.CloseInputStream("input_video").AssertOk();
          _graph.WaitUntilDone().AssertOk();
        }
        finally
        {

          _graph.Dispose();
        }
      }
    }
  }
}