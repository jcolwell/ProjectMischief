using UnityEngine;
using System.Collections;
using UnityEngine.UI;
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
    eMax
};

public class ArtManager : MonoBehaviour 
{
    // Connect the preFab to this
    public GameObject artContextPreFab;

    public string folder;
    public string incorrectPaintingNamesFile;
    public string incorrectYearsFile;
    public string incorrectArtistFile;
    public string artFilePath;
    public static ArtManager instance = null;

    ArtContext[] paintings;

	void Start () 
    {
        if (instance == null)
        {
            instance = this;
            GameObject[] artPieces = GameObject.FindGameObjectsWithTag("picture");
            if (artPieces.Length > 0)
            {
                paintings = new ArtContext[artPieces.Length];
                PopulateArt(ref artPieces);
                Application.LoadLevelAdditive("UIStudy");
            }
        }
	}

	public int GetNumPaintings()
    {
        return paintings.Length;
    }

    public ArtContext GetPainting(int index)
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

    void PopulateArt(ref GameObject [] artPieces)
    {
        // open up file
       //StreamReader  artList  = new StreamReader(artFilePath);
       //StreamReader choiceList = new StreamReader(incorrectChoicesFilePath);
        string [] artList           = File.ReadAllLines(folder + artFilePath);
        string [] falsePaintingList = File.ReadAllLines(folder + incorrectPaintingNamesFile);
        string [] falseYearList     = File.ReadAllLines(folder + incorrectYearsFile);
        string [] falseArtistList   = File.ReadAllLines(folder + incorrectArtistFile);
        


        if(artList == null || falseArtistList == null || falsePaintingList == null || falseYearList == null)
        {
            Debug.LogError("[Art Manager] failed to load file for art info");
            return;
        }

        const int linesPerArt = 7;
        int numOfArtID = artList.Length / linesPerArt;

        for( int i = 0; i < paintings.Length; ++i )
        {
            GameObject artContext = GameObject.Instantiate(artContextPreFab);
            paintings[i] = artContext.GetComponent<ArtContext>();
            ArtPiece curArt = artPieces[i].GetComponent<ArtPiece>();
            curArt.SetArtContextID(i);

            int id = 0;
            if(curArt.randomID)
            {
                // pick random painting 
                bool uniqueID;
                do
                {
                    id = Random.Range(0, numOfArtID);
                    uniqueID = true;
                    for (int j = i - 1; j >= 0; --j )
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
            }

            curArt.artID = id;
            curArt.forgery = false;

            paintings[i].artID = id;

            // TODO: check for duplicate IDs

            // trim the strings starting at the the id num
            artList[(id * linesPerArt) + (int)FileFields.eArtImage]     = artList[(id * linesPerArt) + (int)FileFields.eArtImage].Remove(0, 5);
            artList[(id * linesPerArt) + (int)FileFields.eForegryImage] = artList[(id * linesPerArt) + (int)FileFields.eForegryImage].Remove(0, 9);
            artList[(id * linesPerArt) + (int)FileFields.ePaintingName] = artList[(id * linesPerArt) + (int)FileFields.ePaintingName].Remove(0, 6);
            artList[(id * linesPerArt) + (int)FileFields.eYear]         = artList[(id * linesPerArt) + (int)FileFields.eYear].Remove(0, 6);
            artList[(id * linesPerArt) + (int)FileFields.eArtist]       = artList[(id * linesPerArt) + (int)FileFields.eArtist].Remove(0, 8);
     
            string texture = "Assets\\Images\\";

            if( curArt.forgery )
            {
                texture = texture + artList[(id * linesPerArt) + (int)FileFields.eForegryImage];
                paintings[i].isForegry = true;
            }
            else
            {
                texture = texture + artList[(id * linesPerArt) + (int)FileFields.eArtImage];
                paintings[i].isForegry = false;
            }

            paintings[i].art = Resources.LoadAssetAtPath<Sprite>( texture );

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

        //artList.Close();
        //artList = null;
        //
        //choiceList.Close();
        //choiceList = null;

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
