using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;

public class NewspaperUIcontrol : UIControl
{
    public Text headline;
    public Text newsInfo1;
    public Text newsInfo2;

    List<NewspaperArticle> newspaperArticles;

    struct NewspaperArticle
    {
        public string heading;
        public string info2;
        public char grade;
    }

    NewspaperUIcontrol() : base(UITypes.newspaper)
    { }

    void Start()
    {
        newspaperArticles = new List<NewspaperArticle>();
        TextAsset text = Resources.Load<TextAsset>("Newspaper.xml");
        List<NewspaperArticle> newspapersFromFile = LoadXMLFromString(text.ToString());
    }

    List<NewspaperArticle> LoadXMLFromString(string xmlFile)
    {
        XDocument doc = XDocument.Parse(xmlFile);

        IEnumerable<XElement> articles = from NewspaperList in doc.Root.Elements()
                        select NewspaperList;

        List<NewspaperArticle> infos = new List<NewspaperArticle>();
        foreach (XElement article in articles)
        {
            NewspaperArticle info = new NewspaperArticle();
            info.heading = article.Element("Headline").Value;
            info.info2 = article.Element("TextInfo2").Value;
            info.grade = char.Parse(article.Element("Grade").Value);

            infos.Add(info);
        }


        return infos;
    }
    
}

