using ISchool.Model;

namespace ISchool.Request
{
    public class Request
    {
        public Department Deb { get; set; }
        public int Year { get; set; }
        public string Sec { get; set; }

        public Request(Department deb, int year, string sec)
        {
            Deb = deb;
            Year = year;
            Sec = sec;
        }
    }
}