using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class ContributionsDates
    {
        [Key]
        public int contributionsDateId {  get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; } // validate: should be after startDate and more than 1 months
        public DateTime? finalEndDate { get; set; } //validate: should be after endDate and more than 1 weeks
    }
}
