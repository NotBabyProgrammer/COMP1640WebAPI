namespace COMP1640WebAPI.BusinesLogic.DTO
{
    public class ContributionsDTOPut
    {
        // functions for Coordinator
        public string? title { get; set; }
        //public List<string>? filePath { get; set; }
        //public List<string>? imagePath { get; set; }
        public bool? approval { get; set; }
        public List<string>? comments { get; set; }
    }
}
