using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Services.CouponAPI.DbContexts;
using Restaurant.Services.CouponAPI.Models.Dto;

namespace Restaurant.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;
        protected IMapper _mapper;
        public CouponRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var couponFromDb=await _db.Coupons.FirstOrDefaultAsync(x => x.CouponCode == couponCode);
            return _mapper.Map<CouponDto>(couponFromDb);
        }
    }
}
