using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Immu.Models;
using Microsoft.EntityFrameworkCore;

namespace Immu.Data
{
    public class Manager
    {
        private const int ScorePerHourAtHome = 20;

        private readonly ImmuContext _context;

        public Manager(ImmuContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _context.Users.Where(u => u.Email == email).Include(u => u.Challenges).FirstOrDefaultAsync();

            if (user == null)
                throw new KeyNotFoundException();

            return user;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
                throw new DuplicateNameException();

            var newUser = await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return newUser.Entity;
        }

        public async Task UpdateUserAsync(User user)
        {
            if (!_context.Users.Any(u => u.Email == user.Email))
                throw new KeyNotFoundException();

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string email)
        {
            var user = await GetUserAsync(email);

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserScoreAsync(String email)
        {
            var user = await GetUserAsync(email);

            var result = user.HoursAtHome * ScorePerHourAtHome;
            foreach (var userChallenge in user.Challenges)
            {
                try
                {
                    var challenge = await _context.Challenges.FindAsync(userChallenge.ChallengeId);
                    result += challenge.Score * userChallenge.CompletedTimes;
                }
                catch (Exception)
                {
                    //something failed, process properly
                }
            }

            user.Score = (int)result;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Challenge> GetChallengeAsync(Int64 id)
        {
            var challenge = await _context.Challenges.FindAsync(id);

            if (challenge == null)
                throw new KeyNotFoundException();

            return challenge;
        }

        public async Task<Challenge> CreateChallengeAsync(Challenge challenge)
        {
            if (challenge.Id != 0)
                challenge.Id = 0;

            var newChallenge = await _context.Challenges.AddAsync(challenge);

            await _context.SaveChangesAsync();

            return newChallenge.Entity;
        }

        public async Task DeleteChallegeAsync(long id)
        {
            var challenge = await GetChallengeAsync(id);
            if (challenge == null)
                throw new KeyNotFoundException();

            _context.Challenges.Remove(challenge);

            await _context.SaveChangesAsync();
        }

        public async Task<UserChallenge> SetUserChallengeStatus(string email, long challengeId, ChallengeStatus status)
        {
            if (!_context.UserChallenges.Any(uc => uc.UserEmail == email && uc.ChallengeId == challengeId))
            {
                var newUserChallenge = new UserChallenge()
                {
                    UserEmail = email,
                    ChallengeId = challengeId,
                    Status = ChallengeStatus.None,
                    CompletedTimes = 0
                };
                await _context.UserChallenges.AddAsync(newUserChallenge);
                _context.SaveChanges();
            }

            var userChallenge = await _context.UserChallenges
                .Where(uc => uc.UserEmail == email && uc.ChallengeId == challengeId)
                .Include(uc => uc.Challenge)
                .FirstOrDefaultAsync();

            if (status == ChallengeStatus.Completed && userChallenge.CompletedTimes < userChallenge.Challenge.MaxCompletionCount)
            {
                userChallenge.CompletedTimes++;
                userChallenge.Status = userChallenge.CompletedTimes == userChallenge.Challenge.MaxCompletionCount
                    ? ChallengeStatus.Completed
                    : ChallengeStatus.Assigned;
            }
            else
            {
                userChallenge.Status = status;
            }

            var result = _context.UserChallenges.Update(userChallenge);

            _context.SaveChanges();

            return result.Entity;
        }

        public async Task LeaveHomeAsync(string email)
        {
            var user = await GetUserAsync(email);

            if (user.AtHomeSince != null)
            {
                user.PreviousHoursAtHome += (DateTime.UtcNow - user.AtHomeSince.Value).TotalHours;
                user.AtHomeSince = null;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
        }

        public async Task ComeHomeAsync(string email)
        {
            var user = await GetUserAsync(email);

            if (user.AtHomeSince == null)
            {
                user.AtHomeSince = DateTime.UtcNow;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
        }

        public async Task<List<Challenge>> GetChallengesAsync(ChallengeCategory category)
        {
            if (category != ChallengeCategory.Unknown)
            {
                return _context.Challenges.Where(c => c.Category == category).ToList();
            }
            else
            {
                return _context.Challenges.ToList();
            }
        }
    }
}
