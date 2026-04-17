using System;
using System.Linq;
using System.Threading.Tasks;
using Networking.API;
using Persistence;
using UnityEngine;
using UnityEngine.UI;
using EditorAttributes;

namespace UI
{
    public class PuzzleLeaderboard : MonoBehaviour
    {

        [SerializeField] private LeaderboardRow rowPrefab;
        [SerializeField] private GameObject entriesContainer;
        [SerializeField] private Button exitButton;
        public event Action OnExit;

        private void Awake()
        {
            exitButton.onClick.AddListener(Exit);
            OnExit += ClearLeaderboard;
        }

        public async void FillLeaderboard(string puzzleId)
        {
            var entries = await PuzzleServerApi.Instance.Leaderboards.GetLeaderboardEntries(puzzleId);

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                var row = Instantiate(
                    rowPrefab,
                    entriesContainer.transform,
                    false
                );
                row.SetEntry(i + 1, entry.username, entry.time);
                row.gameObject.SetActive(true);
            }
        }

        private void ClearLeaderboard()
        {
            foreach (var row in GetComponentsInChildren<LeaderboardRow>())
            {
                Destroy(row.gameObject);
            }
        }

        [Button("On Exit")]
        private void Exit()
        {
            OnExit?.Invoke();
        }
    }
}