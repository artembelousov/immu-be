using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Immu.Models
{
    public class Challenge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string AttachedUrl { get; set; }
        public string FullDescription { get; set; }
        [JsonIgnore]
        public List<UserChallenge> UserChallenges { get; set; }
        public int Score { get; set; }
        public int MaxCompletionCount { get; set; }
        public ChallengeCategory Category { get; set; }
    }
}
