using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using iCampusApp.Model;
using iCampusApp.ViewModel;
using System.Threading;
using System.Diagnostics;

namespace iCampusApp
{
    public class Update
    {
        public bool RefreshAll;

        #region REGEX

        private static Regex regexZoneCours = new Regex(@"<dl class=""userCourseList"">(?<zone>[\P{Cn}]+?)</dl>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex regexCours = new Regex(@"<dt class=""(?<stat>[\P{Cn}]*?)""[\P{Cn}]*?<a href=""(?<link>[\P{Cn}]*?)"">(?<name>[\P{Cn}]+?)</a>[\P{Cn}]*?<small>(?<prof>[\P{Cn}]+?)</small>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex regexZoneSections = new Regex(@"<div class=""menu vmenu ""  id=""commonToolList"">(?<zone>[\P{Cn}]+?)</div>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex regexSections = new Regex(@"<a id=""(?<id>[\P{Cn}]+?)""class=""item(?<stat>.*?)"" href=""(?<link>[\P{Cn}]+?)"">[\P{Cn}]+?&nbsp;(?<name>[\P{Cn}]+?)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex regexCoursSection = new Regex(@"<title>(?<cours>[\P{Cn}]+?)</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static Regex regexDocsZone = new Regex(@"<table class=""claroTable emphaseLine"" width=""100%"">[\P{Cn}]+?<tbody>(?<zone>[\P{Cn}]+?)</tbody>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex regexDocsRoot = new Regex(@"<img src=""/web/img/opendir.png[\P{Cn}]+?/>\W*?(?<root>.*?)</th>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static Regex regexDocs = new Regex(@"<tr.+?<a class=""(?<stat>.+?)"" href=""(?<url>/claroline/(?<type>document|backends)/.+?)"".+?><img.+?/>(?<name>.+?)</a>.+?<small>(?<size>.*?)</small>.+?<small>(?<date>.+?)</small>(?(</td></tr><tr align=""left"">).+?<div class=""comment"">(?<comment>.+?)</div>)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static Regex regexAnnZone = new Regex(@"<h3 class=""claroToolTitle"">Annonces</h3><p></p>(?<zone>.*?)<div class=""spacer""></div>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static Regex regexAnn = new Regex(@"<h4 class=""claroBlockHeader""><span class=""(?<status>.*?)"">.*?/> Publiée le : (?<date>.*?)</span>.*?<p><strong>(?<title>.*?)</strong></p>.*?<p>(?<text>.*?)</div><div class=""lnk_link_panel"">", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled); 
        
        #endregion

        public Update(bool RA)
        {
            RefreshAll = RA;
        }

        public void readHTML(string HTMLContent)
        {
            HTMLContent = HTMLContent.Replace("\n", "");
            String[][] result = null;
            string tempCours = (regexCoursSection.Match(HTMLContent)).Groups["cours"].Value;

            #region REGEX COURS
            Match matchingZone = regexZoneCours.Match(HTMLContent);
            if (matchingZone.Success)
            {
                while (matchingZone.Success)
                {
                    string matched = matchingZone.Groups["zone"].Value;
                    MatchCollection Cours = regexCours.Matches(matched);
                    if (result == null) result = new String[Cours.Count][];
                    else
                    {
                        if (Cours.Count != 0)
                        {
                            String[][] resultO = result;
                            result = new String[Cours.Count + resultO.GetLength(0)][];
                            resultO.CopyTo(result, 0);
                        }
                        else break; //Si le matching ne trouve plus rien, on quitte.
                    }
                    int last = 0;
                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        if (result[i] == null)
                        {
                            last = i;
                            break;
                        }
                    }
                    for (int i = last; i < Cours.Count; i++)
                    {
                        result[i] = new String[5];
                        String[] name = Cours[i].Groups["name"].Value.Split('-');
                        result[i][0] = name[0].Trim();
                        result[i][1] = Cours[i].Groups["name"].Value.Replace(name[0] + "-", string.Empty).Trim();
                        result[i][2] = Cours[i].Groups["link"].Value;
                        result[i][3] = Cours[i].Groups["prof"].Value;
                        result[i][4] = Cours[i].Groups["stat"].Value.Contains("hot") ? "hot" : "normal";
                    }
                    matchingZone = matchingZone.NextMatch();
                }
                if (result.Length != 0)
                    PushIntoDB(result, "Cours");
            }

#endregion
            #region REGEX SECTION
            matchingZone = regexZoneSections.Match(HTMLContent);
            if (matchingZone.Success)
            {
                tempCours = tempCours.Split('-')[0].Trim();
                while (matchingZone.Success)
                {
                    string matched = matchingZone.Groups["zone"].Value;
                    MatchCollection section = regexSections.Matches(matched);
                    if (result == null) result = new String[section.Count][];
                    else
                    {
                        if (section.Count != 0)
                        {
                            String[][] resultO = result;
                            result = new String[section.Count + resultO.GetLength(0)][];
                            resultO.CopyTo(result, 0);
                        }
                        else break; //Si le matching ne trouve plus rien, on quitte.
                    }
                    int last = 0;
                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        if (result[i] == null)
                        {
                            last = i;
                            break;
                        }
                    }
                    for (int i = last; i < section.Count; i++)
                    {
                        result[i] = new String[5];
                        result[i][0] = section[i].Groups["name"].Value;
                        result[i][1] = section[i].Groups["link"].Value;
                        result[i][2] = section[i].Groups["id"].Value;
                        result[i][3] = section[i].Groups["stat"].Value.Contains("hot") ? "hot" : "normal";
                        result[i][4] = tempCours;
                    }
                    matchingZone = matchingZone.NextMatch();
                }
                if (result.Length != 0)
                PushIntoDB(result, "Section");
            }
            #endregion
            #region REGEX DOCS
            matchingZone = regexDocsZone.Match(HTMLContent);
            if (matchingZone.Success)
            {
                tempCours = tempCours.Split('-')[1].Trim();

                string root = null;
                Match matchingRoot = regexDocsRoot.Match(HTMLContent);
                if(matchingRoot.Success)
                    root = matchingRoot.Groups["root"].Value;
                while (matchingZone.Success)
                {
                    string matched = matchingZone.Groups["zone"].Value;
                    MatchCollection docs = regexDocs.Matches(matched);
                    if (result == null) result = new String[docs.Count][];
                    else
                    {
                        if (docs.Count != 0)
                        {
                            String[][] resultO = result;
                            result = new String[docs.Count + resultO.GetLength(0)][];
                            resultO.CopyTo(result, 0);
                        }
                        else break; //Si le matching ne trouve plus rien, on quitte.
                    }
                    int last = 0;
                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        if (result[i] == null)
                        {
                            last = i;
                            break;
                        }
                    }
                    for (int i = last; i < docs.Count; i++)
                    {
                        result[i] = new String[9];
                        result[i][0] = docs[i].Groups["name"].Value;
                        result[i][1] = docs[i].Groups["url"].Value;
                        result[i][2] = docs[i].Groups["type"].Value;
                        result[i][3] = docs[i].Groups["size"].Value;
                        result[i][4] = docs[i].Groups["date"].Value;
                        result[i][5] = docs[i].Groups["comment"].Value;
                        result[i][6] = docs[i].Groups["stat"].Value.Contains("hot") ? "hot" : "normal";
                        result[i][7] = tempCours;
                        result[i][8] = root;
                    }
                    matchingZone = matchingZone.NextMatch();
                }
                if (result.Length != 0)
                PushIntoDB(result, "Docs");
            }
            #endregion
            #region REGEX ANNONCES
            matchingZone = regexAnnZone.Match(HTMLContent);
            if (matchingZone.Success)
            {
                tempCours = tempCours.Split('-')[1].Trim();
                while (matchingZone.Success)
                {
                    string matched = matchingZone.Groups["zone"].Value;
                    MatchCollection annonces = regexAnn.Matches(matched);
                    if (result == null) result = new String[annonces.Count][];
                    else
                    {
                        if (annonces.Count != 0)
                        {
                            String[][] resultO = result;
                            result = new String[annonces.Count + resultO.GetLength(0)][];
                            resultO.CopyTo(result, 0);
                        }
                        else break; //Si le matching ne trouve plus rien, on quitte.
                    }
                    int last = 0;
                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        if (result[i] == null)
                        {
                            last = i;
                            break;
                        }
                    }
                    for (int i = last; i < annonces.Count; i++)
                    {
                        result[i] = new String[5];
                        result[i][0] = annonces[i].Groups["title"].Value;
                        result[i][1] = StripTags(annonces[i].Groups["text"].Value);
                        result[i][2] = annonces[i].Groups["date"].Value;
                        result[i][3] = annonces[i].Groups["stat"].Value.Contains("hot") ? "hot" : "normal";
                        result[i][4] = tempCours;
                    }
                    matchingZone = matchingZone.NextMatch();
                }
                if (result.Length != 0)
                    PushIntoDB(result, "Annonces");
            }
            #endregion
        }

        private void PushIntoDB(string[][] result, String type)
        {
            iCampusViewModel VM = App.ViewModel;
            Cours coursToUpdate;

            switch (type)
            {
                #region CASE COURS
                case "Cours":
                    foreach (string[] array in result)
                    {
                        Cours toPushC = new Cours { sysCode = array[0], title = array[1], titular = array[3], notified = (array[4] == "hot") };
                        VM.AddCours(toPushC);
                    }
                    VM.ClearCoursList();

                    if (RefreshAll)
                    {
                        RefreshAll = false;
                        Connect cx = new Connect();
                        foreach (Cours item in VM.AllCours)
                        {
                            //cx.Sync(item.url);
                        }
                    }
                    break;
                #endregion
                #region CASE SECTION
                case "Section":
                    coursToUpdate = (from Cours _cours in VM.AllCours where _cours.sysCode == result[0][4] select _cours).First();

                    foreach (string[] array in result)
                    {
                        switch (array[0])
                        {
                            case "Documents et liens":

                                Connect cx = new Connect();
                                coursToUpdate.isDnL = true;
                                coursToUpdate.dnlNotif = (array[4] == "hot");
                                cx.Sync(array[1].Replace("&amp;","&"));
                                break;

                            case "Annonces":
                                coursToUpdate.isAnn = true;
                                coursToUpdate.annNotif = (array[4] == "hot");
                                (new Connect()).Sync(array[1].Replace("&amp;", "&"));
                                break;

                            default:
                                break;
                        }
                    }
                    //VM.AddCours(coursToUpdate);
                    break;
                #endregion
                #region CASE DOC
                case "Docs":
                    coursToUpdate = (from Cours _cours in VM.AllCours where _cours.sysCode == result[0][7] select _cours).First();

                    foreach (string[] array in result)
                    {
                        string[] name = array[0].Split('.');

                        Documents toPush = new Documents()
                        {
                            url = array[1],
                            Description = array[5],
                            notified = (array[6] == "hot"),
                            Cours = coursToUpdate
                        };

                        switch (array[2])
                        {
                            case "document": //Dossier
                                toPush.path = array[0].Trim();
                                toPush.IsFolder = true;
                                toPush.date = DateTime.Today;
                                (new Connect()).Sync(array[1].Replace("&amp;", "&"));
                                break;

                            case "backends": //Fichier
                                string[] date = array[4].Split('.');
                                string[] partialSize = array[3].Replace("&nbsp;", " ").Replace('.', ',').Split(' ');
                                switch (partialSize[1])
                                {
                                    case "Ko":
                                        partialSize[1] = "E+3";
                                        break;
                                    case "Mo":
                                        partialSize[1] = "E+6";
                                        break;
                                    case "Go":
                                        partialSize[1] = "E+9";
                                        break;
                                    default:
                                        partialSize[1] = "";
                                        break;
                                }
                                if (name.Length > 2)
                                {
                                    for (int i = 1; i < name.Length - 1; i++)
                                    {
                                        name[0] += " "+name[i];
                                    }
                                }
                                toPush.path = name[0].Trim();
                                toPush.IsFolder = false;
                                toPush.Extension = name.Last().Trim();
                                toPush.size = Double.Parse(partialSize[0] + partialSize[1]);
                                toPush.date = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));
                                break;

                            default:
                                break;
                        }
                        if (array[8] != null)
                        {
                            toPush.RootFolder = (from Documents _root in VM.AllFolders where _root.path == result[0][8] select _root).First();
                            toPush.IsRoot = false;
                        }
                        else
                        {
                            toPush.IsRoot = true;
                        }
                        //Debug.WriteLine("Adding Document" + toPush.Name + "\r" + toPush.IsRoot);
                        VM.AddDocument(toPush);
                    }
                    //VM.AddCours(coursToUpdate);
                    break;
                #endregion

                #region CASE ANNONCE
                case "Annonces":
                    coursToUpdate = (from Cours _cours in VM.AllCours where _cours.sysCode == result[0][4] select _cours).First();
                    foreach (string[] array in result)
                    {
                        Annonce AnnToPush = new Annonce()
                        {
                            title = array[0],
                            content = array[1],
                            notified = (array[3] == "hot"),
                            Cours = coursToUpdate,
                            date = DateTime.Parse(array[2])
                        };
                        //Debug.WriteLine("Adding "+ AnnToPush.title +" to DB (depend of " +  AnnToPush.Cours.sysCode);
                        VM.AddAnnonce(AnnToPush);
                    }
                    VM.ClearAnnsOfCours(coursToUpdate);
                    break;
                #endregion

                default:
                    break;
            }
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTags(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

    }
}
