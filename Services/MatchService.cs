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
                .Where(l => l.FromUserId == userId) // 🔥 QUAN TRỌNG
                .Select(l => l.ToUserId)
                .ToListAsync();
        }

        // ========================
        // 🔹 MATCH LOGIC
        // ========================
        public async Task CreateMatchIfMutual(Guid userA, Guid userB)
        {
            if (userA == userB) return;

            // Check mutual like
            bool isMatch =
                await _context.Likes.AnyAsync(x => x.FromUserId == userA && x.ToUserId == userB)
                && await _context.Likes.AnyAsync(x => x.FromUserId == userB && x.ToUserId == userA);

            if (!isMatch) return;

            // Check existing match
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
        // 🔹 AVAILABILITY (TEMP - WILL MOVE LATER)
        // ========================

        public async Task AddAvailability(Availability availability)
        {
            // Basic validation
            if (availability.StartTime >= availability.EndTime)
                return;

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
        }

        public async Task<(DateTime start, DateTime end)?> FindFirstCommonSlot(Guid userA, Guid userB)
        {
            var listA = await _context.Availabilities
                .Where(a => a.UserId == userA)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            var listB = await _context.Availabilities
                .Where(a => a.UserId == userB)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            foreach (var a in listA)
            {
                foreach (var b in listB)
                {
                    if (a.StartTime == null || a.EndTime == null ||
                        b.StartTime == null || b.EndTime == null)
                        continue;

                    var start = a.StartTime > b.StartTime ? a.StartTime : b.StartTime;
                    var end = a.EndTime < b.EndTime ? a.EndTime : b.EndTime;

                    if (start < end)
                    {
                        return (start.Value, end.Value);
                    }
                }
            }

            return null;
        }

        public async Task<bool> HasAvailability(Guid userId)
        {
            return await _context.Availabilities
                .AnyAsync(a => a.UserId == userId);
        }
    }
}