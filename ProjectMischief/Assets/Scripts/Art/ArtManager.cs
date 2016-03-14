using UnityEngine;
using UnityEngine.UI;
//using System.Text;
using System.Collections;
using System.IO;

enum ArtFields
{
    ePainting = 0,
    eYear,
    eArtist,
    eMax
};

enum FileFields
{
    eArtId = 0,
    eArtImage,
    eForegryImage,
    ePaintingName,
    eYear,
    eArtist,
    eDescription,
    eMax
};

public class ArtManager : MonoBehaviour 
{
    public bool uiStudyEnabled = true;

    // Connect the preFab to this
    public GameObject artContextPreFab;

    public string incorrectPaintingNamesFile;
    public string incorrectYearsFile;
    public string incorrectArtistFile;
    public string artFile;
    public static ArtManager instance = null;

    public float minPercentageForA = 90.0f;
    public float minPercentageForB = 80.0f;
    public float minPercentageForC = 65.0f;
    public float minPercentageForD = 50.0f;

    int numIncorrectAtStart = 0;
    int correctChoices = 0;
    int correctChanges = 0;
    int incorrectChanges = 0;

    ArtContext[] paintings;
    Vector3[] paintingsPos;

	public char GetLetterGrade()
    {
        for (uint i = 0; i < paintings.Length; ++i)
        {
            ArtContext curPainting = paintings[i];
            for(uint j = 0; j < curPainting.currentChoices.Length; ++j)
            {
                if(curPainting.correctChoices[j] == curPainting.currentChoices[j])
                {
                    ++correctChanges;
                }
                else
                {
                    ++incorrectChanges;
                }
            }
        }

        correctChoices = correctChanges;

        int numTotalFields = (int)ArtFields.eMax * paintings.Length;
        //correctChanges = correctChanges - (numTotalFields - numIncorrectAtStart);
        //correctChanges = (correctChanges <= 0) ? 0 : correctChanges;

        float percentage = 1.0f;
        percentage = (float)correctChanges / (float)numTotalFields;
        percentage *= 100.0f;
        //if (numIncorrectAtStart != 0)
        //{
        //    //percentage = ((correctChanges - incorrectChanges) / numIncorrectAtStart) * 100.0f;
        //    percentage = ((float)correctChanges / (float)numIncorrectAtStart);
        //    percentage *= 100.0f;
        //}
        //else if(incorrectChanges == 0)
        //{
        //    percentage = 100.0f;
        //}
        //else
        //{
        //    percentage = 0.0f;
        //}

        if      (percentage >= minPercentageForA) return 'A';
        else if (percentage >= minPercentageForB) return 'B';
        else if (percentage >= minPercentageForC) return 'C';
        else if (percentage >= minPercentageForD) return 'D';
                                                  return 'F';
    }

    public int GetCorrectChanges()
    {
        return correctChanges;
    }

    public int GetCorrectChoices()
    {
        return correctChoices;
    }

	public uint GetNumPaintings()
    {
        return (uint)paintings.Length;
    }

    public ArtContext GetPainting(uint index)
    {
        if( index >= 0 && index < paintings.Length )
        {
            return paintings[index];
        }
        else 
        {
            return null;
        }
    }

    public Vector3 GetPaintingPos(uint index)
    {
        if(index < paintingsPos.Length)
        {
            return paintingsPos[index];
        }
        return new Vector3();
    }

    public void SetPaintingPos(uint index, Vector3 pos)
    {
        if (index < paintingsPos.Length)
        {
            paintingsPos[index] = pos;
        }
    }

