using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Common;
using BLL.Dtos.RecentSearchDtos;
using DAL.Helper.Enums;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.RecentSearchServices
{
    public class RecentSearchService : IRecentSearchService
    {
        private readonly IRepository<RecentSearch> _repo;
        private readonly IMapper _mapper;

        public RecentSearchService(IRepository<RecentSearch> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ServiceResult<string>> AddSearchAsync(string userId, string term, RecentSearchType type)
        {
            try
            {
              var temp = term.Trim().ToLower();
                var existing = await _repo.FirstOrDefaultAsync(
                    x => x.UserId == userId && x.SearchTerm == temp&& type==x.SearchType);

                if (existing != null)
                {
                    existing.SearchedAt = DateTime.UtcNow;
                    await _repo.SaveChangesAsync();
                    return ServiceResult<string>.Success("Search updated.");
                }

                var search = new RecentSearch
                {
                    UserId = userId,
                    SearchTerm = term,
                    SearchType = type,
                    SearchedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(search);
                await _repo.SaveChangesAsync();

                return ServiceResult<string>.Success("Search saved.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<List<RecentSearchRS>>> GetRecentJobsAsync(string userId)
        {
            try
            {
                var items = await _repo.Query()
                    .Where(x => x.UserId == userId&&x.SearchType==RecentSearchType.Job)
                    .OrderByDescending(x => x.SearchedAt)
                    .Take(10)
                    .ToListAsync();

                return ServiceResult<List<RecentSearchRS>>
                    .Success(_mapper.Map<List<RecentSearchRS>>(items));
            }
            catch (Exception ex)
            {
                return ServiceResult<List<RecentSearchRS>>
                    .Failure("Error retrieving search history.");
            }
        }

        public async Task<ServiceResult<List<RecentSearchRS>>> GetRecentCoursesAsync(string userId)
        {
            try
            {
                var items = await _repo.Query()
                    .Where(x => x.UserId == userId && x.SearchType == RecentSearchType.Course)
                    .OrderByDescending(x => x.SearchedAt)
                    .Take(10)
                    .ToListAsync();

                return ServiceResult<List<RecentSearchRS>>
                    .Success(_mapper.Map<List<RecentSearchRS>>(items));
            }
            catch (Exception ex)
            {
                return ServiceResult<List<RecentSearchRS>>
                    .Failure("Error retrieving search history.");
            }
        }

        public async Task<ServiceResult<List<RecentSearchRS>>> GetRecentCareerPathsAsync(string userId)
        {
            try
            {
                var items = await _repo.Query()
                    .Where(x => x.UserId == userId && x.SearchType == RecentSearchType.CarrerPath)
                    .OrderByDescending(x => x.SearchedAt)
                    .Take(10)
                    .ToListAsync();

                return ServiceResult<List<RecentSearchRS>>
                    .Success(_mapper.Map<List<RecentSearchRS>>(items));
            }
            catch (Exception ex)
            {
                return ServiceResult<List<RecentSearchRS>>
                    .Failure("Error retrieving search history.");
            }
        }

        public async Task<ServiceResult<string>> ClearRecentJobsAsync(string userId)
        {
            try
            {
                var items = await _repo.Query()
                    .Where(x => x.UserId == userId&&x.SearchType==RecentSearchType.Job)
                    .ToListAsync();

                _repo.RemoveRange(items);
                await _repo.SaveChangesAsync();

                return ServiceResult<string>.Success("History cleared.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<string>> ClearRecentCoursesAsync(string userId)
        {
            try
            {
                var items = await _repo.Query()
                    .Where(x => x.UserId == userId && x.SearchType == RecentSearchType.Course)
                    .ToListAsync();

                _repo.RemoveRange(items);
                await _repo.SaveChangesAsync();

                return ServiceResult<string>.Success("History cleared.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<string>> ClearRecentCareerPathsAsync(string userId)
        {
            try
            {
                var items = await _repo.Query()
                    .Where(x => x.UserId == userId && x.SearchType == RecentSearchType.CarrerPath)
                    .ToListAsync();

                _repo.RemoveRange(items);
                await _repo.SaveChangesAsync();

                return ServiceResult<string>.Success("History cleared.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResult<string>> ClearRecentSearchByIdAsync(string userId, int id)
        {
            try
            {
            var item = _repo.Query().FirstOrDefault(x => x.UserId == userId && x.Id == id);
                if (item == null)
                    return (ServiceResult<string>.Failure("Search not found."));
                _repo.Remove(item);
                await _repo.SaveChangesAsync();
                return ServiceResult<string>.Success("Search cleared.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure(ex.Message);
            }
        }
    }
}
