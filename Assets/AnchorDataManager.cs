using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnchorDataManager : MonoBehaviour
{
    public static AnchorDataManager Instance;
    public AnchorData Data { get; private set; }

    private List<Guid> unboundGuids;

    void Awake()
    {
        Instance = this;
        Data = AnchorData.Deserialize();

        unboundGuids = Data.SavedAnchorIds.Where(guid => guid != Guid.Empty).ToList();

        StartCoroutine(BindUnboundAnchors());
    }

    private IEnumerator BindUnboundAnchors()
    {
        while (unboundGuids.Any())
        {
            var loadOptions = new OVRSpatialAnchor.LoadOptions()
            {
                Uuids = unboundGuids.ToList()
            };
            OVRSpatialAnchor.LoadUnboundAnchors(loadOptions, unboundAnchors =>
            {
                foreach (var anchor in unboundAnchors) anchor.Localize(OnLocalized);
            });

            // Attempt to rebind failed anchors every 5 seconds
            yield return new WaitForSeconds(5);
        }

        void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
        {
            if (!success) return;

            var fingerIndex = Data.GetFingerBySavedGuid(unboundAnchor.Uuid);
            if (fingerIndex < 0) return;
            var personaGO = PersonaManager.Instance.Personas[fingerIndex];
            var spatialAnchor = personaGO.gameObject.AddComponent<OVRSpatialAnchor>();

            personaGO.SpatialAnchor = spatialAnchor;
            personaGO.State = stickToHand.PlacementState.Placed;
            unboundGuids.Remove(unboundAnchor.Uuid);

            var pose = unboundAnchor.Pose;
            spatialAnchor.transform.position = pose.position;
            spatialAnchor.transform.rotation = pose.rotation;

            unboundAnchor.BindTo(spatialAnchor);
        }
    }

    private void OnApplicationPause(bool paused) => Save();

    private void Save()
    {
        Data.Save();
        foreach (OVRSpatialAnchor anchor in GameObject.FindObjectsOfType<OVRSpatialAnchor>())
        {
            anchor.Save();
        }
    }

    [Serializable]
    public class AnchorData
    {
        // Does not update as the anchor status changes, only used for persistent initialization
        public Guid[] SavedAnchorIds { get; private set; }

        [SerializeField] private string[] GuidStrings;

        private const string PLAYER_PREFS_KEY = "AnchorData";

        public void Save()
        {
            GuidStrings ??= new string[PersonaManager.Instance.Personas.Length];
            for (int i = 0; i < PersonaManager.Instance.Personas.Length; i++)
            {
                var anchor = PersonaManager.Instance.Personas[i].SpatialAnchor;
                GuidStrings[i] = anchor == null ? Guid.Empty.ToString() : anchor.Uuid.ToString();
            }
            var json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(PLAYER_PREFS_KEY, json);
        }

        public static AnchorData Deserialize()
        {
            if (!PlayerPrefs.HasKey(PLAYER_PREFS_KEY)) return new AnchorData();

            var json = PlayerPrefs.GetString(PLAYER_PREFS_KEY);
            var anchorData = JsonUtility.FromJson<AnchorData>(json);
            anchorData.SavedAnchorIds = anchorData.GuidStrings.Select(str => new Guid(str)).ToArray();
            return anchorData;
        }

        public int GetFingerBySavedGuid(in Guid guid)
        {
            for (int i = 0; i < SavedAnchorIds.Length; i++)
            {
                if (SavedAnchorIds[i] == guid) return i;
            }
            return -1;
        }

        private AnchorData()
        {
            SavedAnchorIds = new Guid[5];
        }

    }
}
