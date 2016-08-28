using UnityEngine;
using System.Collections;

public class AnswerTracker : MonoBehaviour
{
    public static AnswerTracker instance = null;

    int [] correctAnswers = new int[(int)ArtFields.eMax];
    int [] inCorrectAnswers = new int[(int)ArtFields.eMax];

    public int GetNumCorrectAnswers(ArtFields artField)
    {
        if(artField != ArtFields.eMax)
        {
            return correctAnswers[(int)artField];
        }
        return 0;
    }

    public int GetNumInCorrectAnswers(ArtFields artField)
    {
        if (artField != ArtFields.eMax)
        {
            return inCorrectAnswers[(int)artField];
        }
        return 0;
    }

    public void IncreaseCorrectAnswers(ArtFields artField)
    {
        if (artField != ArtFields.eMax)
        {
            ++correctAnswers[(int)artField];
        }
    }

    public void IncreaseInCorrectAnswers(ArtFields artField)
    {
        if (artField != ArtFields.eMax)
        {
            ++inCorrectAnswers[(int)artField];
        }
    }

    // Use this for initialization
    void Awake ()
    {
        if(instance == null)
        {
            instance = this;
        }
	}
}
