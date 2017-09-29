using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfileClasses
{
    [Serializable]
    public class Profile:IEquatable<Profile>
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public Profile()
        {
            Name = null;
            Score = 0;
        }
        public Profile(string p_name, int p_score)
        {
            Name = p_name;
            Score = p_score;
        }
        public override string ToString()
        {
            return $"Name - {Name}, Score - {Score}";
        }

        bool IEquatable<Profile>.Equals(Profile other)
        {
            return this.Name == other.Name && this.Score == other.Score;
        }
    }
}
