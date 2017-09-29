using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ProfileClasses;
namespace Examv2
{
    class Model
    {
        private string path = @"..\..\Profiles\Profiles.xml";
        XDocument playersData;
        List<Profile> currentProfiles = new List<Profile>();
       

        public Model()
        {
            playersData = XDocument.Load(path);

            LoadXml();
            
        }

        public void LoadXml()
        {
            currentProfiles.Clear();
            var r = from b in playersData.Element("root").Elements("Player")
                    select b;
            foreach (var i in r)
            {
                Profile prof = new Profile()
                {
                    Name = i.Element("Name").Value,
                    Score = Convert.ToInt32(i.Element("Score").Value)
                };
                currentProfiles.Add(prof);
            }
        }

        public void AddProfile(Profile p)
        {
            playersData.Element("root").Add(new XElement("Player", 
                new XElement("Name", p.Name), 
                new XElement("Score", p.Score)));

            playersData.Save(path);
            currentProfiles.Add(p);
            Console.WriteLine($"{p.Name} added");

        }
        public Profile FindProfile(string name)
        {
            Profile p = new Profile();
            var names = from e in playersData.Element("root").Elements("Player")
                        where e.Element("Name").Value == name
                        select e;
            foreach (var item in names)
            {
                p.Name = item.Element("Name").Value;
                p.Score = Convert.ToInt32(item.Element("Score").Value);
            }
            if (currentProfiles.IndexOf(p) == -1)
            {
                Console.WriteLine("No such name");
                return null;
            }
            return p;
        }
        public void EditProfile(string name, string newName)
        {
            Profile p = FindProfile(name);
          
            if (p != null)
            {
                LoadXml();

                playersData.Save(path);

                Console.WriteLine($"Name changed to {newName}");
            }
        }

        public void ShowAll()
        {
            
            int n = 0;
            foreach (var item in currentProfiles)
            {
                Console.WriteLine(item.ToString());
                ++n;
            }
        }
    }
}
