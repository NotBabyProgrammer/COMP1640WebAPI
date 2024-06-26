﻿using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.BusinesLogic.DTO.Contributions
{
    public class ContributionsDTO
    {
        [Key]
        public int contributionId { get; set; }
        public int userId { get; set; }
        public string? title { get; set; } // students name their works
        public List<string>? filePaths { get; set; } // For receiving Word file
        public List<string>? imagePaths { get; set; } // For receiving Image file
        public DateTime? submissionDate { get; set; } // take right now time
        public DateTime? approvalDate { get; set; } // approval date is 14 days after submission
        public DateTime? endDate { get; set; } // same as finalEndDate of ContributionsDate
        public string? status { get; set; } // outdated or on-time
        public bool? approval { get; set; } // false as default
        public string? facultyName { get; set; } // students write in their faculties
        public string? academic { get; set; }
        public string? description { get; set; }
    }
}
