using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessApplication

{
    public class PostRec
    {
        public int id { get; set; }
        public string title { get; set; }
        public string privacy { get; set; }

        public int likes { get; set; }

        public int views { get; set; }

        public int comments { get; set; }

        public DateTime timestamp { get; set; }
    }

    public class PostID
    {
        public int id { get; set; }
    }
}
