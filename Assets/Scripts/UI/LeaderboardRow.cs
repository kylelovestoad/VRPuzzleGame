using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LeaderboardRow : MonoBehaviour
    {

        [SerializeField] private TMP_Text rankField;
        [SerializeField] private TMP_Text userField;
        [SerializeField] private TMP_Text timeField;

        public void SetEntry(long rank, string username, float time)
        {
            rankField.text = rank.ToString();
            userField.text = username;
            timeField.text = UIUtils.AsTimeStringMillis(time);
        }
    }
}