using UnityEngine;
using UnityEngine.UI;

public partial class ObjectCounter : MonoBehaviour
{
    private void UpdateMainOutput()
    {
        CalAccRate();
        switch (TextMode)
        {
            case EditorComboIndicator.ScoreClassic: // Score (+) Classic
                statusScore.text = string.Format("{0:#,##0}", FiNowScore());
                break;
            case EditorComboIndicator.AchievementClassic: // Achievement (+) Classic
                UpdateAchievementColor(accRate[0]);
                statusAchievement.text = string.Format("{0,6:0.00}%", accRate[0]);
                break;
            case EditorComboIndicator.AchievementDownClassic: // Achievement (-) Classic (from 100%)
                UpdateAchievementColor(accRate[1]);
                statusAchievement.text = string.Format("{0,6:0.00}%", accRate[1]);
                break;
            case EditorComboIndicator.AchievementDeluxe: // Achievement (+) Deluxe
                UpdateAchievementColor(accRate[4]);
                statusAchievement.text = string.Format("{0,8:0.0000}%", accRate[4]);
                break;
            case EditorComboIndicator.AchievementDownDeluxe: // Achievement (-) Deluxe (from 100%)
                UpdateAchievementColor(accRate[3]);
                statusAchievement.text = string.Format("{0,8:0.0000}%", accRate[3]);
                break;
            case EditorComboIndicator.ScoreDeluxe: // DX Score (+)
                statusDXScore.text = DxExNowScore().ToString();
                break;
            case EditorComboIndicator.CScoreDedeluxe: // Score (+) DeDX
                statusScore.text = string.Format("{0:#,##0}", DeDxNowScore());
                break;
            case EditorComboIndicator.CScoreDownDedeluxe: // Score (-) DeDX (from 100% rate)
                statusScore.text = string.Format("{0:#,##0}", DeDxNowBreakScore());
                break;
            case EditorComboIndicator.Combo:
            default:
                statusCombo.text = combo > 0 ? combo.ToString() : "";
                break;
        }
        void UpdateAchievementColor(double achievementRate)
        {
            var newColor = achievementRate switch
            {
                >= 100 => AchievementGoldColor,
                >= 97f => AchievementSilverColor,
                >= 80f => AchievementBronzeColor,
                _ => AchievementDudColor
            };

            var textElements = statusAchievement.gameObject.GetComponentsInChildren<Text>();

            foreach (var celm in textElements)
                if (celm.color != newColor)
                    celm.color = newColor;
        }
    }
}