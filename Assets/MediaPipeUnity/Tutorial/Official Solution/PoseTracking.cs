using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Stopwatch = System.Diagnostics.Stopwatch;
using Mediapipe.Unity.CoordinateSystem;
using MediaPipeUnity.Tutorial.Official_Solution;
using Unity.VisualScripting;

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

        [SerializeField] private GameObject frontView;
        [SerializeField] private GameObject sideView;
        [SerializeField] private GameObject[] exercisePrefabs;

        private IEnumerator Start()
        {
            var exerciseList = Array.Empty<string>();
            var totalScores = Array.Empty<int>();
            var frameCtr = Array.Empty<int>();

            try
            {
                exerciseList = LevelSelectDisplay.exerciseList;

                _pilotLogic = gameObject.GetComponent<PilotLogic>();

                totalScores = new int[exerciseList.Length];
                frameCtr = new int[exerciseList.Length];
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
            // yield return _resourceManager.PrepareAssetAsync("pose_landmark_heavy.bytes");
            yield return _resourceManager.PrepareAssetAsync("pose_landmark_full.bytes");

            var stopwatch = new Stopwatch();

            _graph = new CalculatorGraph(_configAsset.text);

            var outputVideoStream = new OutputStream<ImageFramePacket, ImageFrame>(_graph, "segmentation_mask");
            var poseLandmarksStream =
                new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(_graph, "pose_landmarks");
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

            PoseEmbedder poseEmbedder = new PoseEmbedder();
            PoseClassifier classifier = new PoseClassifier("Assets/fitness_poses_csvs_out", poseEmbedder);
            EMADictSmoothing smoothing = new EMADictSmoothing();

            while (true)
            {
                _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_inputPixelData));
                var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgb, _width, _height, _width * 3,
                    _inputTexture.GetRawTextureData<byte>());
                var currentTimestamp = stopwatch.ElapsedTicks / (System.TimeSpan.TicksPerMillisecond / 1000);
                _graph.AddPacketToInputStream("input_video",
                    new ImageFramePacket(imageFrame, new Timestamp(currentTimestamp))).AssertOk();

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
                        Vector3[] worldLandmarkPoints = poseLandmarks.Landmark.Select(lmk =>
                            new Vector3(lmk.X * _width, lmk.Y * _height, lmk.Z * _width)
                        ).ToArray();

                        var classifications = classifier.Classify(worldLandmarkPoints);
                        var smoothed = smoothing.Smooth(classifications);
                        try
                        {
                            if (_pilotLogic.currExercise > -1 && _pilotLogic.currExercise < exerciseList.Length)
                            {
                                string pose = exerciseList[_pilotLogic.currExercise];

                                if (LevelSelectDisplay.currLevel.isGuided)
                                {
                                    UpdateModels(pose);
                                }
                                
                                _pilotLogic.SyncPercentage = (int)Math.Floor(smoothed[pose] * 10);

                                // Update percentages here
                                frameCtr[_pilotLogic.currExercise]++;
                                totalScores[_pilotLogic.currExercise] += (int)Math.Floor(smoothed[pose] * 10);
                                _pilotLogic.percentageList[_pilotLogic.currExercise] =
                                    totalScores[_pilotLogic.currExercise] / frameCtr[_pilotLogic.currExercise];

                                _pilotLogic.AvePercentage = _pilotLogic.percentageList[_pilotLogic.currExercise];

                                print(_pilotLogic.percentageList);

                                // Vector3[] landmarks = classifier.GetPoseLandmarks(pose);
                            }
                            else
                            {
                                ClearModelContainers();

                                if (_pilotLogic.nextExercise < exerciseList.Length)
                                {
                                    string pose = exerciseList[_pilotLogic.nextExercise];
                                    print($"Next pose: {pose}");
                                    _pilotLogic.SyncPercentage = (int)Math.Floor(smoothed[pose] * 10);
                                    _pilotLogic.AvePercentage = 0;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            print(e);

                            _pilotLogic.SyncPercentage = 0;
                        }
                    }
                }
            }
        }
        
        // private void OnDestroy()
        // {
        //     print("Destroyed tracking");
        //     if (_webCamTexture != null)
        //     {
        //         _webCamTexture.Stop();
        //     }
        //
        //     if (_graph != null)
        //     {
        //         try
        //         {
        //             _graph.CloseInputStream("input_video").AssertOk();
        //             _graph.WaitUntilDone().AssertOk();
        //         }
        //         finally
        //         {
        //             _graph.Dispose();
        //         }
        //     }
        // }

        private void UpdateModels(string exercise)
        {
            if (frontView.transform.childCount > 0 || sideView.transform.childCount > 0)
            {
                return;
            }

            print("updating models");
            try
            {
                var prefabs = new Dictionary<string, int>()
                {
                    { "Bird Dog (L)", 0 },
                    { "Bird Dog (R)", 1 },
                    { "Elbow Planks", 2 },
                    { "Glute Bridge", 3 },
                    { "High Planks", 4 },
                    { "Pushup Hold", 5 },
                    { "Side Plank (L)", 6 },
                    { "Side Plank (L) Easy", 7 },
                    { "Side Plank (R)", 8 },
                    { "Side Plank (R) Easy", 9 },
                    { "Single Leg Glute Bridge (L)", 10 },
                    { "Single Leg Glute Bridge (R)", 11 },
                    { "Static Lunge (L)", 12 },
                    { "Static Lunge (R)", 13 },
                    { "Straight Bridge", 14 },
                    { "Superman Hold", 15 },
                    { "Sumo Squat", 16 },
                };

                // Instantiate the prefab
                GameObject instantiatedPrefab = Instantiate(exercisePrefabs[prefabs[exercise]]);
                GameObject clonedPrefab = Instantiate(instantiatedPrefab);

                // Set the instantiated prefab's parent to the desired GameObject
                instantiatedPrefab.transform.SetParent(frontView.transform);
                clonedPrefab.transform.SetParent(sideView.transform);

                // Set the instantiated prefab's local position to zero
                instantiatedPrefab.transform.localPosition = Vector3.zero;
                clonedPrefab.transform.localPosition = Vector3.zero;

                // Set the instantiated prefab's local rotation to identity (no rotation)
                instantiatedPrefab.transform.localRotation = Quaternion.identity;
                clonedPrefab.transform.localRotation = Quaternion.identity;

                // Set the instantiated prefab's local scale to one (default scale)
                instantiatedPrefab.transform.localScale = Vector3.one;
                clonedPrefab.transform.localScale = Vector3.one;
            }
            catch (Exception e)
            {
                print(e);
            }
        }

        private void ClearModelContainers()
        {
            foreach (Transform child in frontView.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in sideView.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}