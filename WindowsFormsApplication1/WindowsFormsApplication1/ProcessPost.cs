using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using Newtonsoft.Json;

namespace PostProcessApplication
{
    public class ProcessPost
    {
        // full list of records
        public List<PostRec> PostRecs { get; set; }
        //top records
        public List<PostRec> TopPostRecs { get; set; }
        // non top records
        public List<PostRec> OtherPostRecs { get; set; }
        //top records for date
        public List<PostRec> TopDaysPostRecs { get; set; }
        // work list for record ids
        public List<PostID> PostIDWork { get; set; }
        // number of records loaded
        public int LoadCount { get; set; }
        // number of records that failed to load
        public int BadCount { get; set; }
        public Boolean LoadPost(string PostFile)
        {
            // load posts into a List
            Boolean LoadSuccess = true;
            LoadCount = 0;
            BadCount = 0;
            PostRecs = new List<PostRec>();
            // use file stream and VB parser to read CSV
            using (FileStream reader = File.OpenRead(PostFile))
            using (TextFieldParser parser = new TextFieldParser(reader))
            {
                parser.TrimWhiteSpace = true; // if you want
                parser.Delimiters = new[] { "," };
                parser.HasFieldsEnclosedInQuotes = true;
                // skip over header record
                string[] header = parser.ReadFields();
                while (!parser.EndOfData)
                {
                    try
                    {
                        //add records to list
                        string[] line = parser.ReadFields();
                        PostRecs.Add(BuildPost(line));
                        ++LoadCount;
                    }
                    catch
                    {
                        // if we get any bad records fail the load
                        LoadSuccess = false;
                        ++BadCount;
                    }
                }
            }
            return (LoadSuccess);
        }
        private PostRec BuildPost(string[] line)
        {
            CultureInfo provider = CultureInfo.CurrentUICulture;
            // build post record for list
            // skip over day of week as it does not match date
            string format = "MMM dd HH:mm:ss yyyy";
            // format to valid date
            DateTime FixDate = DateTime.ParseExact(line[6].Substring(4), format, provider);
            //Create and return post record
            return (new PostRec
            {
                id = Convert.ToInt32(line[0]),
                title = Convert.ToString(line[1]).Trim(),
                privacy = Convert.ToString(line[2]),
                likes = Convert.ToInt32(line[3]),
                views = Convert.ToInt32(line[4]),
                comments = Convert.ToInt32(line[5]),
                timestamp = FixDate
            });
        }
        public void BuildTopAndOther()
        {
            TopPostRecs = new List<PostRec>();
            OtherPostRecs = new List<PostRec>();
            foreach (PostRec PR in PostRecs)
            {
                //Lookup for post records where 
                //*The post must be public
                //* The post must have over 10 comments and over 9000 views
                //* The post title must be under 40 characters

                if (PR.privacy == "public" && PR.comments > 10 && PR.views > 9000 && PR.title.Length < 40)
                {
                    TopPostRecs.Add(PR);
                }
                else
                {
                    OtherPostRecs.Add(PR);
                }
            }
        }
        public void BuildTopDaily()
        {
            TopDaysPostRecs = new List<PostRec>();
            // Sorted the list by date/like count 
            List<PostRec> SortPost = PostRecs.OrderBy(order => order.timestamp.Date).ThenByDescending(order => order.likes).ToList();
            // set first date to min value
            DateTime DateWork = DateTime.MinValue;
            foreach (PostRec PR in SortPost)
            {

                // add record when date changes/ since 2nd sort value is count you will get largest value

                if (PR.timestamp.Date != DateWork)
                {
                    TopDaysPostRecs.Add(PR);
                    DateWork = PR.timestamp.Date;
                }

            }
        }
        public void CreateOutputFilesJ(string DirOut)
            // create Json output full record
        {
            using (StreamWriter file = File.CreateText(DirOut + "\\TopPostF.txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, TopPostRecs);
            }
            using (StreamWriter file = File.CreateText(DirOut + "\\OtherPostF.txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, OtherPostRecs);
            }
            using (StreamWriter file = File.CreateText(DirOut + "\\TopDayPostF.txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, TopDaysPostRecs);
            }
        }
        public void CreateSmallOutputFilesJ(string DirOut)
        {
            // create json output id only

            using (StreamWriter file = File.CreateText(DirOut + "\\TopPost.txt"))
            {
                PostIDWork = new List<PostID>();
                foreach (PostRec pr in TopPostRecs)
                {
                    PostIDWork.Add(new PostID
                    {
                        id = pr.id
                    });
                }
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, PostIDWork);
            }
            using (StreamWriter file = File.CreateText(DirOut + "\\OtherPost.txt"))
            {
                foreach (PostRec pr in OtherPostRecs)
                {
                    PostIDWork.Add(new PostID
                    {
                        id = pr.id
                    });
                }
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, PostIDWork);
            }
            using (StreamWriter file = File.CreateText(DirOut + "\\TopDayPost.txt"))
            {
                foreach (PostRec pr in TopDaysPostRecs)
                {
                    PostIDWork.Add(new PostID
                    {
                        id = pr.id
                    });
                }
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, PostIDWork);
            }
        }
        public void CreateOutputFiles(string DirOut)
        {
            // create csv output full record
            var sb = new StringBuilder();
            System.IO.StreamWriter file;
            foreach (PostRec pr in TopPostRecs)
            {
                sb.AppendLine(pr.id + @",""" + pr.title + @"""," + pr.privacy + "," + pr.likes + "," + pr.views + "," + pr.comments + "," + pr.timestamp);
            }
            file = new System.IO.StreamWriter(DirOut + "\\TopPostF.csv");
            file.WriteLine(sb.ToString());
            file.Close();
            sb = new StringBuilder();
            foreach (PostRec pr in OtherPostRecs)
            {
                sb.AppendLine(pr.id + @",""" + pr.title + @"""," + pr.privacy + "," + pr.likes + "," + pr.views + "," + pr.comments + "," + pr.timestamp);
            }
            file = new System.IO.StreamWriter(DirOut + "\\OtherPostF.csv");
            file.WriteLine(sb.ToString());
            file.Close();

            sb = new StringBuilder();
            foreach (PostRec pr in TopDaysPostRecs)
            {
                sb.AppendLine(pr.id + @",""" + pr.title + @"""," + pr.privacy + "," + pr.likes + "," + pr.views + "," + pr.comments + "," + pr.timestamp);
            }
            file = new System.IO.StreamWriter(DirOut + "\\TopDayPostF.csv");
            file.WriteLine(sb.ToString());
            file.Close();
        }
   
    public void CreateSmallOutputFiles(string DirOut)
        {
        //create csv output id only
        var sb = new StringBuilder();
        System.IO.StreamWriter file;
        foreach (PostRec pr in TopPostRecs)
        {
            sb.AppendLine(pr.id.ToString() );
        }
        file = new System.IO.StreamWriter(DirOut + "\\TopPost.csv");
        file.WriteLine(sb.ToString());
        file.Close();
        sb = new StringBuilder();
        foreach (PostRec pr in OtherPostRecs)
        {
            sb.AppendLine(pr.id.ToString());
        }
        file = new System.IO.StreamWriter(DirOut + "\\OtherPost.csv");
        file.WriteLine(sb.ToString());
        file.Close();

        sb = new StringBuilder();
        foreach (PostRec pr in TopDaysPostRecs)
        {
            sb.AppendLine(pr.id.ToString());
        }
        file = new System.IO.StreamWriter(DirOut + "\\TopDayPost.csv");
        file.WriteLine(sb.ToString());
        file.Close();
        }
    }
}