    void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
            GameObject[] artPieces = GameObject.FindGameObjectsWithTag("picture");
            if (artPieces.Length > 0)
            {
                paintings = new ArtContext[artPieces.Length];
                paintingsPos = new Vector3[artPieces.Length];
                PopulateArt(ref artPieces);
            }
        }
    }

    void Start()
    {
        if (uiStudyEnabled)
        {
            UIManager.instance.LoadStudyUI();
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    void PopulateArt(ref GameObject [] artPieces)
    {
        // open up file
        TextAsset text = Resources.Load<TextAsset> ( artFile );
        char[] delim = new char[] { '\r', '\n' };
        string[] artList = text.text.Split ( delim, System.StringSplitOptions.RemoveEmptyEntries );

        text = Resources.Load<TextAsset> ( incorrectPaintingNamesFile );
        string[] falsePaintingList = text.text.Split ( delim, System.StringSplitOptions.RemoveEmptyEntries );

        text = Resources.Load<TextAsset> ( incorrectYearsFile );
        string [] falseYearList =  text.text.Split ( delim, System.StringSplitOptions.RemoveEmptyEntries );

        text = Resources.Load<TextAsset> ( incorrectArtistFile );
        string[] falseArtistList = text.text.Split ( delim, System.StringSplitOptions.RemoveEmptyEntries );
        
        if(artList == null || falseArtistList == null || falsePaintingList == null || falseYearList == null)
        {
            //Debug.LogError("[Art Manager] failed to load file for art info");
            return;
        }

        const int linesPerArt = 7;
        int numOfArtID = artList.Length / linesPerArt;

        for( uint i = 0; i < paintings.Length; ++i )
        {
            GameObject artContext = GameObject.Instantiate(artContextPreFab);
            paintings[i] = artContext.GetComponent<ArtContext>();
            ArtPiece curArt = artPieces[i].GetComponent<ArtPiece>();
            curArt.SetArtContextID(i);

            int id = 0;
            bool uniqueID = true;
            if(curArt.randomID)
            {
                // pick random painting 
                do
                {
                    id = Random.Range(0, numOfArtID);
                    uniqueID = true;
                    for (int j =  (int)i - 1 ; j >= 0; --j )
                    {
                        uniqueID = (paintings[j].artID == id) ? false : uniqueID;
                    }
                } while (!uniqueID);

                curArt.correctArtist = true;
                curArt.correctYear = true;
                curArt.correctName = true;

                // Choose random amount of fields and make them false
                int numOfWrongFields = 0;
                if(Random.Range(0, 3) != 0)
                {
                    numOfWrongFields = Random.Range(1, (int)ArtFields.eMax);
                }

                numIncorrectAtStart += numOfWrongFields;

                while(numOfWrongFields != 0)
                {
                    switch(Random.Range( 0, (int)ArtFields.eMax))
                    {
                        case (int)ArtFields.ePainting:
                            numOfWrongFields = curArt.correctName ? --numOfWrongFields : numOfWrongFields;
                            curArt.correctName = false;
                            break;
                        case (int)ArtFields.eYear:
                            numOfWrongFields = curArt.correctYear ? --numOfWrongFields : numOfWrongFields;
                            curArt.correctYear = false;
                            break;
                        case (int)ArtFields.eArtist:
                            numOfWrongFields = curArt.correctArtist ? --numOfWrongFields : numOfWrongFields;
                            curArt.correctArtist = false;
                            break;
                    }
                }
            }
            else
            {
                id = (curArt.artID < numOfArtID && curArt.artID >= 0) ? curArt.artID : Random.Range(0, numOfArtID - 1);
                for (int j = (int)i - 1; j >= 0; --j)
                {
                    uniqueID = (paintings[j].artID == id) ? false : uniqueID;
                }
                numIncorrectAtStart = curArt.correctArtist ? numIncorrectAtStart : ++numIncorrectAtStart;
                numIncorrectAtStart = curArt.correctName   ? numIncorrectAtStart : ++numIncorrectAtStart;
                numIncorrectAtStart = curArt.correctYear   ? numIncorrectAtStart : ++numIncorrectAtStart;
            }

            curArt.artID = id;
            curArt.forgery = false;

            paintings[i].artID = id;

            // TODO: check for duplicate IDs

            // trim the strings starting at the the id num
            if (uniqueID)
            {
                artList[(id * linesPerArt) + (int)FileFields.eArtImage] = artList[(id * linesPerArt) + (int)FileFields.eArtImage].Remove(0, 5);
                artList[(id * linesPerArt) + (int)FileFields.eForegryImage] = artList[(id * linesPerArt) + (int)FileFields.eForegryImage].Remove(0, 9);
                artList[(id * linesPerArt) + (int)FileFields.ePaintingName] = artList[(id * linesPerArt) + (int)FileFields.ePaintingName].Remove(0, 6);
                artList[(id * linesPerArt) + (int)FileFields.eYear] = artList[(id * linesPerArt) + (int)FileFields.eYear].Remove(0, 6);
                artList[(id * linesPerArt) + (int)FileFields.eArtist] = artList[(id * linesPerArt) + (int)FileFields.eArtist].Remove(0, 8);
                artList[(id * linesPerArt) + (int)FileFields.eDescription] = artList[(id * linesPerArt) + (int)FileFields.eDescription].Remove(0, 6);
            }

            ArtFileInfo artInfo = new ArtFileInfo();
            artInfo.artFileName = artList[(id * linesPerArt) + (int)FileFields.eArtImage];
            artInfo.name = artList[(id * linesPerArt) + (int)FileFields.ePaintingName];
            artInfo.year = artList[(id * linesPerArt) + (int)FileFields.eYear];
            artInfo.artist = artList[(id * linesPerArt) + (int)FileFields.eArtist];
            artInfo.description = artList[(id * linesPerArt) + (int)FileFields.eDescription];
            PersistentSceneData.GetPersistentData().AddEncounterdArt(artInfo);

            string texture = "Assets\\Images\\";

            if( curArt.forgery )
            {
                //texture = texture + artList[(id * linesPerArt) + (int)FileFields.eForegryImage];
                texture = artList[(id * linesPerArt) + (int)FileFields.eForegryImage];
                paintings[i].isForegry = true;
            }
            else
            {
                //texture = texture + artList[( id * linesPerArt ) + ( int ) FileFields.eArtImage];
                texture = artList[( id * linesPerArt ) + ( int ) FileFields.eArtImage];
                paintings[i].isForegry = false;
            }


            paintings[i].art = Resources.Load<Sprite>( texture );
            paintings[i].description = artList[(id * linesPerArt) + (int)FileFields.eDescription];

            if(paintings[i].art == null)
            {
                //Debug.Log ( "Shit is NULL" );
            }

            paintings[i].correctChoices[(int)ArtFields.ePainting] = artList[(id * linesPerArt)+ (int)FileFields.ePaintingName];
            paintings[i].correctChoices[(int)ArtFields.eYear]     = artList[(id * linesPerArt)+ (int)FileFields.eYear];
            paintings[i].correctChoices[(int)ArtFields.eArtist]   = artList[(id * linesPerArt)+ (int)FileFields.eArtist];

            // fill up choices for all fields
            paintings[i].paintingchoices[0] = paintings[i].correctChoices[(int)ArtFields.ePainting];
            FillChoices(ref paintings[i].paintingchoices, falsePaintingList);

            paintings[i].yearChoices[0] = paintings[i].correctChoices[(int)ArtFields.eYear];
            FillChoices(ref paintings[i].yearChoices, falseYearList);

            paintings[i].artistChoices[0] = paintings[i].correctChoices[(int)ArtFields.eArtist];
            FillChoices(ref paintings[i].artistChoices, falseArtistList);

            // Randomize the order of fields
            RandomizeOrder(ref paintings[i].paintingchoices);
            RandomizeOrder(ref paintings[i].yearChoices);
            RandomizeOrder(ref paintings[i].artistChoices);

            // set current choices

            SetCurrentChoices(ref paintings[i].currentChoices[(int)ArtFields.ePainting], paintings[i].paintingchoices,
                paintings[i].correctChoices[(int)ArtFields.ePainting], curArt.correctName);
            

            SetCurrentChoices(ref paintings[i].currentChoices[(int)ArtFields.eYear], paintings[i].yearChoices,
                paintings[i].correctChoices[(int)ArtFields.eYear], curArt.correctYear);

            SetCurrentChoices(ref paintings[i].currentChoices[(int)ArtFields.eArtist], paintings[i].artistChoices,
                paintings[i].correctChoices[(int)ArtFields.eArtist], curArt.correctArtist);

        }

        artList = null;
        falseArtistList = null;
        falseYearList = null;
        falsePaintingList = null;
    }

    void FillChoices(ref string [] curChoices, string [] allChoices)
    {
        int prevNum = -1;

        for(int i = 1; i < 3; ++i)
        {
            int curNum;
            do
            {
                curNum = Random.Range(0, allChoices.Length - 1);
            } while (curNum == prevNum);

            curChoices[i] = allChoices[curNum];
            prevNum = curNum;
        }
    }

    void RandomizeOrder(ref string [] fields)
    {
        int maxIteration = Random.Range(1, 10); 
        for(int i = 0; i < maxIteration; ++i)
        {
            int index1 = Random.Range(0, fields.Length);
            int index2 = 0;
            do
            {
                index2 = Random.Range(0, fields.Length);
            }while(index1 == index2);

            string temp = fields[index1];
            fields[index1] = fields[index2];
            fields[index2] = temp;
            temp = null;
        }
    }

    void SetCurrentChoices(ref string curChoice, string [] allChoices, string correctChoice, bool setAsCorrect)
    {
        if(setAsCorrect)
        {
            curChoice = correctChoice;
        }
        else
        {
            do
            {
                int randIndex = Random.Range(0, allChoices.Length - 1);
                curChoice = allChoices[randIndex];
            } while (curChoice == correctChoice);
        }
    }
}
