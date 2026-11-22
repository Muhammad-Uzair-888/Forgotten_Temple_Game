using UnityEngine;

namespace NEDClass2024
{
    public class Pedestal : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Pedestal Configuration")]
        [SerializeField] private ArtifactType acceptedArtifact = ArtifactType.Crystal;

        [SerializeField] private Transform placementPoint;

        [SerializeField] private Puzzle1Manager puzzleManager;

        #endregion

        #region Properties

        public ArtifactType AcceptedArtifact => acceptedArtifact;

        public Transform PlacementPoint => placementPoint;

        public bool IsOccupied { get; private set; }

        public ObjectGrabbable PlacedArtifact { get; private set; }

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            EnsurePlacementPoint();
            EnsurePuzzleManager();
        }

        private void EnsurePlacementPoint()
        {
            if (placementPoint == null)
            {
                Transform found = transform.Find("PlacementPoint");
                if (found != null)
                {
                    placementPoint = found;
                }
                else
                {
                    GameObject pointObj = new GameObject("PlacementPoint");
                    pointObj.transform.SetParent(transform, false);
                    pointObj.transform.localPosition = new Vector3(0f, 34.17f, 0f);
                    placementPoint = pointObj.transform;
                }
            }
        }

        private void EnsurePuzzleManager()
        {
            if (puzzleManager == null)
            {
                puzzleManager = FindObjectOfType<Puzzle1Manager>();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates whether the given grabbable artifact matches this pedestal's required type.
        /// </summary>
        public bool CanAcceptArtifact(ObjectGrabbable artifact)
        {
            if (IsOccupied || artifact == null)
                return false;

            return artifact.Type == acceptedArtifact;
        }

        /// <summary>
        /// Places the artifact on this pedestal if valid.
        /// </summary>
        public bool PlaceArtifact(ObjectGrabbable artifact)
        {
            EnsurePlacementPoint();
            EnsurePuzzleManager();

            if (!CanAcceptArtifact(artifact))
                return false;

            PlacedArtifact = artifact;
            IsOccupied = true;

            artifact.PlaceAt(placementPoint);

            Debug.Log($"[Pedestal] {gameObject.name} successfully received {artifact.Type}!");

            if (puzzleManager != null)
            {
                puzzleManager.OnPedestalStateChanged();
            }

            return true;
        }

        #endregion
    }
}
