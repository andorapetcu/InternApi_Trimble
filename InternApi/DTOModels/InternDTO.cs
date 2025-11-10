using Microsoft.VisualBasic;

namespace InternApi.ModelDTO
{
    public class InternDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public int Age { get; set; }

        public DateAndTime date { get; set; }
    }
}
