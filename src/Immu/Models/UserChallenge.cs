using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Immu.Models
{
    public class UserChallenge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public Int64 Id { get; set; }

        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public string UserEmail { get; set; }

        [JsonIgnore]
        public Challenge Challenge { get; set; }
        public Int64 ChallengeId { get; set; }

        public ChallengeStatus Status { get; set; }
        public int CompletedTimes { get; set; }
    }
}
