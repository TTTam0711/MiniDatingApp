using Microsoft.EntityFrameworkCore;
using MiniDatingApp.Data;
using MiniDatingApp.Models;

namespace MiniDatingApp.Services
{
    public class MatchService
    {
        private readonly AppDbContext _context;

        public MatchService(AppDbContext context)
        {
            _context = context;
        }

        // ========================
        // 🔹 LIKE LOGIC
        // ========================
        public async Task LikeAsync(Guid fromId, Guid toId)
        {
            if (fromId == toId) return;

            var exists = await _context.Likes
                .AnyAsync(x => x.FromUserId == fromId && x.ToUserId == toId);

            if (exists) return;

            _context.Likes.Add(new Like
            {
                FromUserId = fromId,
                ToUserId = toId
            });

            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasLiked(Guid fromId, Guid toId)
        {
            return await _context.Likes
                .AnyAsync(x => x.FromUserId == fromId && x.ToUserId == toId);
        }

        public async Task<List<Guid>> GetLikedIds(Guid userId)
        {
            return await _context.Likes
                .Where(l => l.FromUserId == userId)
                .Select(l => l.ToUserId)
                .ToListAsync();
        }

        // ========================
        // 🔹 MATCH LOGIC
        // ========================
        public async Task CreateMatchIfMutual(Guid userA, Guid userB)
        {
            if (userA == userB) return;

            bool isMatch =
                await _context.Likes.AnyAsync(x => x.FromUserId == userA && x.ToUserId == userB)
                && await _context.Likes.AnyAsync(x => x.FromUserId == userB && x.ToUserId == userA);

            if (!isMatch) return;

            bool exists = await _context.Matches.AnyAsync(m =>
                (m.User1Id == userA && m.User2Id == userB) ||
                (m.User1Id == userB && m.User2Id == userA));

            if (exists) return;

            _context.Matches.Add(new Match
            {
                User1Id = userA,
                User2Id = userB
            });

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsMatch(Guid userA, Guid userB)
        {
            return await _context.Matches.AnyAsync(m =>
                (m.User1Id == userA && m.User2Id == userB) ||
                (m.User1Id == userB && m.User2Id == userA));
        }

        public async Task<List<Guid>> GetMatchedIds(Guid userId)
        {
            return await _context.Matches
                .Where(m => m.User1Id == userId || m.User2Id == userId)
                .Select(m => m.User1Id == userId ? m.User2Id : m.User1Id)
                .ToListAsync();
        }

        public async Task<List<Profile>> GetMatchesForUser(Guid userId)
        {
            var matchedIds = await GetMatchedIds(userId);

            return await _context.Profiles
                .Where(p => matchedIds.Contains(p.Id))
                .ToListAsync();
        }

        // ========================
        // 🔹 GET MATCH ENTITY
        // ========================
        public async Task<Match?> GetMatch(Guid userA, Guid userB)
        {
            return await _context.Matches.FirstOrDefaultAsync(m =>
                (m.User1Id == userA && m.User2Id == userB) ||
                (m.User1Id == userB && m.User2Id == userA));
        }

        // ========================
        // 🔹 ADD AVAILABILITY (MULTI SLOT)
        // ========================
        public async Task AddAvailability(int matchId, Availability availability)
        {
            // ❌ KHÔNG XOÁ nữa
            // 👉 cho phép nhiều slot

            availability.MatchId = matchId;

            _context.Availabilities.Add(availability);

            await _context.SaveChangesAsync();
        }

        // ========================
        // 🔹 FIND FIRST COMMON SLOT
        // ========================
        public async Task<(DateTime start, DateTime end)?> GetMatchSlot(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null) return null;

            var userA = match.User1Id;
            var userB = match.User2Id;

            var now = DateTime.UtcNow;
            var max = now.AddDays(21);

            // 🔥 SORT theo thời gian
            var aSlots = await _context.Availabilities
                .Where(x => x.MatchId == matchId && x.UserId == userA && x.EndTime > now && x.EndTime <= max)
                .OrderBy(x => x.StartTime)
                .ToListAsync();

            var bSlots = await _context.Availabilities
                .Where(x => x.MatchId == matchId && x.UserId == userB && x.EndTime > now && x.EndTime <= max)
                .OrderBy(x => x.StartTime)
                .ToListAsync();

            if (!aSlots.Any() || !bSlots.Any())
                return null;

            foreach (var a in aSlots)
            {
                foreach (var b in bSlots)
                {
                    var start = a.StartTime > b.StartTime ? a.StartTime : b.StartTime;
                    var end = a.EndTime < b.EndTime ? a.EndTime : b.EndTime;

                    if (end > start)
                    {
                        return (start.Value, end.Value); // 🔥 FIRST MATCH
                    }
                }
            }

            return null;
        }

        // ========================
        // 🔹 CHECK AVAILABILITY
        // ========================
        public async Task<bool> HasAvailability(int matchId, Guid userId)
        {
            return await _context.Availabilities
                .AnyAsync(a => a.MatchId == matchId && a.UserId == userId);
        }

        public async Task<(bool hasA, bool hasB)> CheckBothAvailability(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null) return (false, false);

            var hasA = await _context.Availabilities
                .AnyAsync(a => a.MatchId == matchId && a.UserId == match.User1Id);

            var hasB = await _context.Availabilities
                .AnyAsync(a => a.MatchId == matchId && a.UserId == match.User2Id);

            return (hasA, hasB);
        }
    }
}