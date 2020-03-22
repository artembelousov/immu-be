using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Immu.Models
{
    public class User
    {
        [Key]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? AtHomeSince { get; set; }
        [JsonIgnore]
        public Double PreviousHoursAtHome { get; set; }

        public Double HoursAtHome
        {
            get
            {
                var result = PreviousHoursAtHome;
                if (AtHomeSince != null)
                    result += (DateTime.UtcNow - AtHomeSince.Value).TotalHours;
                return result;
            }
        }

        public int Level
        {
            get { return Math.Min((int) Score / 1000 + 1, 3); }
        }

        public List<UserChallenge> Challenges { get; set; }
        public int Score { get; set; }
    }
}
