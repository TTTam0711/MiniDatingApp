using Microsoft.EntityFrameworkCore;
using MiniDatingApp.Data;
using MiniDatingApp.Models;

namespace MiniDatingApp.Services
{
    public class ProfileService
    {
        private readonly AppDbContext _context;

        public ProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Profile>> GetAllAsync()
        {
            return await _context.Profiles.ToListAsync();
        }

        public async Task<bool> CreateAsync(Profile profile)
        {
            // Normalize input
            var email = profile.Email.Trim().ToLower();

            // Check duplicate email (case-insensitive)
            var exists = await _context.Profiles
                .AnyAsync(p => p.Email.ToLower() == email);

            if (exists) return false;

            // Clean data trước khi save
            profile.Email = email;
            profile.Name = profile.Name.Trim();
            profile.Gender = profile.Gender.Trim();
            profile.Bio = profile.Bio?.Trim() ?? "";

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
