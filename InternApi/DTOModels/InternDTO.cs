using Microsoft.VisualBasic;

namespace InternApi.ModelDTO
{
    /// <summary>
    /// The Intern DTO that represents the data that is sent or recieved from a client.
    /// </summary>
    public class InternDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
