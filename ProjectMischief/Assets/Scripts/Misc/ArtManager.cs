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
    public string incorrectPaintingNamesFilePath;
    public string incorrectYearsFilePath;
    public string incorrectArtistFilePath;
    public string artFilePath;
    public static ArtManager instance = null;
    ArtContext[] paintings;

	void Awake () 
    {
        if (instance == null)
        {
            instance = this;
            GameObject[] artPieces = GameObject.FindGameObjectsWithTag("picture");
            if (artPieces.Length > 0)
            {
                paintings = new ArtContext[artPieces.Length];
                //PopulateArt(artPieces);
            }
        }
	}
	
	public int GetNumPaintings()
    {
        return paintings.Length;
    }

    public ArtContext GetPainting(int index)
    {
        if( index > 0 && index < paintings.Length )
        {
            return paintings[index];
        }
        else 
        {
            return null;
        }
    }

    void PopulateArt(GameObject [] artPieces)
    {
        // open up file
       //StreamReader  artList  = new StreamReader(artFilePath);
       //StreamReader choiceList = new StreamReader(incorrectChoicesFilePath);
        string [] artList = File.ReadAllLines(artFilePath);
        string [] falsePaintingList = File.ReadAllLines(incorrectPaintingNamesFilePath);
        string [] falseYearList     = File.ReadAllLines(incorrectYearsFilePath);
        string [] falseArtistList   = File.ReadAllLines(incorrectArtistFilePath);
        


        if(artList == null || falseArtistList == null || falsePaintingList == null || falseYearList == null)
        {
            Debug.LogError("[Art Manager] failed to load file for art info");
            return;
        }
        
        const int linesPerArt = 7;
        int numOfArtID = artList.Length / linesPerArt;

        for( int i = 0; i < paintings.Length; ++i )
        {
            ArtPiece curArt = artPieces[i].GetComponent<ArtPiece>();
            curArt.SetArtContextID(i);

            int id = 0;
            if(curArt.randomID)
            {
                // pick random painting 
                id = Random.Range(0, numOfArtID - 1);

                curArt.correctArtist = true;
                curArt.correctYear = true;
                curArt.correctName = true;

                // Choose rnadom amount of fields and make them false
                int numOfWrongFields = Random.Range( 0, (int)ArtFields.eMax - 1);

                while(numOfWrongFields != 0)
                {
                    switch(Random.Range( 0, (int)ArtFields.eMax - 1))
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

            curArt.forgery = false;

            paintings[i].artID = id;

            // TODO: check for duplicate IDs

            // trim the strings starting at the the id num
            artList[id + (int)FileFields.eArtImage] = artList[id].Remove( 0, 4 );
            artList[id + (int)FileFields.eForegryImage] = artList[id].Remove( 0, 9 );
            artList[id + (int)FileFields.ePaintingName] = artList[id].Remove( 0, 5 );
            artList[id + (int)FileFields.eYear] = artList[id].Remove( 0, 5 );
            artList[id + (int)FileFields.eArtist] = artList[id].Remove( 0, 7 );
     
            string texture = "Assets\\Textures\\";

            if( curArt.forgery )
            {
                texture = texture + artList[id + 2];
                paintings[i].isForegry = true;
            }
            else
            {
                texture = texture + artList[id + 1];
                paintings[i].isForegry = false;
            }

            paintings[i].art = Resources.LoadAssetAtPath<Sprite>( texture );

            paintings[i].correctChoices[(int)ArtFields.ePainting] = artList[id + (int)FileFields.ePaintingName];
            paintings[i].correctChoices[(int)ArtFields.eYear]     = artList[id + (int)FileFields.eYear];
            paintings[i].correctChoices[(int)ArtFields.eArtist]   = artList[id + (int)FileFields.eArtist];

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
            } while (curNum != prevNum);

            curChoices[i] = allChoices[curNum];
            prevNum = curNum;
        }
    }

    void RandomizeOrder(ref string [] fields)
    {
        int maxIteration = Random.Range(0, 50); // 50 is an random number for the possible amount of iterations
        for(int i = 0; i < maxIteration; ++i)
        {
            int index1 = Random.Range(0, fields.Length - 1);
            int index2 = 0;
            do
            {
                index2 = Random.Range(0, fields.Length - 1);
            }while(index1 != index2);

            string temp;
            temp = fields[index1];
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
